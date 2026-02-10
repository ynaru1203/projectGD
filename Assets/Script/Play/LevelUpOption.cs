using UnityEngine;

public enum OptionType { NewSkill, UpgradeSkill, Passive }

public abstract class LevelUpOption
{
    public string title;
    public string description;
    public OptionType type;

    // 이 옵션을 선택했을 때 실행될 로직
    public abstract void Apply();
}

public class SkillUpgradeOption : LevelUpOption
{
    public SelectedUpgrade upgrade;
    public SkillData targetSkill;

    public SkillUpgradeOption(SelectedUpgrade u, SkillData s)
    {
        type = OptionType.UpgradeSkill;
        upgrade = u;
        targetSkill = s;
        title = $"{s.skillName} 강화 ({GetTierName(upgrade.tier)})";
        description = $"{u.data.upgradeName} : {u.data.baseValue * u.data.tierMultipliers[upgrade.tier]}%";
        if(upgrade.data.upgradeType == UpgradeType.AttackDamage)
        {
            description = $"{u.data.upgradeName} : {u.data.baseValue * u.data.tierMultipliers[upgrade.tier]}";
        }
    }
    private string GetTierName(int t) => t switch { 3 => "전설", 2 => "에픽", 1 => "레어", _ => "일반" };
    public override void Apply()
    {
        // 아까 만든 UpgradeManager의 로직을 여기서 실행!
        // (UpgradeManager를 싱글톤이나 참조로 들고 있어야 해)
        ExperienceManager.Instance.upgradeManager.ApplyUpgrade(upgrade, targetSkill);
    }
}
// 2. 새로운 스킬 획득 옵션
public class NewSkillOption : LevelUpOption
{
    public SkillData skillToLearn;

    public NewSkillOption(SkillData s)
    {
        type = OptionType.NewSkill;
        skillToLearn = s;
        title = $"새로운 스킬: {s.skillName}";
        description = "새로운 힘을 얻습니다.";
    }

    public override void Apply()
    {
        // 플레이어 스킬 리스트에 추가하는 로직
        ExperienceManager.Instance.LearnNewSkill(skillToLearn);
    }
}

public class PassiveOption : LevelUpOption
{
    public PassiveData passiveData;
    public int tier; // 등급 추가

    public PassiveOption(PassiveData data, int t)
    {
        type = OptionType.Passive;
        passiveData = data;
        tier = t;
        title = $"패시브 ({GetTierName(t)})";

        float effectValue = data.valuePerLevel * data.tierMultipliers[t];
        description = $"{data.passiveName}: {effectValue}%";
    }

    private string GetTierName(int t) => t switch { 3 => "전설", 2 => "에픽", 1 => "레어", _ => "일반" };

    public override void Apply()
    {
        PassiveManager.Instance.AddPassive(passiveData, tier);
    }
}