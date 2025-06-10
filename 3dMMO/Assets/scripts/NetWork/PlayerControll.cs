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
//플레이어 개인 정보 저장되어있는 스크립트
public class PlayerControll : NetworkBehaviour
{
    [SerializeField] private Player playerLogic;
    public CharacterManager myData; //개인데이터
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
    public override async void Spawned() //캐릭터 생성 후 정보 이어주기
    {
        Debug.Log("▶ Spawned: " + Object.InputAuthority);
        await PlayerManager.Instance.RegisterPlayer(Object.InputAuthority, Object);

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
    private void InitMyCharacter() //내 캐릭터 init하기
    {
        myData.playerObj = gameObject;
        StartCoroutine(SettingDataStart());
        RPC_SetName(myData._username);
    }

    private void InitOtherCharacter() //다른 캐릭터 init하기
    {
        var nameUI = transform.Find("name/NameText");
        if (nameUI != null)
        {
            nameUI.GetComponent<TextMeshProUGUI>().text = this.name;
        }
        if (Runner.IsServer && ServerPlayerDataStore.AllPlayerData.TryGetValue(Object.InputAuthority, out var data))
        {
            RPC_SetCharacterData(
                data.myCharacter._hp.Value, data.myCharacter._mp.Value, data.myCharacter._money.Value, data.myCharacter._level.Value,
                data.myCharacterOther._attack.Value, data.myCharacterOther._defense.Value, data.myCharacterOther._critical.Value,
                data.myCharacterOther._speed.Value, data.myCharacterOther._luck.Value, data.myCharacterOther._gem.Value,
                data._username
            );
            Debug.Log(data._username);
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
        ServerPlayerDataStore.AllPlayerData[myPlayerRef] = LoginResultData.LocalCharacterData;

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
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetCharacterData(
    int hp, int mp, int money, int level,
    int atk, int def, int cri, int spd, int luck, int gem,
    string username)
    {
        Debug.Log($"[RPC] 다른 플레이어 캐릭터 세팅됨: {username}");
        if (!Object.HasInputAuthority)
        {
            myData = new CharacterManager();
            myData.myCharacter._hp.Value = hp;
            myData.myCharacter._mp.Value = mp;
            myData.myCharacter._money.Value = money;
            myData.myCharacter._level.Value = level;

            myData.myCharacterOther._attack.Value = atk;
            myData.myCharacterOther._defense.Value = def;
            myData.myCharacterOther._critical.Value = cri;
            myData.myCharacterOther._speed.Value = spd;
            myData.myCharacterOther._luck.Value = luck;
            myData.myCharacterOther._gem.Value = gem;

            myData._username = username;
            this.name = username;

            var nameUI = transform.Find("name/NameText");
            if (nameUI != null)
                nameUI.GetComponent<TextMeshProUGUI>().text = username;
        }
    }
    public void StartMoveToTargetNav(Vector3 getTarget)
    {
        playerLogic.GoTarget(getTarget);
    }
}
