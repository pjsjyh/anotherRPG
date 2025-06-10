using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SettingAccountManager;
using CharacterInfo;
using System;
using System.Threading.Tasks;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private string characterData;
    public CharacterManager myDataSetting;
    public event Action OnPlayerDataReady;
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
        myDataSetting = new CharacterManager();
    }
    public void saveData(string data)
    {
        characterData = data;
    }
    public async Task InitializeGameManagers()
    {
        Debug.Log("GameManager 초기화 중...");
        try
        {
            await SettingAccount.DoSettingAccount(characterData);
            Debug.Log("매니저 셋팅 완료");
        }
        catch (Exception e)
        {
            Debug.LogError("SettingAccount에서 예외 발생: " + e.Message);
        }
        // 다른 매니저들 초기화 (예: QuestUISetting, StoryManager 등)
    }
    public void PlayerDataReady()
    {
        Debug.Log("실행!");
        OnPlayerDataReady?.Invoke();
    }

}
