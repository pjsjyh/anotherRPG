using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using System.Threading;
using System.Reflection;
using System.Collections;
using CharacterInfo;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{

    public NetworkRunner networkRunnerPrefab; // 네트워크 런너 프리팹
    public static NetworkManager instance;
    private NetworkRunner networkRunner;
    public NetworkPrefabRef playerPrefabRef;
    public GameObject playerPrefab;
    private List<PlayerRef> spawnQueue = new List<PlayerRef>();
    bool spawnReady = false;
    private void Awake()
    {
        var existingManager = FindObjectOfType<NetworkManager>();
        if (existingManager != null && existingManager != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        networkRunner = GetComponent<NetworkRunner>();
        PlayerManager.Instance.SetRunner(networkRunner);

        instance = this;
     }

    private async void Start()
    {
        await StartNetwork();

    }
    void OnDestroy()
    {
        if (networkRunner == null)
        {
            Debug.LogError("⚠ NetworkRunner가 이미 삭제된 상태입니다!");
        }
        else
        {
            DontDestroyOnLoad(networkRunner.gameObject);
            Debug.Log($"⚠ NetworkRunner 상태 확인: {networkRunner.State}");
        }
    }
    public async Task StartNetwork()
    {
        PhotonAppSettings settings = Resources.Load<PhotonAppSettings>("PhotonAppSettings");
        if (settings == null)
        {
            Debug.LogError("❌ Photon Fusion App ID가 설정되지 않았습니다!");
            return;
        }

        // NetworkProjectConfig 확인
        var config = NetworkProjectConfig.Global;
        if (config == null)
        {
            Debug.LogError("❌ NetworkProjectConfig가 올바르게 로드되지 않았습니다! Fusion 설정을 확인하세요.");
            return;
        }
        var result = await networkRunner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok)
        {
            Debug.Log($"✅ 로비 입장 성공, 세션 목록 기다리는 중... {result.ShutdownReason}");
            // 세션 목록은 OnSessionListUpdated에서 처리됨
        }
        else
        {
            Debug.LogError("❌ 로비 입장 실패");
        }

    }
    public async void StartGame(GameMode mode)
    {
        Debug.Log(mode);
        try
        {
            // 세션 이름 설정
            string sessionName = "TestRoom";
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneRef sceneRef = SceneRef.FromIndex(2);
            if (mode == GameMode.Host || mode == GameMode.Server)
                networkRunner.ProvideInput = false;
            else
                networkRunner.ProvideInput = true;

            if (networkRunner.GetComponent<NetworkSceneManagerDefault>() == null)
            {
                networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
            networkRunner.AddCallbacks(this);

            var result = await networkRunner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = sessionName,
                SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>(),
                PlayerCount = 10,
                Scene = sceneRef,
            });

            if (result.Ok)
            {
                Debug.Log($"✅ 네트워크 시작 성공! 세션 이름: {sessionName}");
                
                //var parameters = new NetworkLoadSceneParameters()
                //{
                //    // 기존 씬들을 언로드하고 새 씬만 남기도록 설정
                    
                //};

                //if (!sceneRef.IsValid)
                //{
                //    Debug.LogError("🚫 유효하지 않은 씬 경로입니다!");
                //}
                //else
                //{
                //    await networkRunner.SceneManager.LoadScene(sceneRef, new NetworkLoadSceneParameters());
                //}

            }
            else
            {
                Debug.LogError($"❌ StartGame() 실패: {result.ShutdownReason}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌StartGame() 실패: {e.Message}");
        }
    }

    public IEnumerator StratSpawn()
    {

        Debug.Log("🎯 씬 로딩 완료됨 → 캐릭터 생성 시작");
        
        float timeout = 5f;
        while (!spawnReady && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }
        if (networkRunner.IsServer)
        {
            //StartCoroutine(SpawnPlayerCoroutine());
        }
        else
        {
            Debug.Log("!");
            //StartCoroutine(SpawnPlayerCoroutineClient());

        }
    }

    private IEnumerator SpawnPlayerCoroutineClient()
    {

        Debug.Log("생성하러들어옴");
        yield return new WaitForSeconds(0.2f);
        foreach (var player in networkRunner.ActivePlayers)
        {
            Debug.Log($"[PlayerRef] {player}");
        }

        Debug.Log(networkRunner.LocalPlayer);

        float timeout = 10f;
        while (timeout > 0f)
        {
            if (networkRunner.TryGetPlayerObject(networkRunner.LocalPlayer, out var playerObject) && playerObject != null)
            {
                Debug.Log("✅ 클라이언트 → 내 캐릭터 오브젝트 찾음!");

                // 이름 설정 (옵션)
                //playerObject.name = CharacterManager.Instance._username;

                var nameText = playerObject.transform.Find("name/NameText");
                if (nameText != null)
                {
                   // nameText.GetComponent<TextMeshProUGUI>().text = CharacterManager.Instance._username;
                }

                yield break;
            }

            timeout -= Time.deltaTime;
            yield return null;
        }
        Debug.LogWarning($"[클라이언트] 캐릭터 없음. ActivePlayers: {string.Join(", ", networkRunner.ActivePlayers)}");
        Debug.LogWarning("⚠ 클라이언트 → 시간 초과: 캐릭터 오브젝트를 찾지 못했습니다.");

    }
    private IEnumerator SpawnPlayerCoroutine(PlayerRef player)
    {
        yield return new WaitForSeconds(0.2f); // 씬 안정 대기용 (필요 시)
        if (networkRunner == null || !networkRunner.IsRunning)
        {
            Debug.LogError("`networkRunner`가 없거나 실행되지 않았습니다.");
            yield break;
        }


        if (!playerPrefabRef.IsValid)
        {
            Debug.LogError("`playerPrefabRef`가 유효하지 않습니다! NetworkPrefabTable 등록 여부를 확인하세요.");
            yield break;
        }

        // 스폰 위치 지정
        Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
        //var charInfo = CharacterManager.Instance.characterPersonalinfo;
        //spawnPosition = new Vector3(charInfo.chaPosition[0], charInfo.chaPosition[1], charInfo.chaPosition[2]);
        Debug.Log("SpawnAsync 실행 시작");
        networkRunner.ProvideInput = true;


        NetworkObject networkPrefab = playerPrefab.GetComponent<NetworkObject>();

        var prefabAssets = Resources.Load<NetworkPrefabAssets>("NetworkPrefabAssets");

        var loadedPrefab = Resources.Load<NetworkObject>("Character");
        var config = NetworkProjectConfig.Global;
        if (!(networkRunner is NetworkRunner))
        {
            Debug.LogError("`networkRunner`가 `NetworkRunner` 타입이 아님!");
        }
        else
        {
            Debug.Log($"`networkRunner` 타입 확인: {networkRunner.GetType()}");
        }
        if (networkRunner.TryGetPlayerObject(player, out var existingObj) && existingObj != null)
        {
            Debug.Log("이미 플레이어가 존재합니다. TrySpawn 생략");
            yield break;
        }
        Debug.Log(player);
        if (networkRunner.IsServer)
        {
            var spawnTask = networkRunner.SpawnAsync(
   playerPrefabRef,
   spawnPosition,
   Quaternion.identity,
   player
);
            while (!spawnTask.IsSpawned && !spawnTask.IsFailed)
            {

                yield return null;
            }
            if (spawnTask.IsFailed)
            {
                yield break;
            }
            NetworkObject playerObj = null;

         
            // 내 캐릭터인 경우만 데이터 셋팅 트리거
            Debug.Log(player + " " + networkRunner.LocalPlayer);
            if (player == networkRunner.LocalPlayer)
            {
                Debug.Log("콜");
                
            }
            else
            {
                Debug.Log("실행안됨");
            }
            //while (!networkRunner.TryGetPlayerObject(player, out playerObj))
            //{
            //    Debug.Log($"Trying GetPlayerObject for: {player}");

            //    yield return null;
            //}

            //Debug.Log($"✅ [서버 Spawn] {player} 객체 등록됨: {playerObj.name}");

            // PlayerManager 등록
            //PlayerManager.Instance.RegisterPlayer(player, playerObj);

        }
        else
        {
            Debug.LogError("❌ 클라이언트가 Spawn을 시도했습니다! 이건 허용되지 않음");
        }
        try
        {

            
           
             // ✅ `TrySpawn()` 실행
            //            NetworkSpawnStatus status = networkRunner.TrySpawn(
            //                networkPrefab, // ✅ `GameObject` 대신 `NetworkObject` 전달
            //                out NetworkObject spawnedObject,
            //                spawnPosition,
            //                Quaternion.identity,
            //                networkRunner.LocalPlayer // 플레이어 소유권 지정 (선택)
            //            );
            //if (status == NetworkSpawnStatus.Spawned)
            //{
            //    Debug.Log($"`TrySpawn()`을 통해 플레이어 생성 성공! {spawnedObject.name}");

            //}
            //else
            //{
            //    Debug.LogError($"`TrySpawn()` 실패: {status}");
            //}
            //spawnedObject.name = CharacterManager.Instance._username;
            //spawnedObject.transform.Find("name/NameText").GetComponent<TextMeshProUGUI>().text = CharacterManager.Instance._username;


            //if (spawnedObject != null)
            //{
            //    Debug.Log($"플레이어 스폰 성공! {spawnedObject.name}");
            //}
            //else
            //{
            //    Debug.LogError("SpawnAsync는 완료되었지만, 생성된 오브젝트가 null입니다.");
            //}
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SpawnAsync 도중 예외 발생: {e.Message}");
        }



    }


    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogError($"❌ 네트워크가 종료됨: {shutdownReason}");
        string sessionName = runner.SessionInfo.Name;
        if (runner.SessionInfo != null)
        {
            Debug.LogError($"🔹 종료된 세션 이름: {runner.SessionInfo.Name}");
        }
        else
        {
            Debug.LogError("⚠ 종료된 세션 정보를 가져올 수 없습니다! (SessionInfo가 null)");
        }
        switch (shutdownReason)
        {

            case ShutdownReason.GameNotFound:
                Debug.LogError("⚠ 세션을 찾을 수 없습니다. 올바른 세션 이름을 입력하세요.");
                break;

            case ShutdownReason.Ok:
                Debug.LogError("⚠ 클라이언트가 정상적으로 종료되었습니다.");
                break;

            case ShutdownReason.DisconnectedByPluginLogic:
                Debug.LogError("⚠ 서버 플러그인에서 클라이언트를 강제로 연결 해제했습니다.");
                break;
            case ShutdownReason.IncompatibleConfiguration:
                Debug.LogError("⚠ 네트워크 설정이 호환되지 않습니다. 설정 확인 필요.");
                break;

            default:
                Debug.LogError("⚠ 기타 종료 사유: " + shutdownReason);
                break;
        }
    }
    private IEnumerator WaitForMyCharacter(PlayerRef player)
    {
        float timeout = 5f;
        while (timeout > 0f)
        {
            if (networkRunner.TryGetPlayerObject(player, out var obj) && obj != null)
            {
                Debug.Log($"✅ [클라이언트] 플레이어 오브젝트 찾음: {obj.name}");
                yield break;
            }

            timeout -= Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning("⚠ 클라이언트 → 시간 초과: 캐릭터 오브젝트를 찾지 못했습니다.");
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!spawnQueue.Contains(player))
        {
            Debug.Log($"📥 {player} 대기열에 추가");
            spawnQueue.Add(player);
        }
        Debug.Log($"[OnPlayerJoined] 플레이어 연결됨: {player} / LocalPlayer: {runner.LocalPlayer} / IsServer: {runner.IsServer}");

        //if (!runner.IsServer && runner.LocalPlayer == player)
        //{
        //   // StartCoroutine(SpawnPlayerCoroutineClient());
        //}
        if (runner.IsServer )
        {
            Debug.Log($"🛠 [서버] {player}에 대해 Spawn 시도");
            StartCoroutine(SpawnPlayerCoroutine(player));
            //var spawnPos = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));

            //var obj = runner.Spawn(playerPrefabRef, spawnPos, Quaternion.identity, player);
            //if (obj != null)
            //{
            //    Debug.Log($"✅ [서버] {player} 캐릭터 Spawn 완료");
            //    obj.name = $"PlayerObject_{player}";
            //}
            //else
            //{
            //    Debug.LogError($"❌ [서버] {player} Spawn 실패!");
            //}
        }
    }
    public void Shutdown()
    {
        networkRunner.Shutdown();
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"🔄 세션 목록 갱신됨: {sessionList.Count}개");

        foreach (var session in sessionList)
        {
            Debug.Log($" - {session.Name} / {session.PlayerCount}명 접속 중");
        }

        // 세션이 있으면 Client, 없으면 Host 시작
        if (sessionList.Any(s => s.Name == "TestRoom"))
        {
            StartGame(GameMode.Client);
            Debug.Log("🎮 클라이언트는 이미 세션 감지됨, 대기 중");
        }
        else
        {
            StartGame(GameMode.Host);
        }
        spawnReady = true;
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("✅ 씬 로드 완료 → 캐릭터 생성 대기");
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log($"✅ 씬 로드 완료: {currentScene.name} (Index: {currentScene.buildIndex})");
        if (currentScene.name == "GameScene")
        {
            // 이전 씬(LoadingScene)이 남아 있으면 언로드
            var loadingScene = SceneManager.GetSceneByName("LoadingScene");
            if (loadingScene.IsValid() && loadingScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(loadingScene);
                Debug.Log("🧹 LoadingScene 언로드 완료");
            }

            StartCoroutine(StratSpawn());
        }
    }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }
}