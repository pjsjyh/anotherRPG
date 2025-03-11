using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Newtonsoft.Json;
namespace Questsetting
{

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

    public class Quest
    {
        public string quest_id;
        public string name;
        public string description;
        public string reward;
        public string type;
        public string quest_type;
        public List<string> required_npcs;
    }
    public class QuestGet
    {
        public string quest_id;
        public bool is_finish;
        public List<string> progress;
    }
    public class QuestInfo
    {
        [JsonProperty("QuestData")]
        public QuestGet questget;

        [JsonProperty("Quest")]
        public Quest quest;
    }
}