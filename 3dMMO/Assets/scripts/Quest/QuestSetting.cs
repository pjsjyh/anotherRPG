using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Quest;

[System.Serializable]
public class QuestSet
{
    public string id;
    public string title;
    public string dialogue;
    public string reward;
}
[System.Serializable]
public class QuestList
{
    public List<QuestSet> quest0;
}

public class QuestSetting : StoryManager
{

    public GameObject questPrefab;
    public GameObject questParent;
    public void SettingQuestUI(string title, string description, string reward)
    {
    }                                                                               
    public void ClickQuest()
    {

    }


}
