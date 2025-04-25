using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public GameObject networkManagerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("thisis loading");
        StartCoroutine(LoadMainSceneAsync());
    }

    IEnumerator LoadMainSceneAsync()
    {
        GameManager.Instance.InitializeGameManagers();
        //yield return StartCoroutine(SettingNetwork());
        yield return new WaitForSeconds(3.0f);
       // SceneManager.LoadScene("GameScene");
        yield return new WaitForSeconds(0.5f);
        Debug.Log($"🔹 NetworkManager 존재 여부: {FindObjectOfType<NetworkManager>() != null}");
    }

    IEnumerator SettingNetwork()
    {
        if (FindObjectOfType<NetworkManager>() != null)
        {
            Debug.Log("✅ 이미 NetworkManager가 존재함, 새로 생성하지 않음.");
            yield break;
        }
        GameObject networkManagerObj = Instantiate(networkManagerPrefab);
        Debug.Log("🔹 NetworkManager 생성 중...");
        DontDestroyOnLoad(networkManagerObj);
        Debug.Log("✅ DontDestroyOnLoad 적용 완료!");
        NetworkManager networkManager = networkManagerObj.GetComponent<NetworkManager>();
        Task startNetworkTask = networkManager.StartNetwork();
        yield return new WaitUntil(() => startNetworkTask.IsCompleted); // Task 완료될 때까지 대기

    }
}
