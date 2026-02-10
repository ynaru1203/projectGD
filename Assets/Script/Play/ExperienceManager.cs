using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    [Header("레벨 시스템")]
    public int currentLevel = 1;
    public float currentExp = 0;
    public float nextLevelExp = 100;

    [Header("참조")]
    public UpgradeManager upgradeManager;
    public GameObject upgradeUI;

    void Awake() => Instance = this;

    public void AddExperience(float amount)
    {
        currentExp += amount;
        while (currentExp >= nextLevelExp) LevelUp();
    }

    private void LevelUp()
    {
        currentExp -= nextLevelExp;
        currentLevel++;
        nextLevelExp = Mathf.RoundToInt(nextLevelExp * 1.1f);//레벨업 할 떄마다 요구 경험칠량 1.1배
        ShowUpgradeUI();
    }

    public void ShowUpgradeUI()
    {
        Time.timeScale = 0f;
        List<LevelUpOption> finalOptions = new List<LevelUpOption>();

        var db = AttackManager.instance.allSkillDatabase;
        var acquired = AttackManager.instance.acquiredSkills;
        var availableNewSkills = db.Where(s => !acquired.Any(a => a.No == s.No)).ToList();

        int safetyNet = 0;
        while (finalOptions.Count < 3 && safetyNet < 100)
        {
            safetyNet++;
            float r = Random.Range(0f, 100f);
            LevelUpOption selected = null;

            if (r < 33.3f) // 1. 새로운 스킬 (33%)
            {
                if (availableNewSkills.Count > 0)
                {
                    var skill = availableNewSkills[Random.Range(0, availableNewSkills.Count)];
                    selected = new NewSkillOption(skill);
                }
            }
            else if (r < 66.6f) // 2. 기존 스킬 강화 (33%)
            {
                if (acquired.Count > 0)
                {
                    var skill = acquired[Random.Range(0, acquired.Count)];
                    // UpgradeManager에서 가중치 계산된 강화 하나를 가져와
                    var upgrades = upgradeManager.GetRandomUpgrades(1);
                    if (upgrades.Count > 0) selected = new SkillUpgradeOption(upgrades[0], skill);
                }
            }
            else // 3. 패시브 (33%)
            {
                selected = new PassiveOption();
            }

            // 선택한 카테고리가 불가능할 때의 예외 처리
            if (selected == null)
            {
                if (availableNewSkills.Count > 0)
                    selected = new NewSkillOption(availableNewSkills[Random.Range(0, availableNewSkills.Count)]);
                else if (acquired.Count > 0)
                    selected = new SkillUpgradeOption(upgradeManager.GetRandomUpgrades(1)[0], acquired[Random.Range(0, acquired.Count)]);
                else
                    selected = new PassiveOption();
            }

            // 중복 체크 (한 화면에 똑같은 카드가 나오면 안 되니까)
            if (selected != null && !finalOptions.Any(opt => opt.title == selected.title && opt.description == selected.description))
            {
                finalOptions.Add(selected);
                if (selected is NewSkillOption nso) availableNewSkills.Remove(nso.skillToLearn);
            }
        }

        if (upgradeUI != null)
        {
            upgradeUI.SetActive(true);
            upgradeUI.GetComponent<UpgradeUIController>().Init(finalOptions);
        }
    }


    public void LearnNewSkill(SkillData skill)
    {
        AttackManager.instance.AcquireSkill(skill.No);
    }
}