using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using CharacterInfo;
using System.IO;
public class CharacterRepository : MonoBehaviour
{
    private string saveFilePath => Application.persistentDataPath + "/characterData.json";

    public void SaveCharacterData(CharacterManager characterManager)
    {
        try
        {
            string json = JsonConvert.SerializeObject(characterManager, Formatting.Indented);
            File.WriteAllText(saveFilePath, json);
            //Debug.Log("✅ 캐릭터 데이터 저장 완료: " + saveFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ 캐릭터 데이터 저장 실패: " + e.Message);
        }
    }

    public CharacterManager LoadCharacterData()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                return JsonConvert.DeserializeObject<CharacterManager>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError("❌ 캐릭터 데이터 불러오기 실패: " + e.Message);
            }
        }
        Debug.LogWarning("⚠ 저장된 캐릭터 데이터가 없음. 새 데이터 생성");
        return null;
    }
}
