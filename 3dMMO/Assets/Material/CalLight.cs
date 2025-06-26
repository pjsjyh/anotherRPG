using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalLight : MonoBehaviour
{
    public Light directionalLight;
    Material targetMaterial;

    private void Start()
    {
        targetMaterial = GetComponent<SkinnedMeshRenderer>().material;
    }
    void Update()
    {
        if (directionalLight && targetMaterial)
        {
            Vector3 lightDir = -directionalLight.transform.forward; // ������ ���ϴ� ����
            targetMaterial.SetVector("_LightDir", new Vector4(lightDir.x, lightDir.y, lightDir.z, 0));
        }
    }
}
