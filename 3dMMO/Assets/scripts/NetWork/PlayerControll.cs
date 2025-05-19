using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using CharacterInfo;
using System;
using SettingAccountManager;
using TMPro;
public static class LoginResultData
{
    public static CharacterManager LocalCharacterData;
}
public class PlayerControll : NetworkBehaviour
{
    [SerializeField] private Player playerLogic;
    public CharacterManager myData;
    public PlayerRef myPlayerRef;
   
    public void Awake()
    {
            myData = new CharacterManager();
    }
    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            playerLogic.CustomUpdate();
        }
    }
    public override void Spawned()
    {
        Debug.Log("▶ Spawned: " + Object.InputAuthority);
        PlayerManager.Instance.RegisterPlayer(Object.InputAuthority, Object);
        myPlayerRef = Object.InputAuthority;
        if (Object.HasInputAuthority)
        {
            Debug.Log("내 캐릭터 생성");
            InitMyCharacter();
            var mouseMove = Camera.main.transform.parent.GetComponent<MouseMove>();

            if (mouseMove)
                mouseMove.InitializeCamera(this.transform, GetComponent<Player>());
        }
        else
        {
            InitOtherCharacter();
        }
        GameManager.Instance.PlayerDataReady();
    }
    private void InitMyCharacter()
    {
        myData.playerObj = gameObject;
       StartCoroutine(SettingDataStart());
        RPC_SetName(myData._username);
    }

    private void InitOtherCharacter()
    {
        var nameUI = transform.Find("name/NameText");
        if (nameUI != null)
        {
            nameUI.GetComponent<TextMeshProUGUI>().text = this.name;
        }
    }
    public void OnObjectSpawned(NetworkRunner runner, NetworkObject obj)
    {
        var playerCtrl = obj.GetComponent<PlayerControll>();
        if (playerCtrl != null && !PlayerManager.Instance.GetPlayerObject(obj.InputAuthority))
        {
            PlayerManager.Instance.RegisterPlayer(obj.InputAuthority, obj);
        }
    }
    IEnumerator SettingDataStart()
    {

        var data = LoginResultData.LocalCharacterData;
        if (myData != null && data != null)
        {
            var mouseMove = Camera.main.transform.parent.GetComponent<MouseMove>();
            var charInfo = data.characterPersonalinfo;
            mouseMove.transform.position = new Vector3(charInfo.chaPosition[0], charInfo.chaPosition[1], charInfo.chaPosition[2]);
           
            myData.myCharacter = data.myCharacter;
            myData.myCharacterOther = data.myCharacterOther;
            myData.characterPersonalinfo = data.characterPersonalinfo;
            myData._username = data._username;
            this.name = data._username;
            var nameUI = transform.Find("name/NameText");
            if (nameUI != null)
                nameUI.GetComponent<TextMeshProUGUI>().text = data._username;
        }
        myData = data;
        Debug.Log("돌아가 데이터 세팅 완료");
        yield return null;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetName(string playerName)
    {
        Debug.Log($"[RPC] 이름 설정: {playerName}");
        this.name = playerName;
        var nameUI = transform.Find("name/NameText");
        if (nameUI != null)
            nameUI.GetComponent<TextMeshProUGUI>().text = playerName;
    }
}
