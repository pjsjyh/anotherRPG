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

            CharacterManager.Instance.characterPersonalinfo.charater_id = jsonResponse["charaterID"].ToString();



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
            if (playerInfo.ContainsKey("getQuest") && playerInfo["getQuest"] != null)
            {
                try
                {
                    List<QuestInfo> questInfoList = JsonConvert.DeserializeObject<List<QuestInfo>>(playerInfo["getQuest"].ToString());

                    if (questInfoList != null && questInfoList.Count > 0)
                    {
                        //변환 전 JSON 확인
                        string getQuestJson = JsonConvert.SerializeObject(questInfoList);

                        //List<QuestInfo>에서 List<QuestGet> 추출
                        List<QuestGet> quests = questInfoList.Select(qi => qi.questget).ToList();

                        //변환 성공 여부 확인
                        if (quests != null && quests.Count > 0)
                        {
                            if (QuestManager.Instance == null)
                            {
                                Debug.LogError("❌ QuestUISetting.Instance가 NULL입니다!");
                                return;
                            }
                            else
                            {
                                QuestManager.Instance.questSet.Clear();
                                QuestManager.Instance.questInfo.Clear();
                            }
                           
                            QuestManager.Instance.questSet.AddRange(quests);
                            QuestManager.Instance.questInfo.AddRange(questInfoList);

                        }
                        else
                        {
                            Debug.LogError("❌ 변환된 퀘스트 리스트가 NULL이거나 비어 있음.");
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ 퀘스트 JSON 파싱 오류: {e.Message}");
                }
            }
            else
            {
                Debug.Log("⚠️ 퀘스트 데이터가 비어 있음.");
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


            if (isParsed)
            {
                CharacterManager.Instance.characterPersonalinfo.storyNum = storyNum;
            }
            CharacterManager.Instance.InitializePlayer(characterData, playerName, playerHP, playerMp, playerMoney, playerLevel, positionArray, rotationArray);
            CharacterManager.Instance.SaveData();

            await Task.CompletedTask;
        }
    }

}