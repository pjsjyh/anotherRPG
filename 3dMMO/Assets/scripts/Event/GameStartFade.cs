using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartFade : MonoBehaviour
{
    private static GameStartFade instance;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f; // 시작할 때 완전히 검게
    }

    public static void TurnOff()
    {
        if (instance != null)
            instance.StartCoroutine(instance.FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float duration = 1.5f; // 페이드 아웃 시간
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false); // 다 끝나면 꺼줌
    }
}
