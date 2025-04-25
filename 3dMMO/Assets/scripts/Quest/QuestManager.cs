using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Questsetting;
using System.Linq;
using CharacterInfo;
using System.Threading.Tasks;
//퀘스트 관리. 현재 내 퀘스트들의 상태.
[System.Serializable]
public class Questver
{
    public string questId;
    public string name;
    public string description;
    public bool isMainQuest;
    public string reward;
    public List<QuestGoal> goals = new();
    public bool isCompleted;

    public bool CheckCompleted()
    {
        foreach (var goal in goals)
        {
            if (!goal.IsCompleted)
                return false;
        }

        isCompleted = true;
        return true;
    }
}
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance = null;
    public List<Questver> activeQuests = new();  // 현재 진행 중인 퀘스트
    public List<Questver> completedQuests = new(); // 완료된 퀘스트


    public Quest nowquest;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
        
    }
    public async void AddQuest()
    {
        if (!activeQuests.Any(q => q.questId == nowquest.quest_id))
        {
            var newQuest = ConvertToProgressQuest(nowquest);
            activeQuests.Add(newQuest);
            Debug.Log($"[퀘스트 추가] {newQuest.name}");

            await QuesetServer.Instance.SendQuestToServer(newQuest); //서버에도 저장
        }
    }
    public void LoadFromServer(List<Questver> serverQuests)
    {
        activeQuests.Clear();
        foreach (var quest in serverQuests)
        {
            //AddQuest(quest);
        }
    }
    public async void CompleteQuest(string questId)
    {
        Questver quest = activeQuests.Find(q => q.questId == questId);
        if (quest != null)
        {
            quest.isCompleted = true;
            completedQuests.Add(quest);
            activeQuests.Remove(quest);
        }
        await TryGetNextMainQuest(nowquest.next_quest_id);
        QuestUISetting.Instance.MakeQuestUI();
    }
    public Questver ConvertToProgressQuest(Quest nowquest)
    {
        var quest = new Questver
        {
            questId = nowquest.quest_id,
            name = nowquest.name,
            description = nowquest.description,
            reward = nowquest.reward,
            isMainQuest = nowquest.type.ToLower() == "main",
            isCompleted = false,
            goals = new List<QuestGoal>()
        };
        foreach (var target in nowquest.target_id)
        {
            var goal = new QuestGoal
            {
                goalType = nowquest.quest_type,
                targetId = target,
                requiredAmount = nowquest.required_amount,
                currentAmount = 0
            };
            quest.goals.Add(goal);
        }
        return quest;
    }
    private async Task TryGetNextMainQuest(string completedQuestId)
    {
        // 서버에서 다음 메인 퀘스트 받아오기
        var nextQuest = await QuesetServer.Instance.GetNextMainQuest();

        if (nextQuest != null)
        {
           if (nextQuest.quest_id == completedQuestId)
            {
                nowquest = nextQuest;
                if(nowquest.quest_type== QuestType.Kill)
                {

                }
                else if (nowquest.quest_type == QuestType.Talk)
                {
                    NPCManager.Instance.SetNPCState(nowquest.target_id[0], npcState.mainquest);
                }
                Debug.Log($"[메인퀘스트 연결] 다음 퀘스트: {nextQuest.name}");
            }
            else
            {
                Debug.Log("[메인퀘스트 없음] 다음 퀘스트 조건 불일치");
            }
        }
    }
    ///
    public void OnTalkedTo(string npcId)
    {
        List<Questver> completed = new();

        foreach (var quest in activeQuests)
        {
            foreach (var goal in quest.goals)
            {
                if (goal.goalType == QuestType.Talk && goal.targetId == npcId)
                {
                    goal.currentAmount = goal.requiredAmount;
                }
            }

            if (quest.CheckCompleted())
            {
                completed.Add(quest); // ✅ 리스트만 모아둠
            }
        }
        foreach (var quest in completed)
        {
            CompleteQuest(quest.questId);
            if (quest.isMainQuest)
            {
                CharacterManager myPlayer = PlayerManager.Instance.GetMyCharacterData();
                myPlayer.characterPersonalinfo.storyNum += 0.1f;
                NPCManager.Instance.SetNPCState(npcId, npcState.mainquest);
            }
            Debug.Log($"퀘스트 완료됨: {quest.name}");
        }
    }

    public void OnMonsterKilled(string monsterId)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var goal in quest.goals)
            {
                if (goal.goalType == QuestType.Kill && goal.targetId == monsterId)
                {
                    goal.currentAmount++;
                }
            }

            if (quest.CheckCompleted())
            {
                CompleteQuest(quest.questId);
                Debug.Log($"퀘스트 완료됨: {quest.name}");
            }
        }
    }

    //현재 퀘스트에 따른 NPC의 상태 변화
    public void NpcSetting()
    {
        if (nowquest != null)
        {
            if (nowquest.quest_type == QuestType.Talk)
            {
                for (int i = 0; i < nowquest.target_id.Count; i++)
                {
                    NPCManager.Instance.SetNPCState(nowquest.target_id[i], npcState.mainquest);
                }
            }
            else if (nowquest.quest_type == QuestType.Kill)
            {

            }
        }
       
    }
    
}
