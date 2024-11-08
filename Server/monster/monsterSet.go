package monster

import (
	"fmt"
	"net/http"

	"github.com/gin-gonic/gin"
	_ "github.com/lib/pq"
)

func MonsterSet(c *gin.Context) {
	monsters, err := MonsterSetting()
	if err != nil {
		c.JSON(http.StatusConflict, gin.H{"status": "error",
			"message": "Failed to get monster",
			// 구체적인 오류 타입 추가
		})
	}

	for _, monster := range monsters {
		fmt.Printf("Monster: %+v\n", monster)
	}
	c.JSON(http.StatusOK, gin.H{"status": "success", "message": "Get monsterinfo successful", "monster": monsters})
}
