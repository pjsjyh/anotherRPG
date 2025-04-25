using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Questsetting;
public enum QuestState
{
    NotStarted,
    InProgress,
    Completed
}

public enum QuestType
{
    Kill,       // 몬스터 처치
    Talk,       // NPC 대화
    Collect,    // 아이템 수집
    Explore     // 특정 장소 도달
}
[System.Serializable]
public class QuestGoal
{
    public QuestType goalType;
    public string targetId;        // 예: "Slime001", "Item_Herb", "Npc_Bob"
    public int requiredAmount = 1;
    public int currentAmount = 0;

    public bool IsCompleted => currentAmount >= requiredAmount;

    public void AddProgress(int amount = 1)
    {
        currentAmount += amount;
    }
}
public class Quests : MonoBehaviour
{
    public Quest baseInfo;        // 퀘스트 정보 (정적)
    public QuestGet progressInfo; // 진행 정보

    public bool IsCompleted => progressInfo.is_finish;

    public void OnMonsterKilled(string monsterId)
    {
        if (baseInfo.quest_type != QuestType.Kill) return;
        if (!progressInfo.progress.Contains(monsterId))
            progressInfo.progress.Add(monsterId);

        // 예시로 몬스터 3마리 잡는 퀘스트라면:
        if (progressInfo.progress.Count >= 3)
        {
            progressInfo.is_finish = true;
        }
    }

    public void OnTalkedToNPC(string npcId)
    {
        if (baseInfo.quest_type != QuestType.Talk) return;

        if (baseInfo.target_id.Contains(npcId) &&
            !progressInfo.progress.Contains(npcId))
        {
            progressInfo.progress.Add(npcId);
        }

        if (baseInfo.target_id.TrueForAll(id => progressInfo.progress.Contains(id)))
        {
            progressInfo.is_finish = true;
        }
    }
}
