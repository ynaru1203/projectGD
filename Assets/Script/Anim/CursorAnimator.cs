using UnityEngine;


public class CursorAnimator : MonoBehaviour
{
    private float lastWorldRadius;
    private RectTransform rectTransform;
    private Vector2 baseSizeDelta; // 평상시(85%) 크기
    private float currentReturnSpeed = 5f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // 시작할 때 현재 크기를 기본값으로 잡아둬
        baseSizeDelta = rectTransform.sizeDelta;
    }

    void Update()
    {
        // 현재 크기를 계산된 기본 크기(baseSizeDelta)로 부드럽게 되돌려
        rectTransform.sizeDelta = Vector2.Lerp(
            rectTransform.sizeDelta,
            baseSizeDelta,
            Time.deltaTime * currentReturnSpeed
        );
    }

    public void UpdateBaseRadius(float worldRadius)
    {
        // 1. 월드 단위 반지름을 화면 픽셀 단위로 변환
        float screenRadius = worldRadius * (Screen.height / (Camera.main.orthographicSize * 2f));
        float targetDiameter = screenRadius * 2f;

        // 2. 평상시 크기를 공격 범위의 85%로 설정 (최소 크기 제한 삭제!)
        baseSizeDelta = new Vector2(targetDiameter * 0.85f, targetDiameter * 0.85f);
    }

    public void PlayBurst(float worldRadius, float speed)
    {
        currentReturnSpeed = speed;

        // 범위를 먼저 갱신해주고
        UpdateBaseRadius(worldRadius);

        // 3. 공격 시점에는 실제 공격 범위(100%)까지 즉시 키워!
        float screenRadius = worldRadius * (Screen.height / (Camera.main.orthographicSize * 2f));
        float burstDiameter = screenRadius * 2f;

        rectTransform.sizeDelta = new Vector2(burstDiameter, burstDiameter);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || Camera.main == null) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        Gizmos.DrawWireSphere(worldPos, lastWorldRadius);
        Gizmos.DrawSphere(worldPos, 0.1f);
    }
}