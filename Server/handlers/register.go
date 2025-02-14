package handlers

import (
	"Server/db"
	"encoding/json"
	"fmt"
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
	"golang.org/x/crypto/bcrypt"
)

type CharacterInfo struct {
	Level int `json:"_level"`
	HP    int `json:"_hp"`
	MP    int `json:"_mp"`
	Money int `json:"_money"`
}
type CharacterInfo2 struct {
	Attack   int `json:"_attack"`
	Defense  int `json:"_defense"`
	Critical int `json:"_critical"`
	Speed    int `json:"_speed"`
	Luck     int `json:"_luck"`
	Gem      int `json:"_gem"`
}
type SkillInfo struct {
	Attack1 int `json:"_attack1"`
	Attack2 int `json:"_attack2"`
	Attack3 int `json:"_attack3"`
	Attack4 int `json:"_attack4"`
}

func Register(c *gin.Context) {
	id := c.PostForm("id")
	password := c.PostForm("password")
	username := c.PostForm("username")
	fmt.Println("DB get id:", id)

	// 1. DB에서 중복 ID 확인
	var existingID string
	err := db.DB.QueryRow("SELECT player_id FROM auth WHERE player_id = $1", id).Scan(&existingID)
	if err == nil {
		// 이미 해당 id가 존재하는 경우
		c.JSON(http.StatusConflict, gin.H{"status": "error",
			"message":    "ID already exists",
			"error_type": "duplicate_id",
			// 구체적인 오류 타입 추가
		})
		return
	}
	// 1-1. DB에서 중복 Username 확인
	var existingNAME string
	err = db.DB.QueryRow("SELECT username FROM auth WHERE username = $1", username).Scan(&existingNAME)
	if err == nil {
		// 이미 해당 id가 존재하는 경우
		c.JSON(http.StatusConflict, gin.H{"status": "error",
			"message":    "Username already exists",
			"error_type": "duplicate_username",
			// 구체적인 오류 타입 추가
		})
		return
	}

	// 2. 비밀번호 해시 처리
	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(password), bcrypt.DefaultCost)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Failed to hash password"})
		return
	}

	// 3. DB에 저장
	//fmt.Println("value: ", id, hashedPassword, username)
	if db.DB == nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Database not initialized"})
		return
	}
	// 비밀번호 해시 처리 후 DB에 저장 postgres
	_, err = db.DB.Exec("INSERT INTO auth (player_id, username, password, status) VALUES ($1, $2, $3, $4)", id, username, hashedPassword, false)
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"status": "error", "message": "Failed to register"})
		return
	}
	//캐릭터 정보 생성
	characterID := createPlayerInfoInDB(id)
	var getplayerinfo GetPlayerInfo = GetCharacterInfo(id, username)
	fmt.Println(getplayerinfo)
	// 성공적으로 저장되었을 때
	c.JSON(http.StatusOK, gin.H{"status": "success", "message": "Registration successful", "playerinfo": getplayerinfo, "charaterID": characterID})

}

func createPlayerInfoInDB(id string) string {
	//characterinfo := CharacterInfo{Level: 1, HP: 100, MP: 100, Money: 0}
	characterinfo2 := CharacterInfo2{Attack: 10, Defense: 5, Critical: 0, Speed: 1, Luck: 1, Gem: 0}
	//skillinfo := SkillInfo{Attack1: 0, Attack2: 0, Attack3: 0, Attack4: 0}

	characterInfojsonData, err := json.Marshal(characterinfo2)
	if err != nil {
		log.Fatal(err)
	}
	// skillinfojsonData, err := json.Marshal(skillinfo)
	// if err != nil {
	// 	log.Fatal(err)
	// }
	chaUUID := uuid.New().String()
	_, err = db.DB.Exec("INSERT INTO character (character_id, hp, mp, money, level, attributes, player_id, storynum) VALUES ($1, $2, $3, $4, $5, $6, $7, $8)", chaUUID, 100, 100, 100, 1, characterInfojsonData, id, 0)
	if err != nil {
		log.Fatal(err)
	}
	return chaUUID
}
