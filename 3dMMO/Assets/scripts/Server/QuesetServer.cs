using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ApiUtilities;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using Quest;
using MyServerManager;
using CharacterInfo;
using System.Threading.Tasks;
public class QuesetServer : MonoBehaviour
{
    public class Quest
    {
        public string quest_id;
        public string name;
        public string description;
        public string reward;
        public string type;
    }

    public void GetQuestByID(string questID)
    {
        StartCoroutine(GetQuest(questID));
    }
    public async Task addQuest(string getid)
    {
        var values = new Dictionary<string, string>
        {
            { "quest_id", getid },
            { "character_id", CharacterManager.Instance.characterPersonalinfo.charater_id },
        };
        UnityWebRequest response = await ServerManager.Instance.PostAsync(ApiUrls.QuestAddUrl, values);

        await FetchQuestList(CharacterManager.Instance.characterPersonalinfo.charater_id);
    }
    IEnumerator GetQuest(string questID)
    {
        if (string.IsNullOrEmpty(questID))
        {
            Debug.LogError("❌ questID가 비어 있음!");
            yield break;
        }

        string url = ApiUrls.QuesteUrl + questID;
        Debug.Log($"✅ 요청 URL: {url}"); // 요청 URL이 제대로 되는지 확인

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ 서버 요청 실패: {request.responseCode} {request.error}");
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"✅ 서버 응답: {jsonResponse}");
                Quest q = JsonConvert.DeserializeObject<Quest>(jsonResponse);
                QuestManager.Instance.SettingQuestUI(q.name, q.description, q.reward, q.quest_id);
            }
        }
    }
    public async Task FetchQuestList(string characterId)
    {
        string url = ApiUrls.QuestList + characterId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone) await System.Threading.Tasks.Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"✅ 퀘스트 목록 불러오기 성공: {jsonResponse}");

                // 🚀 UI 업데이트
                await GetQuestList(jsonResponse);
            }
            else
            {
                Debug.LogError($"❌ 퀘스트 목록 불러오기 실패: {request.error}");
            }
        }
    }
    public async Task GetQuestList(string jsonResponse)
    {
        var response = JsonConvert.DeserializeObject<QuestResponse>(jsonResponse);

        if (response != null && response.quests != null)
        {
            Debug.Log(response);
            // ✅ 기존 리스트를 초기화하고 새 데이터를 추가
            QuestManager.Instance.questInfo.Clear();
            QuestManager.Instance.questInfo.AddRange(response.quests);

            Debug.Log("✅ 퀘스트 목록 저장 완료!");
            foreach (var quest in response.quests)

            await Task.Delay(100);
        }
        else
        {
            Debug.LogError("❌ 퀘스트 목록 파싱 실패: 응답이 비어 있음.");
        }
    }
}

