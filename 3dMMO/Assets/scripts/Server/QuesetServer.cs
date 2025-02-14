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
    public async void addQuest(string getid)
    {
        var values = new Dictionary<string, string>
        {
            { "quest_id", getid },
            { "character_id", CharacterManager.Instance.characterPersonalinfo.charater_id },
        };
        UnityWebRequest response = await ServerManager.Instance.PostAsync(ApiUrls.QuestAddUrl, values);
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

}

