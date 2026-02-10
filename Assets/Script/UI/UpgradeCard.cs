using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public Image iconImage;
    public Image frameImage;
    public GameObject uiRoot;
    private LevelUpOption assignedOption;
    private Card cardVisual;

    private void Awake()
    {
        cardVisual = GetComponent<Card>();
    }
    public void Setup(LevelUpOption option)
    {
        assignedOption = option;
        titleText.text = option.title;
        descText.text = option.description;

        // 아이콘 및 프레임 색상 설정 (기존 로직 유지)
        UpdateVisuals(option);

        // 버튼 컴포넌트를 가져와서 클릭 이벤트를 초기화하고 새로 연결해
        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClickCard);
    }

    public void OnClickCard()
    {
        if (assignedOption != null)
        {
            // 1. 실제로 옵션 효과를 적용해 (강화 혹은 새 스킬)
            assignedOption.Apply();
            cardVisual.ForcedPointerExit();
            // 2. 멈췄던 시간을 다시 흐르게 해
            Time.timeScale = 1f;

            // 3. 열려있는 강화 UI 패널을 닫아버려
            // 보통 카드들이 Panel의 자식일 테니까 root를 지우거나 비활성화하면 돼
            if (uiRoot != null) uiRoot.SetActive(false);

            Debug.Log($"{assignedOption.title} 선택 완료!");
        }
    }

    private Color GetColorByTier(int tier)
    {
        return tier switch
        {
            3 => new Color(1f, 0.85f, 0.42f), // 전설
            2 => new Color(0.95f, 0.5f, 1f),  // 에픽
            1 => new Color(0.45f, 0.77f, 1f),  // 레어
            _ => Color.white
        };
    }
    private void UpdateVisuals(LevelUpOption option)
    {
        assignedOption = option;
        titleText.text = option.title;
        descText.text = option.description;

        if (option is SkillUpgradeOption skillOpt)
        {
            iconImage.sprite = skillOpt.targetSkill.icon;
            frameImage.color = GetColorByTier(skillOpt.upgrade.tier);
        }
        else if (option is NewSkillOption newSkillOpt)
        {
            iconImage.sprite = newSkillOpt.skillToLearn.icon;
            frameImage.color = Color.white;
        }
        else if (option is PassiveOption passive)
        {
            // 패시브용 기본 아이콘이 있다면 그걸 넣어줘
            iconImage.sprite = passive.passiveData.icon;
            frameImage.color = GetColorByTier(passive.tier);
        }
    }
}