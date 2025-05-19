using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackArea : MonoBehaviour
{
    public float duration = 2f;
    public Image fillImage; // 채워지는 Image (UI)
    public GameObject visualEffect; // 장판 이펙트
    private System.Action onComplete;

    public void Initialize(Vector3 pos, Quaternion rot, Vector3 size, System.Action onCompleteCallback)
    {
        Vector3 adjustedPos = pos;
        adjustedPos.y += 0.01f;
        transform.position = adjustedPos;
        transform.rotation = Quaternion.Euler(90, rot.eulerAngles.y, 0);
        transform.localScale = size;
        onComplete = onCompleteCallback;

        StartCoroutine(ShowTelegraph());
    }

    private IEnumerator ShowTelegraph()
    {
        float timer = 0f;
        fillImage.fillAmount = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        onComplete?.Invoke(); // 실제 공격 발생
        Destroy(gameObject); // 장판 제거
    }
}
