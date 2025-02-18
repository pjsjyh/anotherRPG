package quest

import (
	"Server/db"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
)

type AcceptQuestRequest struct {
	CharacterID string `json:"character_id"`
	QuestID     string `json:"quest_id"`
}
type QuestList struct {
	QuestID     string `json:"quest_id"`
	Name        string `json:"name"`
	Description string `json:"description"`
	Reward      string `json:"reward"`
	Type        string `json:"type"`
	Progress    int    `json:"progress"`
	IsFinish    bool   `json:"is_finish"`
}

func GetQuestList(c *gin.Context) {
	characterID := c.Param("character_id")

	if db.DB == nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Database not initialized"})
		return
	}

	// 1. 퀘스트 리스트
	rows, err := db.DB.Query(`
		SELECT q.quest_id, q.name, q.description, q.reward, q.type, cq.progress, cq.is_finish
		FROM quest q 
		JOIN character_quest cq ON q.quest_id = cq.quest_id 
		WHERE cq.character_id = $1`, characterID)

	if err != nil {
		log.Println("❌ 퀘스트 조회 실패:", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to fetch quests"})
		return
	}
	defer rows.Close()

	// 2. quest_id 존재 여부 확인
	var quests []QuestList
	for rows.Next() {
		var q QuestList
		if err := rows.Scan(&q.QuestID, &q.Name, &q.Description, &q.Reward, &q.Type, &q.Progress, &q.IsFinish); err != nil {
			log.Println("❌ 데이터 변환 오류:", err)
			c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to parse quests"})
			return
		}
		quests = append(quests, q)
	}
	log.Println("✅ 퀘스트 목록 조회 성공:", quests)
	// JSON 응답 반환
	c.JSON(http.StatusOK, gin.H{
		"status": "success",
		"quests": quests,
	})
}
