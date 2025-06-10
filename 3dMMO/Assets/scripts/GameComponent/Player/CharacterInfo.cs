using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
namespace CharacterInfo
{
    public class ChaInfo
    {
        public ReactiveProperty<int> _hp = new(100);
        public ReactiveProperty<int> _mp = new(50);
        public ReactiveProperty<int> _money = new(1000);
        public ReactiveProperty<int> _level = new(1);
    }

    public class ChaInfoOther
    {
        public ReactiveProperty<int> _attack = new(10);
        public ReactiveProperty<int> _defense = new(5);
        public ReactiveProperty<int> _critical = new(1);
        public ReactiveProperty<int> _speed = new(3);
        public ReactiveProperty<int> _luck = new(2);
        public ReactiveProperty<int> _gem = new(0);
    }

    public class CharacterPersonalInfo
    {
        public string charater_id;
        public string nextstory_name = null;
        public string currentstory_name = null;
        public string nextstory_npc_id = null;
        public float storyNum;
        public float[] chaPosition;
        public float[] chaRotation;
    }
    public class CharacterManager
    {
       // private static CharacterManager instance;
        private static CharacterRepository characterRepo = new CharacterRepository();
        public ChaInfo myCharacter;
        public ChaInfoOther myCharacterOther;
        public CharacterPersonalInfo characterPersonalinfo;
        public string _username = "";
        public GameObject playerObj;
        public CharacterManager Clone()
        {
            return new CharacterManager
            {
                _username = this._username,
                myCharacter = this.myCharacter, // 얕은 복사
                myCharacterOther = this.myCharacterOther,
                characterPersonalinfo = this.characterPersonalinfo
            };
        }
        //public static CharacterManager Instance
        //{

        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new CharacterManager();
        //        }
        //        return instance;
        //    }
        //}
        public CharacterManager()
        {
            myCharacter = new ChaInfo();
            myCharacterOther = new ChaInfoOther();
            characterPersonalinfo = new CharacterPersonalInfo();
        }
        public void ManagerSetting()
        {
            ChaInfoOther managerInfo = new ChaInfoOther
            {
                _attack = new ReactiveProperty<int>(9999),
                _defense = new ReactiveProperty<int>(9999),
                _critical = new ReactiveProperty<int>(9999),
                _speed = new ReactiveProperty<int>(100),
                _luck = new ReactiveProperty<int>(9999),
                _gem = new ReactiveProperty<int>(0)
            };

            InitializePlayer(managerInfo, "manager", 100, 100, 999999, 999, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 }, "MainFirst", "MainSecond");

        }
       
        public void InitializePlayer(ChaInfoOther playerInfo, string username, int hp, int mp, int money, int level, float[] position, float[] rotation, string currentStory, string nextStory)
        {
            myCharacter._hp.Value = hp;
            myCharacter._mp.Value = mp;
            myCharacter._money.Value = money;
            myCharacter._level.Value = level;
            myCharacterOther = playerInfo;
            _username = username;
            characterPersonalinfo.chaPosition = position;
            characterPersonalinfo.chaRotation = rotation;
            characterPersonalinfo.currentstory_name = currentStory;
            characterPersonalinfo.nextstory_name = nextStory;
        }
        
       
        public void SaveData()
        {
            characterRepo.SaveCharacterData(this);
        }

        public void LoadData()
        {
            var loadedData = characterRepo.LoadCharacterData();
            if (loadedData != null)
            {
                myCharacter = loadedData.myCharacter;
                myCharacterOther = loadedData.myCharacterOther;
                characterPersonalinfo = loadedData.characterPersonalinfo;
                _username = loadedData._username;
            }
        }
        public void SettingMainStory(string currentStory, string nextStory, string nextnpcid)
        {
            characterPersonalinfo.currentstory_name = currentStory;
            characterPersonalinfo.nextstory_name = nextStory;
            characterPersonalinfo.nextstory_npc_id = nextnpcid;
        }


        public void GetMoneyReward(int reward)
        {
            myCharacter._money.Value += reward;
        }
    }



}
