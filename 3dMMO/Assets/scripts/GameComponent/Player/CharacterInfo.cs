using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterInfo
{
    public class ChaInfo
    {
        public int _hp;
        public int _mp;
        public int _money;
        public int _level;

    };
    public class ChaInfoOther
    {
        public int _attack;
        public int _defense;
        public int _critical;
        public int _speed;
        public int _luck;
        public int _gem;
    }

    public class CharacterPersonalInfo
    {
        public string charater_id;
        public string nextstory_name = null;
        public string currentstory_name = null;
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
                _attack = 9999,
                _defense = 9999,
                _critical = 9999,
                _speed = 100,
                _luck = 9999,
                _gem = 0
            };

            InitializePlayer(managerInfo, "manager", 100, 100, 999999, 999, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 }, "MainFirst", "MainSecond");

        }
       
        public void InitializePlayer(ChaInfoOther playerInfo, string username, int hp, int mp, int money, int level, float[] position, float[] rotation, string currentStory, string nextStory)
        {
            myCharacter._hp = hp;
            myCharacter._mp = mp;
            myCharacter._money = money;
            myCharacter._level = level;
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
        public void SettingMainStory(string currentStory, string nextStory)
        {
            characterPersonalinfo.currentstory_name = currentStory;
            characterPersonalinfo.nextstory_name = nextStory;
        }
    }



}
