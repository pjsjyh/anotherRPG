package db

import (
	"database/sql"
	"fmt"
	"log"
	"sync"

	_ "github.com/go-sql-driver/mysql"
	_ "github.com/lib/pq"
)

var DB *sql.DB
var once sync.Once

func InitDB() {
	once.Do(func() {
		var err error
		dsn := "host=localhost port=5432 user=postgres password=000105 dbname=rpg_game  sslmode=disable"
		//dsn := "host=localhost port=5432 user=postgres password=000105 dbname=rpg sslmode=disable"
		DB, err = sql.Open("postgres", dsn)
		if err != nil {
			log.Fatal("Failed to connect to the database:", err)
		}

		err = DB.Ping()
		if err != nil {
			log.Fatal("Database connection failed:", err)
		}

		fmt.Println("Database connection successful!")
		// fmt.Printf("DB address in InitDB: %p\n", DB)
	})

}
