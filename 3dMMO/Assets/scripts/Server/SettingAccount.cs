using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic; //dictionary
using Newtonsoft.Json;
using ApiUtilities;
using CharacterInfo;

namespace SettingAccountManager
{
    public class SettingAccount
    {
        public static async Task DoSettingAccount(string responseBody)
        {
            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

            var playerInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse["playerinfo"].ToString());

            var characterJson = playerInfo["character"].ToString();
            CharacterManager.Instance.characterPersonalinfo.charater_id = jsonResponse["charaterID"].ToString();
          
            // 다시 JSON으로 파싱
            Debug.Log(jsonResponse);
            ChaInfoOther characterData =  string.IsNullOrWhiteSpace(characterJson)
                ? new ChaInfoOther { _attack = 1, _defense = 1, _critical = 1, _speed = 1, _luck = 1 }
                : JsonConvert.DeserializeObject<ChaInfoOther>(characterJson);
          

            var playerHP = int.Parse(playerInfo["HP"].ToString());
            var playerMp = int.Parse(playerInfo["MP"].ToString());
            var playerMoney = int.Parse(playerInfo["Money"].ToString());
            var playerLevel = int.Parse(playerInfo["Level"].ToString());
            var playerName = playerInfo["Username"].ToString();
            int storyNum;
            bool isParsed = int.TryParse(playerInfo["Storynum"].ToString(), out storyNum);

            if (isParsed)
            {
                CharacterManager.Instance.characterPersonalinfo.storyNum = storyNum;
            }
            CharacterManager.Instance.InitializePlayer(characterData, playerName, playerHP, playerMp, playerMoney, playerLevel);


            CharacterManager.Instance.SaveData();

            await Task.CompletedTask;
        }
    }

}