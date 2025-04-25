using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Questsetting;
public class AddQuestUI : MonoBehaviour
{
    //왼쪽 사이드 퀘스트 목록 생성
    public GameObject QuestPrefab;
    private TextMeshProUGUI prefab_title;
    private TextMeshProUGUI prefab_des;

    public void makeQuestUI()
    {
        foreach(Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Questver q in QuestManager.Instance.activeQuests)
        {
            GameObject questUI = Instantiate(QuestPrefab, this.transform);
            prefab_title = questUI.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            prefab_des = questUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            prefab_title.text = q.name;
            prefab_des.text = q.description;
        }
    }
}
