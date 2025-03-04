using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadMainSceneAsync());
    }

    IEnumerator LoadMainSceneAsync()
    {
        GameManager.Instance.InitializeGameManagers();
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("GameScene");
    }
}
