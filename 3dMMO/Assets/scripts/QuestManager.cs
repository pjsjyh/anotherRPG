using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Questsetting;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance = null;
    public List<QuestGet> questSet; //���� �� ����Ʈ ��Ȳ
    public List<QuestInfo> questInfo; //��ü ����Ʈ. [��������Ʈ ��Ȳ , ����Ʈ ����]

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
