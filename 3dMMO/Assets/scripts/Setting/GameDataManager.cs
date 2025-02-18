using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        Debug.Log("🚀 게임 종료 감지됨! 데이터 저장 중...");
        SaveGameData();
    }

    private void SaveGameData()
    {
        // 여기에 저장 로직 추가 (예: JSON 파일 저장, PlayerPrefs 저장 등)
        Debug.Log("✅ 게임 데이터가 성공적으로 저장되었습니다.");
    }
}
