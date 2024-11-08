package monster

import (
	"Server/db"

	_ "github.com/lib/pq"
)

type Monster struct {
	MonsterInfoID    string
	MonsterID        string
	PositionX        int
	PositionY        int
	PositionZ        int
	LastResponseTime int
	IsActive         bool
}

func MonsterSetting() ([]Monster, error) {
	query := `
        SELECT monsterinfo_id, monster_id, position_x, position_y, position_z, last_response_time, is_active
        FROM monsterinfo
        WHERE is_active = TRUE;
    `

	// 쿼리 실행
	rows, err := db.DB.Query(query)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	// 결과를 저장할 슬라이스
	var monsters []Monster

	// 각 행을 Monster 구조체로 스캔하여 슬라이스에 추가
	for rows.Next() {
		var monster Monster
		err := rows.Scan(&monster.MonsterInfoID, &monster.MonsterID, &monster.PositionX, &monster.PositionY, &monster.PositionZ, &monster.LastResponseTime, &monster.IsActive)
		if err != nil {
			return nil, err // 스캔 중 오류가 발생하면 즉시 반환
		}
		monsters = append(monsters, monster) // 슬라이스에 추가
	}

	// 반복문 에러 확인
	if err = rows.Err(); err != nil {
		return nil, err
	}

	// 몬스터 리스트와 nil 오류 반환
	return monsters, nil
}
