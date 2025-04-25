package quest

import (
	"Server/db"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
	"github.com/lib/pq"
	//"github.com/google/uuid"
)

type QuestData struct {
	QuestID  string         `json:"quest_id"`
	Progress pq.StringArray `json:"progress"`
	IsFinish bool           `json:"is_finish"`
}

func AddQuestID(c *gin.Context) {
	quest_id := c.PostForm("quest_id")
	character_id := c.PostForm("character_id")

	if db.DB == nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Database not initialized"})
		return
	}

	// 1. character_id 존재 여부 확인
	var exists bool
	err := db.DB.QueryRow("SELECT EXISTS(SELECT 1 FROM character WHERE character_id = $1)", character_id).Scan(&exists)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Database error while checking character_id"})
		return
	}
	if !exists {
		c.JSON(http.StatusBadRequest, gin.H{"status": "error", "message": "Invalid character_id"})
		return
	}

	// 2. quest_id 존재 여부 확인
	err = db.DB.QueryRow("SELECT EXISTS(SELECT 1 FROM quest WHERE quest_id = $1)", quest_id).Scan(&exists)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Database error while checking quest_id"})
		return
	}
	if !exists {
		c.JSON(http.StatusBadRequest, gin.H{"status": "error", "message": "Invalid quest_id"})
		return
	}

	// 3. 이미 받은 퀘스트인지 확인
	err = db.DB.QueryRow(`
SELECT EXISTS(
	SELECT 1 FROM character_quest 
	WHERE character_id = $1 AND quest_id = $2
)`, character_id, quest_id).Scan(&exists)

	if err != nil {
		log.Println("❌ 캐릭터 퀘스트 확인 오류:", err)
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Database error while checking character's quests"})
		return
	}
	if exists {
		c.JSON(http.StatusConflict, gin.H{"status": "error", "message": "Quest already assigned to character"})
		return
	}
	progressJson := `{"current": 0}`
	// 4. 퀘스트 추가
	characterQuestID := uuid.New().String()
	log.Println("🧪 INSERT 실행 전:", characterQuestID, character_id, quest_id)
	log.Println("📦 progressJson 값 확인:", progressJson)

	_, err = db.DB.Exec(`
	INSERT INTO character_quest (character_quest_id, character_id, quest_id, progress, is_finish)
	VALUES ($1, $2, $3, $4::jsonb, $5)
`, characterQuestID, character_id, quest_id, progressJson, false)

	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Failed to insert quest into character_quest"})
		return
	}

	// ✅ 성공 응답
	c.JSON(http.StatusOK, gin.H{
		"status":  "success",
		"message": "Quest added successfully",
	})
	// // 4. 새로운 퀘스트 할당
	//newUUID := uuid.New().String()
	// _, err = db.DB.Exec("INSERT INTO character_quest (character_quest_id, character_id, quest_id, progress, is_finish) VALUES ($1, $2, $3, $4, $5)",
	// 	newUUID, character_id, quest_id, 0, false)

	// if err != nil {
	// 	c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Failed to insert quest into character_quest"})
	// 	return
	// }
	// // JSON 응답 반환
	// c.JSON(http.StatusOK, gin.H{
	// 	"status":  "success",
	// 	"message": "Quest added successfully",
	// })
}
