using System.Collections.Generic;
using CharacterInfo;
using Fusion;
using UnityEngine;
using System.Threading.Tasks;
public static class ServerPlayerDataStore
{
    public static Dictionary<PlayerRef, CharacterManager> AllPlayerData = new();
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private Dictionary<PlayerRef, NetworkObject> playerObjects = new Dictionary<PlayerRef, NetworkObject>();

    public NetworkRunner Runner { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void SetRunner(NetworkRunner runner)
    {
        Runner = runner;
    }

    public async Task RegisterPlayer(PlayerRef playerRef, NetworkObject netObj)
    {
        if (!playerObjects.ContainsKey(playerRef))
        {
            Debug.Log("RegisterPlayer: " + playerRef);
            playerObjects[playerRef] = netObj;
            await Task.Yield();
        }
    }
    public CharacterManager GetCharacterData(PlayerRef player)
    {
        if (playerObjects.TryGetValue(player, out var netObj))
        {
            var controll = netObj.GetComponent<PlayerControll>();
            if (controll != null)
            {
                return controll.myData;
            }
            Debug.LogWarning("컨트롤러 없음: " + netObj.name);
        }
        else
        {
            Debug.LogWarning("해당 플레이어 오브젝트 없음: " + player);
        }

        return null;
    }


    public void UnregisterPlayer(PlayerRef playerRef)
    {
        if (playerObjects.ContainsKey(playerRef))
        {
            playerObjects.Remove(playerRef);
        }
    }

    public NetworkObject GetPlayerObject(PlayerRef playerRef)
    {
        playerObjects.TryGetValue(playerRef, out var obj);
        return obj;
    }

    public NetworkObject GetMyPlayerObject()
    {


        if (Runner == null)
        {
            Debug.LogWarning("Runner가 아직 설정되지 않았습니다.");
            return null;
        }


        return PlayerManager.Instance.GetPlayerObject(Runner.LocalPlayer);
    }
    public PlayerControll GetMyPlayerControll()
    {
        var obj = GetMyPlayerObject();
        return obj != null ? obj.GetComponent<PlayerControll>() : null;
    }
    public CharacterManager GetMyCharacterData()
    {
        var controller = GetMyPlayerControll();
        return controller != null ? controller.myData : null;
    }
}
