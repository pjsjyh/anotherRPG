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
        public float storyNum;
        public float[] chaPosition;
        public float[] chaRotation;
    }
    public class CharacterManager
    {
        private static CharacterManager instance;
        private static CharacterRepository characterRepo = new CharacterRepository();
        public ChaInfo myCharacter;
        public ChaInfoOther myCharacterOther;
        public CharacterPersonalInfo characterPersonalinfo;
        public string _username = "";

        public static CharacterManager Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new CharacterManager();
                }
                return instance;
            }
        }
        private CharacterManager()
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

            InitializePlayer(managerInfo, "manager", 100, 100, 999999, 999, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 });

        }
       
        public void InitializePlayer(ChaInfoOther playerInfo, string username, int hp, int mp, int money, int level, float[] position, float[] rotation)
        {
            myCharacter._hp = hp;
            myCharacter._mp = mp;
            myCharacter._money = money;
            myCharacter._level = level;
            myCharacterOther = playerInfo;
            _username = username;
            characterPersonalinfo.chaPosition = position;
            characterPersonalinfo.chaRotation = rotation;
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
                //questInfo = loadedData.questInfo;
                characterPersonalinfo = loadedData.characterPersonalinfo;
                _username = loadedData._username;
            }
        }
    }



}
