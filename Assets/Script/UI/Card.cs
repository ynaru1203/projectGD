using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // GraphicRaycaster 등을 위해 추가

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform cardImage;
    [SerializeField] private Vector3 cardRotateValue = new Vector3(15, 15, 0);
    [SerializeField] private float lerpSpeed = 10f;

    [Header("스케일 설정")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleSpeed = 10f;

    private RectTransform parentRect;
    private Canvas cardCanvas;
    private bool isHovering = false;
    private Vector3 initialScale;

    private void Start()
    {
        parentRect = GetComponent<RectTransform>();
        cardCanvas = GetComponent<Canvas>();

        if (cardImage != null)
            initialScale = cardImage.localScale;

        // 혹시 모르니 레이캐스트 타겟 확인
        var img = GetComponent<Image>();
        if (img != null) img.raycastTarget = true;
    }

    private void Update()
    {
        if (cardImage == null) return;

        // 1. 스케일 처리
        Vector3 targetScale = isHovering ? initialScale * hoverScale : initialScale;
        cardImage.localScale = Vector3.Lerp(cardImage.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);

        // 2. 마우스가 위에 있을 때 실시간 회전 처리 (움직이지 않아도 실행됨)
        if (isHovering)
        {
            UpdateRotation();
        }
    }

    private void UpdateRotation()
    {
        Vector2 localCursor;
        // 스크린 좌표를 부모의 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, uiCamera, out localCursor))
        {
            // 중앙 0 기준 -1 ~ 1 범위로 정규화
            float normalizedX = Mathf.Clamp(localCursor.x / (parentRect.rect.width * 0.5f), -1f, 1f);
            float normalizedY = Mathf.Clamp(localCursor.y / (parentRect.rect.height * 0.5f), -1f, 1f);

            // [핵심] 부호 수정: Y가 마이너스(아래)일 때 X회전도 마이너스가 되어야 함
            // 선생이 느낀 "반대로 뒤집힘" 현상을 여기서 해결!
            float rotateX = normalizedY * cardRotateValue.x;
            float rotateY = -normalizedX * cardRotateValue.y;

            Quaternion targetRotation = Quaternion.Euler(rotateX, rotateY, 0);
            cardImage.localRotation = Quaternion.Lerp(cardImage.localRotation, targetRotation, Time.unscaledDeltaTime * lerpSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (cardCanvas != null) cardCanvas.sortingOrder = 10;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (cardCanvas != null) cardCanvas.sortingOrder = 0;

        // 회전 초기화는 부드럽게 코루틴으로 (Update와 겹치지 않게 isHovering이 false일 때만 작동)
        StartCoroutine(CardImageReset());
    }

    public void ForcedPointerExit()
    {
        isHovering = false;
        if (cardCanvas != null) cardCanvas.sortingOrder = 0;

        // 회전 초기화는 부드럽게 코루틴으로 (Update와 겹치지 않게 isHovering이 false일 때만 작동)
        StartCoroutine(CardImageReset());
    }

    private IEnumerator CardImageReset()
    {
        // 마우스가 다시 들어오면 리셋 중단
        while (!isHovering && Quaternion.Angle(cardImage.localRotation, Quaternion.identity) > 0.1f)
        {
            cardImage.localRotation = Quaternion.Slerp(cardImage.localRotation, Quaternion.identity, Time.unscaledDeltaTime * lerpSpeed);
            yield return null;
        }
        if (!isHovering) cardImage.localRotation = Quaternion.identity;
    }
}