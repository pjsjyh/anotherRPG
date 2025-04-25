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
        canvasGroup.alpha = 1f; // ������ �� ������ �˰�
    }

    public static void TurnOff()
    {
        if (instance != null)
            instance.StartCoroutine(instance.FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float duration = 1.5f; // ���̵� �ƿ� �ð�
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false); // �� ������ ����
    }
}
