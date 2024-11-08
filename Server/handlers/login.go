package handlers

import (
	"Server/db"
	"net/http"

	"github.com/gin-gonic/gin"
	"golang.org/x/crypto/bcrypt"
)

func Login(c *gin.Context) {
	id := c.PostForm("id")
	password := c.PostForm("password")

	var storedPassword string
	err := db.DB.QueryRow("SELECT password FROM auth WHERE player_id = $1", id).Scan(&storedPassword)
	isValid := CheckPasswordHash(password, storedPassword)
	if err != nil {
		c.JSON(http.StatusUnauthorized, gin.H{"status": "error", "message": "Invalid username or password"})
		return
	}
	if isValid {
		var getUserName string
		err := db.DB.QueryRow("SELECT username FROM auth WHERE player_id = $1", id).Scan(&getUserName)
		if err != nil {
			c.JSON(http.StatusUnauthorized, gin.H{"status": "error", "message": "Account Error"})
			return
		}
		var getplayerinfo GetPlayerInfo = GetCharacterInfo(id, getUserName)
		c.JSON(http.StatusOK, gin.H{"status": "success", "message": "Login successful", "playerinfo": getplayerinfo})
	} else {
		c.JSON(http.StatusUnauthorized, gin.H{"status": "error", "message": "Invalid username or password"})
		return
	}

}
func CheckPasswordHash(password, hash string) bool {
	err := bcrypt.CompareHashAndPassword([]byte(hash), []byte(password))
	return err == nil // 비밀번호가 일치하면 nil 반환
}
