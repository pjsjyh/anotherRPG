using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Questsetting;
using System.Linq;
using CharacterInfo;
using System.Threading.Tasks;
using Newtonsoft.Json;
//퀘스트 관리. 현재 내 퀘스트들의 상태.
[System.Serializable]
public class Questver
{
    public string questId;
    public string name;
    public string description;
    public bool isMainQuest;
    public string type;
    public QuestType quest_type;

    [JsonProperty("targetId")]
    public List<string> target_id;
    public List<Vector3> target_pos = new List<Vector3>();

    public string reward;
    public List<QuestGoal> goals = new();
    public bool isCompleted;
    public string next_quest_id;

    public bool CheckCompleted()
    {
        foreach (var goal in goals)
        {
            if (!goal.IsCompleted)
            {
                if (goal.requiredAmount == goal.currentAmount)
                {
                    isCompleted = true;
                    return true;
                }
            }
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
    //새 퀘스트 추가
    public async void AddQuest()
    {
        if (!activeQuests.Any(q => q.questId == nowquest.quest_id))
        {
            var newQuest = ConvertToProgressQuest(nowquest);
            Debug.Log(newQuest.isCompleted + " " + newQuest.target_id[0]+"  "+newQuest.next_quest_id);

            newQuest.target_pos.Add(NPCManager.Instance.GetNPCpos(newQuest.target_id[0]));
            activeQuests.Add(newQuest);
            Debug.Log($"[퀘스트 추가] {newQuest.name}");
            if (newQuest.isMainQuest)
            {
                Debug.Log(newQuest.target_id);
                //NPCManager.Instance.SetNPCState(newQuest.target_id, npcState.mainquest);
            }
            await QuesetServer.Instance.SendQuestToServer(newQuest); //서버에도 저장
        }
    }
    //퀘스트 완료시 호출
    public async void CompleteQuest(string questId)
    {
        Questver quest = activeQuests.Find(q => q.questId == questId);
        if (quest != null)
        {
            quest.isCompleted = true;
            completedQuests.Add(quest);
            activeQuests.Remove(quest);
            CharacterManager playerManager = PlayerManager.Instance.GetMyCharacterData();
            playerManager.GetMoneyReward(int.Parse(quest.reward));
        }
        Debug.Log(quest.next_quest_id);
        await TryGetNextMainQuest(quest.next_quest_id);
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
            quest_type = nowquest.quest_type,
            type = nowquest.type,
            target_id = nowquest.target_id,
            isCompleted = false,
            goals = new List<QuestGoal>(),
            next_quest_id = nowquest.next_quest_id
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
        var nextQuest = await QuesetServer.Instance.GetNextMainQuest(completedQuestId);
        Debug.Log(nextQuest);
        if (nextQuest != null)
        {
           if (nextQuest.quest_id == completedQuestId)
            {
                nowquest = nextQuest;
                if (nowquest.quest_type == QuestType.Kill)
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
   
    public void QuestClearBtnClick()
    {
        Debug.Log(activeQuests.Count);
        foreach (var quest in activeQuests.ToList())
        {
            if (quest.isCompleted)
            {
                //QuestUISetting.Instance.OnQuestClearBtn();

                CompleteQuest(quest.questId);
            }
        }

    }
    public void OnTalkedTo(string npcId)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var goal in quest.goals)
            {
                if (goal.goalType == QuestType.Talk && goal.targetId == npcId)
                {
                    goal.currentAmount = goal.requiredAmount;
                    quest.isCompleted = true;
                }
            }
            if (quest.CheckCompleted())
            {
                //completed.Add(quest); // 리스트만 모아둠
            }
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
                    if (goal.currentAmount == goal.requiredAmount) quest.isCompleted = true;
                }
            }
            if (quest.isCompleted)
            {
                CharacterManager chi = PlayerManager.Instance.GetMyCharacterData();
                NPCManager.Instance.SetNPCState(chi.characterPersonalinfo.nextstory_npc_id, npcState.mainquest);

            }
        }
    }
    public bool CheckGoalsCompletedOnly(List<QuestGoal> goals)
    {
        foreach (var goal in goals)
        {
            if (!goal.IsCompleted)
                return false;
        }

        return true; // 모든 목표 완료 → 대기 상태
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
