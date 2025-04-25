package quest

import (
	"Server/db"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/lib/pq"
)

// 퀘스트 구조체
type Quest struct {
	QuestID        string         `json:"quest_id"`
	Name           string         `json:"name"`
	Description    string         `json:"description"`
	Reward         string         `json:"reward"`
	Type           string         `json:"type"`
	QuestType      string         `json:"quest_type"`
	TargetId       pq.StringArray `json:"target_id"`
	RequiredAmount int            `json:"required_amount"`
	NextQuestID    string         `json:"next_quest_id"`
}
type GoalResponse struct {
	GoalType       string `json:"goalType"`
	TargetId       string `json:"targetId"`
	RequiredAmount int    `json:"requiredAmount"`
	CurrentAmount  int    `json:"currentAmount"`
}

type QuestResponse struct {
	QuestID     string         `json:"questId"`
	Title       string         `json:"title"`
	Description string         `json:"description"`
	IsMainQuest bool           `json:"isMainQuest"`
	Goals       []GoalResponse `json:"goals"`
	IsCompleted bool           `json:"isCompleted"`
}

// 퀘스트 목록 가져오기 (Gin용)
func GetQuests(c *gin.Context) {
	// DB 연결 확인
	log.Println("❌ DB 조회 오류:")
	if db.DB == nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Database not initialized"})
		return
	}

	// 퀘스트 목록 조회
	rows, err := db.DB.Query(`
	SELECT quest_id, name, description, reward, type, quest_type, target_id, required_amount
	FROM quest
`)
	if err != nil {
		log.Println("❌ DB 조회 오류:", err)
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to fetch quests"})
		return
	}
	defer rows.Close()

	var quests []Quest
	for rows.Next() {
		var q Quest
		if err := rows.Scan(
			&q.QuestID,
			&q.Name,
			&q.Description,
			&q.Reward,
			&q.Type,
			&q.QuestType,
			&q.TargetId,
			&q.RequiredAmount,
			&q.NextQuestID,
		); err != nil {
			log.Println("❌ 데이터 변환 오류:", err)
			c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to parse quests"})
			return
		}
		quests = append(quests, q)
	}

	// JSON 응답 반환
	c.JSON(http.StatusOK, gin.H{"quests": quests})
}
func GetQuestByID(c *gin.Context) {
	questID := c.Param("id") // URL에서 quest ID 가져오기

	if db.DB == nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Database not initialized"})
		return
	}

	// 퀘스트 정보 조회
	var q Quest
	err := db.DB.QueryRow("SELECT quest_id, name, description, reward, type, quest_type, target_id, required_amount, next_quest_id FROM quest WHERE quest_id = $1", questID).
		Scan(&q.QuestID, &q.Name, &q.Description, &q.Reward, &q.Type, &q.QuestType, &q.TargetId, &q.RequiredAmount, &q.NextQuestID)

	if err != nil {
		log.Println("❌ 퀘스트 조회 실패:", err)
		c.JSON(http.StatusNotFound, gin.H{"error": "Quest not found"})
		return
	}

	// JSON 응답 반환
	c.JSON(http.StatusOK, q)
}
