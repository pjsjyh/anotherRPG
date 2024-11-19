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
            
            // 다시 JSON으로 파싱
            Debug.Log(characterJson);
            ChaInfoOther characterData;
            if (string.IsNullOrWhiteSpace(characterJson))
            {
                // 기본값으로 초기화
                characterData = new ChaInfoOther
                {
                    _attack = 1,
                    _defense = 1,
                    _critical = 1,
                    _speed = 1,
                    _luck = 1
                    // 다른 필드들도 기본값으로 설정
                };
            }
            else
            {
                // characterJson이 비어있지 않으면 JSON 데이터를 파싱
                characterData = JsonConvert.DeserializeObject<ChaInfoOther>(characterJson);
            }

            var playerHP = int.Parse(playerInfo["HP"].ToString());
            var playerMp = int.Parse(playerInfo["MP"].ToString());
            var playerMoney = int.Parse(playerInfo["Money"].ToString());
            var playerLevel = int.Parse(playerInfo["Level"].ToString());
            var playerName = playerInfo["Username"].ToString();
            CharacterManager.Instance.InitializePlayer(characterData, playerName, playerHP, playerMp, playerMoney, playerLevel);




            await Task.CompletedTask;
        }
    }

}