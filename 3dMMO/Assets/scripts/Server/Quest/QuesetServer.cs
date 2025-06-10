using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ApiUtilities;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using MyServerManager;
using CharacterInfo;
using System.Threading.Tasks;
using Questsetting;

public class QuesetServer : MonoBehaviour
{

    public static QuesetServer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 이동 시 유지

    }
    private void Start()
    {
        GameManager.Instance.OnPlayerDataReady += async () =>
        {
            var myPlayer = PlayerManager.Instance.GetMyCharacterData();
            await LoadSavedQuests(myPlayer.characterPersonalinfo.charater_id);
        };
    }
    //quest id를 이용해 quest정보 불러오기
    public void GetQuestByID(string questID, bool story=false)
    {
        if(questID !=null)
            StartCoroutine(GetQuest(questID, story));
    }
  
    IEnumerator GetQuest(string questID, bool story)
    {
        if (string.IsNullOrEmpty(questID)|| questID==""||questID=="0"|| questID == "clear")
        {
            yield break;
        }

        string url = ApiUrls.QuesteUrl + questID;
        Debug.Log($"요청 URL: {url}"); // 요청 URL이 제대로 되는지 확인

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"서버 요청 실패: {request.responseCode} {request.error}");
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"서버 응답: {jsonResponse}");
                Quest q = JsonConvert.DeserializeObject<Quest>(jsonResponse);
                QuestManager.Instance.nowquest = q;
                Debug.Log("퀘스트 가져옴 " + q.next_quest_id+" "+q.target_id[0]);
                if (story) openQuestUI();

            }
        }
    }
    private void openQuestUI()
    {
        QuestUISetting.Instance.SettingQuestUI();

    }

    //퀘스트 받으면 서버에 받았다고 전달
    public async Task SendQuestToServer(Questver quest)
    {
        var myPlayer = PlayerManager.Instance.GetMyCharacterData();
        var goal = quest.goals[0]; // 현재 목표 하나일 때 기준
        Debug.Log(goal.currentAmount+" "+goal.requiredAmount);
        var values = new Dictionary<string, string>
    {
        { "quest_id", quest.questId },
        { "character_id", myPlayer.characterPersonalinfo.charater_id },
        { "progress", goal.currentAmount.ToString() },
        { "progress_detail", $"{goal.currentAmount}:{goal.requiredAmount}" },
        { "is_finish", quest.isCompleted.ToString().ToLower() }
    };

        UnityWebRequest response = await ServerManager.Instance.PostAsync(ApiUrls.QuestAddUrl, values);

        if (response.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("서버에 퀘스트 저장 성공!");
        }
        else
        {
            Debug.LogError("서버 퀘스트 저장 실패: " + response.error);
        }
    }
    //서버에서 자신이 받은 퀘스트 목록 불러오기
    public async Task LoadSavedQuests(string characterId)
    {
        string url = ApiUrls.QuestList + characterId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        await request.SendWebRequest();
        Debug.Log("퀘스트 로드");
        if (request.result == UnityWebRequest.Result.Success)
        {
            var json = request.downloadHandler.text;
            if (json == null) return;
            var serverQuests = JsonConvert.DeserializeObject<List<Questver>>(json);

            if (serverQuests == null)
            {
                return;
            }

            foreach (var quest in serverQuests)
            {
                Debug.Log(quest.isCompleted + " " + quest.next_quest_id);
                if (!quest.isCompleted)
                {
                    quest.target_pos.Add(NPCManager.Instance.GetNPCpos(quest.target_id[0]));
                    QuestManager.Instance.activeQuests.Add(quest);
                    if (quest.type=="main")
                    {
                        Debug.Log(quest.target_id[0]);
                        NPCManager.Instance.SetNPCState(quest.target_id[0], npcState.mainquest);
                    }
                }
                else
                    QuestManager.Instance.completedQuests.Add(quest);
            }
            Debug.Log("서버 퀘스트 로드 완료!");
            QuestUISetting.Instance.MakeQuestUI();
        }
        else
        {
            Debug.LogError($"퀘스트 불러오기 실패: {request.error}");
        }
    }
    public async Task<Quest> GetNextMainQuest(string completedQuestId)
    {
        // 서버 요청해서 다음 메인퀘스트 받아오기
        GetQuestByID(completedQuestId);
        Quest nextQuest = null;

        // 가짜 테스트용 코드
        await Task.Delay(100); // 서버 통신 딜레이
        // nextQuest = 결과물

        return nextQuest;
    }
}

