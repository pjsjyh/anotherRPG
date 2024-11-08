package handlers

import (
	"Server/db"
	"fmt"
)

type GetPlayerInfo struct {
	ID         string
	HP         string
	MP         string
	Money      string
	Level      string
	Attributes string `json:"character"`
	PlayerID   string
	Username   string
}

func GetCharacterInfo(id, username string) GetPlayerInfo {
	var playerInfo GetPlayerInfo
	err := db.DB.QueryRow("SELECT character_id, hp, mp, money,level, attributes, player_id FROM character WHERE player_id = $1", id).Scan(&playerInfo.ID, &playerInfo.HP, &playerInfo.MP, &playerInfo.Money, &playerInfo.Level, &playerInfo.Attributes, &playerInfo.PlayerID)
	if err != nil {
		fmt.Println(playerInfo)
		return playerInfo
	}
	playerInfo.Username = username
	// var characterData CharacterInfo
	// var skillData SkillInfo

	// json.Unmarshal([]byte(playerInfo.Character), &characterData) // JSON -> 구조체
	// json.Unmarshal([]byte(playerInfo.Skill), &skillData)

	// fmt.Println(characterData)
	// fmt.Println(skillData)

	//  response := map[string]interface{}{
	//     "character": characterData,
	//     "skill":     skillData,
	// }

	// // 응답을 JSON으로 변환
	// jsonResponse, err := json.Marshal(response)
	// if err != nil {
	//     log.Fatal(err)
	// }

	// // 클라이언트로 JSON 응답 보내기
	// fmt.Fprintln(w, string(jsonResponse))  // 'w'는 http.ResponseWriter
	// playerInfo 구조체를 JSON으로 변환
	fmt.Println(playerInfo)
	return playerInfo
}
