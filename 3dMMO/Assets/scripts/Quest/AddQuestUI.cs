using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Quest;
public class AddQuestUI : MonoBehaviour
{
    public GameObject QuestPrefab;
    private TextMeshProUGUI prefab_title;
    private TextMeshProUGUI prefab_des;

    public void Awake()
    {
       
    }
    public void makeQuestUI()
    {
        foreach(Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("ªË¡¶");
        foreach (QuestInfo quest in QuestManager.Instance.questInfo)
        {
            GameObject questUI = Instantiate(QuestPrefab, this.transform);
            prefab_title = questUI.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            prefab_des = questUI.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
            prefab_title.text = quest._title;
            prefab_des.text = quest._description;
        }
    }
}
