using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHit : MonoBehaviour
{
    private Material _material;
    private Coroutine _hitCoroutine;
    private static readonly int HitAmount = Shader.PropertyToID("_HitAmount");

    void Start()
    {
        // 머테리얼 가져오기 (SkinnedMeshRenderer 기준)
       
        var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            _material = renderer.materials[1]; // 쉐이더가 적용된 2번째 머테리얼
        }
    }

    public void PlayHitEffect()
    {
        if (_hitCoroutine != null)
            StopCoroutine(_hitCoroutine);

        _hitCoroutine = StartCoroutine(HitFlash());
    }

    private System.Collections.IEnumerator HitFlash()
    {
        // HitAmount = 1 (빨간색 보이게)
        _material.SetFloat(HitAmount, 1f);

        yield return new WaitForSeconds(0.2f); // 0.2초 유지

        // 다시 0으로 (효과 제거)
        _material.SetFloat(HitAmount, 0f);
    }
}
