package quest

import (
	"Server/db"
	"encoding/json"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/lib/pq"
)

type AcceptQuestRequest struct {
	CharacterID string `json:"character_id"`
	QuestID     string `json:"quest_id"`
}

type QuestSetting struct {
	QuestData QuestData
	Quest     Quest
}

func GetQuestList(c *gin.Context) {
	characterID := c.Param("character_id")

	if db.DB == nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Database not initialized"})
		return
	}

	// 1️⃣ `character` 테이블에서 `get_quest` 데이터 가져오기
	var getQuestJSON string
	err := db.DB.QueryRow("SELECT get_quest FROM character WHERE character_id = $1", characterID).Scan(&getQuestJSON)
	if err != nil {
		log.Println("❌ 캐릭터 퀘스트 목록 조회 실패:", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to fetch character quests"})
		return
	}
	// 2️⃣ JSON 데이터를 `QuestData` 배열로 변환 (문자열로 저장된 progress 처리)
	log.Println("📌 getQuestJSON 데이터 확인:", getQuestJSON)

	var unescapedJSON string
	if err := json.Unmarshal([]byte(getQuestJSON), &unescapedJSON); err == nil {
		getQuestJSON = unescapedJSON // 🔹 JSON 문자열이었으면 한 번 풀어줌
		log.Println("🔄 JSON 이중 인코딩 해제됨:", getQuestJSON)
	}

	var rawQuestDataList []map[string]interface{}
	if err := json.Unmarshal([]byte(getQuestJSON), &rawQuestDataList); err != nil {
		log.Println("❌ JSON 파싱 오류:", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to parse get_quest data"})
		return
	}

	var questDataList []QuestData
	for _, rawData := range rawQuestDataList {
		var questData QuestData
		questData.QuestID = rawData["quest_id"].(string)
		questData.IsFinish = rawData["is_finish"].(bool)

		// ✅ progress 값이 문자열이면 int로 변환
		if progressData, ok := rawData["progress"].([]interface{}); ok {
			for _, value := range progressData {
				if strValue, valid := value.(string); valid {
					questData.Progress = append(questData.Progress, strValue)
				}
			}
		} else {
			questData.Progress = pq.StringArray{} // 기본값으로 빈 배열
		}

		questDataList = append(questDataList, questData)
	}

	// 3️⃣ `quest_id`들을 모아서 `quest` 테이블에서 상세 정보 가져오기
	var questSettings []QuestSetting
	for _, questData := range questDataList {
		var quest Quest

		err := db.DB.QueryRow(`
			SELECT quest_id, name, description, reward, type, quest_type, required_npcs 
			FROM quest WHERE quest_id = $1`, questData.QuestID).Scan(
			&quest.QuestID, &quest.Name, &quest.Description, &quest.Reward, &quest.Type, &quest.QuestType, &quest.RequiredNpc)

		if err != nil {
			log.Println("⚠️ 퀘스트 정보 조회 실패:", questData.QuestID, err)
			continue
		}

		// 4️⃣ `QuestSetting` 구조체로 합쳐서 리스트에 추가
		questSettings = append(questSettings, QuestSetting{
			QuestData: questData,
			Quest:     quest,
		})
	}

	// ✅ 최종 응답
	log.Println("✅ 퀘스트 목록 조회 성공:", questSettings)
	c.JSON(http.StatusOK, gin.H{
		"status": "success",
		"quests": questSettings,
	})
}
