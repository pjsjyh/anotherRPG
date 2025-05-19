package quest

import (
	"Server/db"
	"encoding/json"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/lib/pq"
)

type QuestProgressData struct {
	QuestID        string         `json:"questId"`
	Name           string         `json:"name"`
	Description    string         `json:"description"`
	Reward         string         `json:"reward"`
	QuestType      string         `json:"questType"`
	TargetID       pq.StringArray `json:"targetId"`       // PostgreSQL 배열
	RequiredAmount int            `json:"requiredAmount"` // 목표 수치
	NextQuestID    string         `json:"next_quest_id"`

	Progress map[string]interface{} `json:"progress"` // jsonb 파싱 결과
	IsFinish bool                   `json:"isFinish"` // 완료 여부
}
type AcceptQuestRequest struct {
	CharacterID string `json:"character_id"`
	QuestID     string `json:"quest_id"`
}

type QuestSetting struct {
	QuestData QuestData
	Quest     Quest
}

func GetQuestList(c *gin.Context) {
	characterId := c.Param("character_id")

	rows, err := db.DB.Query(`
		SELECT 
			cq.quest_id,
			cq.progress,
			cq.is_finish,
			q.name,
			q.description,
			q.reward,
			q.quest_type,
			q.target_id,
			q.required_amount,
			q.next_quest_id
		FROM character_quest cq
		JOIN quest q ON cq.quest_id = q.quest_id
		WHERE cq.character_id = $1
	`, characterId)

	if err != nil {
		log.Println("❌ 퀘스트 불러오기 실패:", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to load quests"})
		return
	}
	defer rows.Close()

	var quests []QuestProgressData
	for rows.Next() {
		var quest QuestProgressData
		var progressRaw []byte

		err := rows.Scan(&quest.QuestID, &progressRaw, &quest.IsFinish, &quest.Name, &quest.Description, &quest.Reward, &quest.QuestType, &quest.TargetID, &quest.RequiredAmount, &quest.NextQuestID)
		if err != nil {
			log.Println("❌ Row 스캔 실패:", err)
			continue
		}

		// jsonb 파싱
		err = json.Unmarshal(progressRaw, &quest.Progress)
		if err != nil {
			log.Println("❌ JSON 파싱 실패:", err)
		}

		quests = append(quests, quest)
	}
	if quests == nil {
		log.Println("📭 quests is nil")
	} else {
		preview, err := json.MarshalIndent(quests, "", "  ")
		if err != nil {
			log.Println("❌ JSON 미리보기 변환 실패:", err)
		} else {
			log.Println("📦 서버에서 보내기 전 quests:", string(preview))
		}
	}
	c.JSON(http.StatusOK, quests)
}
