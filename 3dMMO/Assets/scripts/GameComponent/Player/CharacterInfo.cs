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

    public struct CharacterPersonalInfo
    {
        public string charater_id;
        public float storyNum;
    }
    public class CharacterManager
    {
        private static CharacterManager instance;
        private static CharacterRepository characterRepo = new CharacterRepository();
        public ChaInfo myCharacter;
        public ChaInfoOther myCharacterOther;
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
        
       
        public void SaveData()
        {
            characterRepo.SaveCharacterData(this);
        }

        // ✅ 캐릭터 데이터 불러오기
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
