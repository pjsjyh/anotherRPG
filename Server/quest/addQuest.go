package quest

import (
	"Server/db"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
)

type AcceptQuestRequest struct {
	CharacterID string `json:"character_id"`
	QuestID     string `json:"quest_id"`
}

func AddQuestID(c *gin.Context) {
	character_id := c.PostForm("character_id")
	quest_id := c.PostForm("quest_id")
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

	err = db.DB.QueryRow("SELECT EXISTS(SELECT 1 FROM character_quest WHERE character_id = $1 AND quest_id = $2)", character_id, quest_id).Scan(&exists)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Database error while checking character_quest"})
		return
	}
	if exists {
		c.JSON(http.StatusConflict, gin.H{"status": "error", "message": "Quest already assigned to character"})
		return
	}
	// 3. character_quest 데이터 삽입
	newUUID := uuid.New().String()
	_, err = db.DB.Exec("INSERT INTO character_quest (character_quest_id, character_id, quest_id, progress, is_finish) VALUES ($1, $2, $3, $4, $5)",
		newUUID, character_id, quest_id, 0, false)

	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Failed to insert quest into character_quest"})
		return
	}
	// JSON 응답 반환
	c.JSON(http.StatusOK, gin.H{
		"status":  "success",
		"message": "Quest added successfully",
	})
}
