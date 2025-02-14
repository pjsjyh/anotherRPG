using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterInfo
{
    public struct ChaInfo
    {
        public int _hp;
        public int _mp;
        public int _money;
        public int _level;

    };
    public struct ChaInfoOther
    {
        public int _attack;
        public int _defense;
        public int _critical;
        public int _speed;
        public int _luck;
        public int _gem;
    }
    public struct SkillInfo
    {
        public int _attack1;
        public int _attack2;
        public int _attack3;
        public int _attack4;
    };
    public struct QuestInfo
    {
        public string _questId;
        public bool _isComplete;
        public int _requiredAmount;
        public string _questType;
    };
    public struct CharacterPersonalInfo
    {
        public string charater_id;
        public int storyNum;
    }
    public class CharacterManager
    {
        private static CharacterManager instance;
        public ChaInfo myCharacter;
        public ChaInfoOther myCharacterOther;
        public List<QuestInfo> questInfo;
        public CharacterPersonalInfo characterPersonalinfo;
        public string _username = "";
        // private 생성자: 외부에서 인스턴스 생성 불가능
        private CharacterManager() { }

        // public static 메서드: 단일 인스턴스 반환
        public static CharacterManager Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new CharacterManager();
                    instance.questInfo = new List<QuestInfo>();
                }
                return instance;
            }
        }
        public void InitializePlayer(ChaInfoOther playerInfo, string username, int hp, int mp, int money, int level)
        {
            myCharacter._hp = hp;
            myCharacter._mp = mp;
            myCharacter._money = money;
            myCharacter._level = level;
            myCharacterOther = playerInfo;
            _username = username;
        }
        
        public void AddQuest(string id, string questType, int requiredAmount)
        {

            QuestInfo newQuest = new QuestInfo
            {
                _questId = id,
                _questType = questType,
                _requiredAmount = requiredAmount,
                _isComplete = false
            };
            Debug.Log(newQuest);
            questInfo.Add(newQuest);
        }
    }



}
