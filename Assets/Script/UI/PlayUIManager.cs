using UnityEngine;
using UnityEngine.UI;
public class PlayUIManager : MonoBehaviour
{
    public static PlayUIManager instance;

    [Header("UI 슬라이더")]
    public Slider enemyGauge;
    public Slider expBar;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        UpdateEnemyGauge();
        UpdateExpBar();
    }

    private void UpdateEnemyGauge()
    {
        if (EnemyManager.instance == null || enemyGauge == null) return;

        // 현재 적 마릿수 / 최대 적 마릿수 비율 계산
        float ratio = (float)EnemyManager.instance.activeEnemies.Count / EnemyManager.instance.maxEnemyCount;
        enemyGauge.value = ratio;

        // 적이 80% 이상 차면 게이지를 깜빡이게 하거나 색을 바꾸면 더 긴장되겠지?
    }

    private void UpdateExpBar()
    {
        if (ExperienceManager.Instance == null || expBar == null) return;

        // 현재 경험치 / 다음 레벨까지 필요한 경험치 비율
        // (ExperienceManager에 관련 변수가 있다고 가정할게!)
        float ratio = (float)ExperienceManager.Instance.currentExp / ExperienceManager.Instance.nextLevelExp;
        expBar.value = ratio;
    }
}
