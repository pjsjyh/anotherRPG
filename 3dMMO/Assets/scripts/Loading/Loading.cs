using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    //로딩 시작.
    public GameObject networkManagerPrefab;
    
    void Start()
    {
        Debug.Log("thisis loading");
        StartCoroutine(LoadMainSceneAsync());
    }
    //로그인 후 데이터 가져와 플레이어 셋팅
    // 네트워크 셋팅
    IEnumerator LoadMainSceneAsync()
    {
        var initTask = GameManager.Instance.InitializeGameManagers();
        yield return new WaitUntil(() => initTask.IsCompleted);
        var task = NetworkManager.instance.StartNetworkFunc();
        yield return new WaitUntil(() => task.IsCompleted);
        yield return new WaitForSeconds(3.0f);
       // SceneManager.LoadScene("GameScene");
        yield return new WaitForSeconds(0.5f);
        Debug.Log($"NetworkManager 존재 여부: {FindObjectOfType<NetworkManager>() != null}");
    }

}
