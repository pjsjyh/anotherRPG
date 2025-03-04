using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SettingAccountManager;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private string characterData;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }
    public void saveData(string data)
    {
        characterData = data;
    }
    public async void InitializeGameManagers()
    {
        Debug.Log("✅ GameManager 초기화 중...");
        QuestManager.Instance.Initialize();
        await SettingAccount.DoSettingAccount(characterData);
        // 다른 매니저들 초기화 (예: QuestUISetting, StoryManager 등)
    }
}
