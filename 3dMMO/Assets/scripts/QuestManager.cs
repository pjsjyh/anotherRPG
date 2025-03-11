using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Questsetting;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance = null;
    public List<QuestGet> questSet; //���� �� ����Ʈ ��Ȳ
    public List<QuestInfo> questInfo; //��ü ����Ʈ. [��������Ʈ ��Ȳ , ����Ʈ ����]
    public Quest nowquest;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Initialize()
    {
        questSet = new List<QuestGet>();
        questInfo = new List<QuestInfo>();
    }
    public void NpcSetting()
    {
        if (nowquest != null)
        {
            if (nowquest.quest_type == "talk_to_npcs")
            {
                for (int i = 0; i < nowquest.required_npcs.Count; i++)
                {
                    NPCManager.Instance.SetNPCState(nowquest.required_npcs[i], npcState.mainquest);
                }
            }
            else if (nowquest.quest_type == "kill_moster")
            {

            }
        }
       
    }
}
