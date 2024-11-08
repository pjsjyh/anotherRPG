using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransManager : MonoBehaviour
{
    public static SceneTransManager Instance { get; private set; } // 싱글톤

    public CanvasGroup fadeCanvasGroup; // Fade 효과를 위한 CanvasGroup
    public float fadeDuration = 1.0f; // Fade In/Out 지속 시간

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 파괴
        }
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadScene(sceneName));
    }

    // Fade Out -> 씬 로드 -> Fade In
    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // Fade Out
        yield return StartCoroutine(FadeOut(fadeDuration));

        // 씬 비동기 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade In
        yield return StartCoroutine(FadeIn(fadeDuration));
    }

    private IEnumerator FadeOut(float duration)
    {
        float startAlpha = fadeCanvasGroup.alpha;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1, t / duration); // 완전히 검은 화면
            yield return null;
        }

        fadeCanvasGroup.alpha = 1; // 완전히 어두워짐
    }

    private IEnumerator FadeIn(float duration)
    {
        float startAlpha = fadeCanvasGroup.alpha;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / duration); // 화면이 밝아짐
            yield return null;
        }

        fadeCanvasGroup.alpha = 0; // 완전히 밝은 상태
    }
}
