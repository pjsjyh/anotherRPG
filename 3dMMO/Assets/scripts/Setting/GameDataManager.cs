using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterInfo;
using Newtonsoft.Json;

using UnityEngine.Networking;
using MyServerManager;
using ApiUtilities;
using System;

public class CharacterData
{
    public string character_id;
    public int hp;
    public int mp;
    public int money;
    public int level;
    public float storynum;
    public string player_id;
    public string attributes;  // JSON 형식의 문자열
    public string get_quest;   // JSON 형식의 문자열
    public List<object> position;
    public List<object> rotation;
}

public class GameDataManager : MonoBehaviour
{
    private static GameDataManager instance;
    public static GameDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("GameDataManager");
                instance = obj.AddComponent<GameDataManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private CharacterData chaData = new CharacterData();

    public void GetSignal()
    {
        Debug.Log("🚀 게임 종료 감지됨! 데이터 저장 중...");
        StartCoroutine(SortAndSaveData());
    }

    private IEnumerator SortAndSaveData()
    {
        GameObject.Find("Character").GetComponent<Player>().SavePositionRotation();

        // 데이터 정리
        SortData();

        // 데이터 저장
        SaveGameData();

        // 서버로 데이터 전송
         yield return StartCoroutine(SendCharacterData());
       //yield return new WaitForSeconds(0.1f);
    }

    private void SaveGameData()
    {
        // 로컬 파일 저장 (필요 시 확장 가능)
        Debug.Log("✅ 게임 데이터가 성공적으로 저장되었습니다.");
    }
    public float[] GetFloatArray(object obj)
    {
        Debug.Log(obj);
        //  이미 float[]이면 그대로 반환
        if (obj is float[])
            return (float[])obj;

        // object[] 배열로 변환 가능하면 float[]으로 변환
        if (obj is object[] objArray)
        {
            try
            {
                return Array.ConvertAll(objArray, x => Convert.ToSingle(x));
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ 변환 실패 (object[]): {e.Message}");
            }
        }

        // List<object> 형태라면 float[] 변환 시도
        if (obj is List<object> list)
        {
            try
            {
                return list.ConvertAll(x => Convert.ToSingle(x)).ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ 변환 실패 (List<object>): {e.Message}");
            }
        }

        // JSON에서 오는 경우 string일 가능성이 있음
        if (obj is string jsonString)
        {
            try
            {
                return JsonUtility.FromJson<float[]>(jsonString);
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ 변환 실패 (string JSON): {e.Message}");
            }
        }

        Debug.LogError("❌ 변환 실패: 올바른 배열 형식이 아님!");
        return new float[] { 0, 0, 0 };
    }

    private void SortData()
    {
        chaData.character_id = CharacterManager.Instance.characterPersonalinfo.charater_id;
        chaData.hp = CharacterManager.Instance.myCharacter._hp;
        chaData.mp = CharacterManager.Instance.myCharacter._mp;
        chaData.money = CharacterManager.Instance.myCharacter._money;
        chaData.level = CharacterManager.Instance.myCharacter._level;
        chaData.storynum = CharacterManager.Instance.characterPersonalinfo.storyNum;
        chaData.attributes = JsonConvert.SerializeObject(CharacterManager.Instance.myCharacterOther);
        chaData.get_quest = JsonConvert.SerializeObject(QuestManager.Instance.questInfo);
        float[] pos = CharacterManager.Instance.characterPersonalinfo.chaPosition;
        float[] rot = CharacterManager.Instance.characterPersonalinfo.chaRotation;
        chaData.position = new List<object> { pos[0], pos[1], pos[2] };
        chaData.rotation = new List<object> { rot[0], rot[1], rot[2] };
    }

    private IEnumerator SendCharacterData()
    {
        string url = ApiUrls.SaveData;
        string jsonData = JsonConvert.SerializeObject(chaData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ 캐릭터 데이터 서버에 저장 완료!");
        }
        else
        {
            Debug.LogError($"❌ 캐릭터 데이터 저장 실패: {request.error}");
        }
    }
}
