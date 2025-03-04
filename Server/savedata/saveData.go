package savedata

import (
	"Server/db"
	"encoding/json"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
)

type Character struct {
	CharacterID string          `json:"character_id"`
	HP          int             `json:"hp"`
	MP          int             `json:"mp"`
	Money       int             `json:"money"`
	Attributes  json.RawMessage `json:"attributes"` // ✅ JSON 필드
	PlayerID    string          `json:"player_id"`
	Level       int             `json:"level"`
	Storynum    float32         `json:"storynum"`
	GetQuest    json.RawMessage `json:"get_quest"` // ✅ JSON 필드
}

func SaveData(c *gin.Context) {
	var charData Character

	// JSON 바디를 디코딩
	if err := c.ShouldBindJSON(&charData); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"status": "error", "message": "Invalid JSON data"})
		return
	}
	charDataJSON, _ := json.MarshalIndent(charData, "", "  ")
	log.Println("📌 받은 캐릭터 데이터:\n", string(charDataJSON))
	charData.Attributes = json.RawMessage(charData.Attributes) // ✅ JSON 형식으로 저장됨

	// DB에 업데이트 (예제 코드)
	_, err := db.DB.Exec(`UPDATE character 
		SET hp = $1, mp = $2, money = $3, level = $4, storynum = $5, attributes = $6, get_quest = $7
		WHERE character_id = $8`,
		charData.HP, charData.MP, charData.Money, charData.Level,
		charData.Storynum, charData.Attributes, charData.GetQuest, charData.CharacterID)

	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Failed to update character data"})
		return
	}

	// 성공 응답
	c.JSON(http.StatusOK, gin.H{"status": "success", "message": "Character data saved successfully"})

}
