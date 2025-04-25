
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SwordEffect : MonoBehaviour
{
    [Range(3, 100)]
    public int segments = 20;         // 슬래시 곡선의 세밀도
    public float radius = 1f;         // 슬래시 반지름
    public float angle = 90f;         // 부채꼴 각도

    void Start()
    {
        GenerateSlashMesh();
    }

    void GenerateSlashMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // 중심점
        vertices[0] = Vector3.zero;

        float angleStep = angle / segments;
        float halfAngle = angle / 2f;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = -halfAngle + i * angleStep;
            float rad = Mathf.Deg2Rad * currentAngle;

            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;

            vertices[i + 1] = new Vector3(x, y, 0);
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
        transform.LookAt(Camera.main.transform);

    }
}
