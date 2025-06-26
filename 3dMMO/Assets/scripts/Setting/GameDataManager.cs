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
    public List<object> position;
    public List<object> rotation;
}
public class RawChaInfoOther
{
    public int _attack;
    public int _defense;
    public int _critical;
    public int _speed;
    public int _luck;
    public int _gem;
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
        SortAndSaveData();
    }
    public void GetLastSignal()
    {
        Debug.Log("🚀 게임 종료 감지됨! 데이터 저장 중...");
        StartCoroutine(SaveData());
    }
    private void SortAndSaveData()
    {

        // 데이터 정리
        SortData();

        // 데이터 저장
        SaveGameData();


        //yield return new WaitForSeconds(0.1f);
    }
    private IEnumerator SaveData()
    {
        // 서버로 데이터 전송
        yield return StartCoroutine(SendCharacterData());
        yield return new WaitForEndOfFrame();
        NetworkManager.instance.Shutdown();

        //yield return new WaitForSeconds(0.1f);
    }
    private void SaveGameData()
    {
        // 로컬 파일 저장 (필요 시 확장 가능)
        Debug.Log("게임 데이터가 성공적으로 저장되었습니다.");
    }
   
    private void SortData()
    {
        var myPlayer = PlayerManager.Instance.GetMyCharacterData();
        //myPlayer.playerObj.GetComponent<Player>().SavePositionRotation();
        chaData.character_id = myPlayer.characterPersonalinfo.charater_id;
        chaData.hp = myPlayer.myCharacter.GetHp();
        chaData.mp = myPlayer.myCharacter.GetMp();
        chaData.money = myPlayer.myCharacter.GetMoney();
        chaData.level = myPlayer.myCharacter.GetLevel();
        chaData.storynum = myPlayer.characterPersonalinfo.storyNum;
        var raw = new RawChaInfoOther
        {
            _attack = myPlayer.myCharacterOther.GetAttack(),
            _defense = myPlayer.myCharacterOther.GetDefense(),
            _critical = myPlayer.myCharacterOther.GetCritical(),
            _speed = myPlayer.myCharacterOther.GetSpeed(),
            _luck = myPlayer.myCharacterOther.GetLuck(),
            _gem = myPlayer.myCharacterOther.GetGem()
        };
        chaData.attributes = JsonConvert.SerializeObject(raw);
        float[] pos = myPlayer.characterPersonalinfo.chaPosition;
        float[] rot = myPlayer.characterPersonalinfo.chaRotation;
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
            Debug.Log("캐릭터 데이터 서버에 저장 완료!");
        }
        else
        {
            Debug.LogError($"캐릭터 데이터 저장 실패: {request.error}");
        }
    }
}
