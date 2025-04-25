using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic; 
using Newtonsoft.Json;
using CharacterInfo;
using System.Linq;
using Questsetting;
using Newtonsoft.Json.Linq;

namespace SettingAccountManager
{
    //게임 로그인시 계정 셋팅.
    //http통신을 이용한 캐릭터 정보 셋팅.
    public class SettingAccount
    {
        public static async Task DoSettingAccount(string responseBody)
        {
            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

            var playerInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse["playerinfo"].ToString());

            var attributesJson = playerInfo["attributes"].ToString();




            ChaInfoOther characterData;
            if (string.IsNullOrWhiteSpace(attributesJson))
            {
                characterData = new ChaInfoOther { _attack = 1, _defense = 1, _critical = 1, _speed = 1, _luck = 1 };
            }
            else
            {
                //JSON이 이중으로 감싸져 있는 경우 해결
                if (attributesJson.StartsWith("\"") && attributesJson.EndsWith("\""))
                {
                    attributesJson = JsonConvert.DeserializeObject<string>(attributesJson);
                }

                characterData = JsonConvert.DeserializeObject<ChaInfoOther>(attributesJson);
            }
            

            var playerHP = int.Parse(playerInfo["HP"].ToString());
            var playerMp = int.Parse(playerInfo["MP"].ToString());
            var playerMoney = int.Parse(playerInfo["Money"].ToString());
            var playerLevel = int.Parse(playerInfo["Level"].ToString());
            var playerName = playerInfo["Username"].ToString();
            var playerPosition = playerInfo["Position"];
            var playerRotation = playerInfo["Rotation"];
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
            var myPlayerSetting = GameManager.Instance.myDataSetting;
            myPlayerSetting.characterPersonalinfo.charater_id = jsonResponse["charaterID"].ToString();
            if (isParsed)
            {
                myPlayerSetting.characterPersonalinfo.storyNum = storyNum;
            }
            myPlayerSetting.InitializePlayer(characterData, playerName, playerHP, playerMp, playerMoney, playerLevel, positionArray, rotationArray);
            myPlayerSetting.SaveData();
           await Task.CompletedTask;
        }
    }
    
}