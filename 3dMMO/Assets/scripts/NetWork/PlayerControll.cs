using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using CharacterInfo;
using System;
using SettingAccountManager;
using TMPro;

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
        CharacterManager ch = GameManager.Instance.myDataSetting;

        if (myData != null && ch != null)
        {
            var mouseMove = Camera.main.transform.parent.GetComponent<MouseMove>();
            var charInfo = ch.characterPersonalinfo;
            mouseMove.transform.position = new Vector3(charInfo.chaPosition[0], charInfo.chaPosition[1], charInfo.chaPosition[2]);
           
            myData.myCharacter = ch.myCharacter;
            myData.myCharacterOther = ch.myCharacterOther;
            myData.characterPersonalinfo = ch.characterPersonalinfo;
            myData._username = ch._username;
            this.name = ch._username;
            var nameUI = transform.Find("name/NameText");
            if (nameUI != null)
                nameUI.GetComponent<TextMeshProUGUI>().text = ch._username;
        }
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
