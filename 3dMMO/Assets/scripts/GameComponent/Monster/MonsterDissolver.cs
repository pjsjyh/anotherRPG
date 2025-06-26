using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDissolver : MonoBehaviour
{
    public float dissolveDuration = 1.5f;
    private Material mat;
    public GameObject body;
    private float cut = 0f;
    private float elapsed = 0f;
    private bool isDissolving = false;

    void Start()
    {
        if (mat == null)
        {
            var renderer = body.GetComponent<SkinnedMeshRenderer>();
            if (renderer != null)
                mat = renderer.material;
        }
    }
    public void TriggerDissolve()
    {
        isDissolving = true;
        elapsed = 0f;
    }

    void Update()
    {
        if (!isDissolving || mat == null) return;

        elapsed += Time.deltaTime;
        cut = Mathf.Clamp01(elapsed / dissolveDuration);
        mat.SetFloat("_Cut", cut);

        if (cut >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
