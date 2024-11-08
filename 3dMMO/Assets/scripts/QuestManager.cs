﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using CharacterInfo;
namespace Quest
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance = null;

        public GameObject popupQuest;
        public TextMeshProUGUI questTitle;
        public TextMeshProUGUI questDescription;
        public TextMeshProUGUI questReward;
        public Button questOKBtn;
        private string jsonFilePath = "Assets/scripts/Quest/QuestInfo.json";
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
            }
            else
            {
                Destroy(gameObject); // 이미 인스턴스가 있으면 파괴
            }
        }


        public void PopupQuest(bool isOn, string storynum, string filename, string questnum)
        {
            if (isOn == false)
            {
                popupQuest.SetActive(false);
            }
            else
            {
                Debug.Log("!!!!");
                LoadQuestData(storynum, filename, questnum);
                popupQuest.SetActive(true);
            }
        }
        private void LoadQuestData(string storynum, string questnum, string title)
        {
            string jsonData = System.IO.File.ReadAllText(jsonFilePath); // 파일 읽기
            Dictionary<string, List<QuestSet>> questDict = JsonConvert.DeserializeObject<Dictionary<string, List<QuestSet>>>(jsonData);
            Debug.Log("!!!!" + questDict);

            // questnum에 해당하는 퀘스트 리스트를 찾음
            if (questDict.TryGetValue(storynum, out List<QuestSet> quests))
            {
                QuestSet quest = quests.Find(q => q.id == questnum);
                Debug.Log(quest);
                if (quest != null)
                {
                    SettingQuestUI(quest.title, quest.dialogue, quest.reward);
                }
            }
        }
        public void SettingQuestUI(string title, string description, string reward)
        {
            questTitle.text = title;
            questDescription.text = description;
            questReward.text = reward;
        }
        public void ClickQuest()
        {
            //CharacterManager.Instance.AddQuest(questTitle, questType, requiredAmount);
        }
    }
}
