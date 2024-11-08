package main

import (
	"Server/db"
	"Server/handlers"
	"Server/monster"
	"fmt"

	"github.com/gin-gonic/gin"
)

func main() {
	r := gin.Default()
	fmt.Printf("DB address in main:")

	// 데이터베이스 초기화
	db.InitDB()

	// 라우트 설정
	r.POST("/register", handlers.Register)
	r.POST("/login", handlers.Login)
	r.POST("/monsterSet", monster.MonsterSet)

	// 서버 시작
	r.Run(":8080")
}
