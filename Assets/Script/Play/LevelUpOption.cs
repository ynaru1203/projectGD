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
        title = $"{s.skillName} 강화";
        description = $"{u.data.upgradeName} ({upgrade.tier}등급)";
    }

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

    public PassiveOption(PassiveData data)
    {
        type = OptionType.Passive;
        passiveData = data;
        title = data.passiveName;
        // 설명에 현재 적용될 수치를 보여주면 더 친절하겠지?
        description = $"{data.description}\n(현재 효과: +{data.valuePerLevel})";
    }

    public override void Apply()
    {
        // 패시브 매니저에게 이 데이터를 전달해서 수치를 올리라고 시켜!
        PassiveManager.Instance.AddPassive(passiveData);
        UnityEngine.Debug.Log($"{passiveData.passiveName} 패시브 적용 완료!");
    }
}