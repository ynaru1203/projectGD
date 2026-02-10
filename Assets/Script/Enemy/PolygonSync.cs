using UnityEngine;

public class PolygonSync : MonoBehaviour
{
    public int sides = 3; // 다각형 변의 수
    public float radius = 0.5f; // 반지름
    private PolygonCollider2D col;
    private int realsides = 3;
    void Awake()
    {
        col = GetComponent<PolygonCollider2D>();
    }

    public void UpdateCollider()
    {
        if (sides >= 20)
        {
            realsides = 20;
        }
        else
        {
            realsides = sides;
        }
            Vector2[] points = new Vector2[realsides];

        for (int i = 0; i < realsides; i++)
        {
            // 핵심: i에 0.5를 더하거나 시작 위치를 -90도(Mathf.PI / -2f)로 잡는 거야.
            // 쉐이더의 Polygon 노드는 보통 첫 번째 정점을 정중앙 위(12시)에 두려고 하거든.
            // float angle = (i * Mathf.PI * 2f / sides) - (Mathf.PI / 2f);

            // 만약 그래도 안 맞으면 아래처럼 '반 절반'만큼 더 돌려봐!
            float angle = ((i + 0.5f) * Mathf.PI * 2f / realsides) - (Mathf.PI / 2f);

            points[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }
        col.points = points;
    }
}