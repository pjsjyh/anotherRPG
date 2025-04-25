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
        // ���׸��� �������� (SkinnedMeshRenderer ����)
       
        var renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            _material = renderer.materials[1]; // ���̴��� ����� 2��° ���׸���
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
        // HitAmount = 1 (������ ���̰�)
        _material.SetFloat(HitAmount, 1f);

        yield return new WaitForSeconds(0.2f); // 0.2�� ����

        // �ٽ� 0���� (ȿ�� ����)
        _material.SetFloat(HitAmount, 0f);
    }
}
