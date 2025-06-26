using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
namespace CharacterInfo
{
    public class Stat
    {
        public ReactiveProperty<int> Value { get; private set; }

        public Stat(int initial)
        {
            Value = new(initial);
        }
        public int Get() => Value.Value;
        public void Set(int value) => Value.Value = value;
        public void Add(int amount) => Value.Value += amount;
        public void Sub(int amount) => Value.Value = Mathf.Max(0, Value.Value - amount);
        public bool IsZero() => Value.Value <= 0;
    }
    public class ChaInfo
    {
        public Stat _hp = new(100);
        public Stat _mp = new(50);
        public Stat _money = new(1000);
        public Stat _level = new(1);

        public void SetHp(int value) => _hp.Set(value);
        public void SetMp(int value) => _mp.Set(value);
        public void SetMoney(int value) => _money.Set(value);
        public void SetLevel(int value) => _level.Set(value);

        public int GetHp() => _hp.Get();
        public int GetMp() => _mp.Get();
        public int GetMoney() => _money.Get();
        public int GetLevel() => _level.Get();
        public bool IsDead() => _hp.IsZero();
    }

    public class ChaInfoOther
    {
        public Stat _attack = new(10);
        public Stat _defense = new(5);
        public Stat _critical = new(1);
        public Stat _speed = new(3);
        public Stat _luck = new(2);
        public Stat _gem = new(0);

        public void SetAttack(int value) => _attack.Set(value);
        public void SetDefense(int value) => _defense.Set(value);
        public void SetCritical(int value) => _critical.Set(value);
        public void SetSpeed(int value) => _speed.Set(value);
        public void SetLuck(int value) => _luck.Set(value);
        public void SetGem(int value) => _gem.Set(value);

        public int GetAttack() => _attack.Get();
        public int GetDefense() => _defense.Get();
        public int GetCritical() => _critical.Get();
        public int GetSpeed() => _speed.Get();
        public int GetLuck() => _luck.Get();
        public int GetGem() => _gem.Get();
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
                _attack = new Stat(9999),
                _defense = new Stat(9999),
                _critical = new Stat(9999),
                _speed = new Stat(100),
                _luck = new Stat(9999),
                _gem = new Stat(0)
            };

            InitializePlayer(managerInfo, "manager", 100, 100, 999999, 999, new float[] { 0, 0, 0 }, new float[] { 0, 0, 0 }, "MainFirst", "MainSecond");

        }
       
        public void InitializePlayer(ChaInfoOther playerInfo, string username, int hp, int mp, int money, int level, float[] position, float[] rotation, string currentStory, string nextStory)
        {
            myCharacter._hp.Set(hp);
            myCharacter._mp.Set(mp);
            myCharacter._money.Set(money);
            myCharacter._level.Set(level);
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
            myCharacter._money.Add(reward);
        }

        public void TakeDamage(int damage)
        {
            myCharacter._hp.Sub(damage);
        }
        public int GetHp()
        {
            return myCharacter._hp.Get();
        }

    }



}
