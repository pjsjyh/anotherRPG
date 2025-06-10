using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic; 
using Newtonsoft.Json;
using CharacterInfo;
using System.Linq;
using Questsetting;
using Newtonsoft.Json.Linq;
using UniRx;
using System;

namespace SettingAccountManager
{
    //게임 로그인시 계정 셋팅.
    //http통신을 이용한 캐릭터 정보 셋팅.
    public class RawChaInfoOther
    {
        public int _attack;
        public int _defense;
        public int _critical;
        public int _speed;
        public int _luck;
        public int _gem;
    }
    public class SettingAccount
    {
        //처음 캐릭터 데이터가 들어왔을 때 데이터 가져와서 셋팅.
        public static async Task DoSettingAccount(string responseBody)
        {
            await Task.Yield();

            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

            var playerInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse["playerinfo"].ToString());

            var attributesJson = playerInfo["attributes"].ToString();




            ChaInfoOther characterData;

            if (string.IsNullOrWhiteSpace(attributesJson))
            {
                // 기본값
                characterData = new ChaInfoOther
                {
                    _attack = new ReactiveProperty<int>(1),
                    _defense = new ReactiveProperty<int>(1),
                    _critical = new ReactiveProperty<int>(1),
                    _speed = new ReactiveProperty<int>(1),
                    _luck = new ReactiveProperty<int>(1),
                    _gem = new ReactiveProperty<int>(0)
                };
            }
            else
            {
                if (attributesJson.StartsWith("\"") && attributesJson.EndsWith("\""))
                {
                    attributesJson = JsonConvert.DeserializeObject<string>(attributesJson);
                }

                // RawChaInfoOther로 먼저 역직렬화
                var raw = JsonConvert.DeserializeObject<RawChaInfoOther>(attributesJson);

                characterData = new ChaInfoOther
                {
                    _attack = new ReactiveProperty<int>(raw._attack),
                    _defense = new ReactiveProperty<int>(raw._defense),
                    _critical = new ReactiveProperty<int>(raw._critical),
                    _speed = new ReactiveProperty<int>(raw._speed),
                    _luck = new ReactiveProperty<int>(raw._luck),
                    _gem = new ReactiveProperty<int>(raw._gem)
                };
            }

            var loginData = new CharacterManager();

            var playerHP = int.Parse(playerInfo["HP"].ToString());
            var playerMp = int.Parse(playerInfo["MP"].ToString());
            var playerMoney = int.Parse(playerInfo["Money"].ToString());
            var playerLevel = int.Parse(playerInfo["Level"].ToString());
            var playerName = playerInfo["Username"].ToString();
            var playerPosition = playerInfo["Position"];
            var playerRotation = playerInfo["Rotation"];
            var playerCurrentStory = playerInfo["CurrentStory"].ToString();
            var playerNextStory = playerInfo["NextStory"].ToString();
            float[] positionArray= { 0,0,0}, rotationArray= { 0,0,0};
            if (playerPosition is JArray jArray)
            {
                positionArray = jArray.ToObject<float[]>(); 
            }
            if (playerRotation is JArray jArray2)
            {
                rotationArray = jArray2.ToObject<float[]>();
            }
            float storyNum;
            bool isParsed = float.TryParse(playerInfo["Storynum"].ToString(), out storyNum);
            var charaterID = jsonResponse["charaterID"].ToString();

            var myPlayerSetting = GameManager.Instance.myDataSetting;

            if (isParsed)
            {
                myPlayerSetting.characterPersonalinfo.storyNum = storyNum;
            }
            myPlayerSetting.InitializePlayer(characterData, playerName, playerHP, playerMp, playerMoney, playerLevel, positionArray, rotationArray, playerCurrentStory, playerNextStory);
            myPlayerSetting.SaveData();

            loginData.characterPersonalinfo = new CharacterPersonalInfo
            {
                charater_id = charaterID,
                storyNum = storyNum,
                chaPosition = positionArray,
                chaRotation = rotationArray,
                currentstory_name = playerCurrentStory,
                nextstory_name = playerNextStory
            };
            // 나머지 기본 정보
            loginData._username = playerName;
            loginData.myCharacterOther = characterData; // 필요에 따라 채워넣기
            loginData.myCharacter._hp.Value = playerHP;
            loginData.myCharacter._mp.Value = playerMp;
            loginData.myCharacter._level.Value = playerLevel;
            loginData.myCharacter._money.Value = playerMoney;

            // 전역 저장
            LoginResultData.LocalCharacterData = loginData;
            Debug.Log("셋팅 끝");
            await Task.Yield();
            await Task.CompletedTask;
        }
    }
    
}