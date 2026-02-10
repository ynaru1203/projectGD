using UnityEngine;

public class PointerFollower : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas parentCanvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // PC라면 실제 마우스 커서는 숨기는 게 좋겠지?
        Cursor.visible = false;
    }

    void Update()
    {
        // 마우스나 터치 위치를 가져와
        Vector2 mousePos = Input.mousePosition;

        // Canvas가 RenderMode에 따라 좌표계가 다르니 안전하게 변환해줄게
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = mousePos;
        }
        else
        {
            // Screen Space - Camera 모드일 때 사용하는 방식이야
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                mousePos,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );
            rectTransform.anchoredPosition = localPoint;
        }
    }
}
