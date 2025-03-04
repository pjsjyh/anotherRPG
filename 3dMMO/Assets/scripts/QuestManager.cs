using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Questsetting;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance = null;
    public List<QuestGet> questSet; //현재 내 퀘스트 현황
    public List<QuestInfo> questInfo; //전체 퀘스트. [받은퀘스트 현황 , 퀘스트 내용]

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

}
