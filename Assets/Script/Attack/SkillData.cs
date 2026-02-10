using UnityEngine;

public interface ISkill
{
    string SkillName { get; }
    float Cooldown { get; }
    void Execute(Vector2 mousePosition); // 마우스 위치에서 실행!
}

public abstract class SkillData : ScriptableObject, ISkill
{
    public int level = 1;
    public int No;
    public string skillName;
    public float cooldown;
    public Sprite icon;
    public float speed = 1f;
    public float range = 1f;

    [Header("대미지 계산 요소")]
    public int defaultDmg = 5;
    public float magnificationFactor = 1.0f;
    public int powerFactor = 0;


    public string SkillName => skillName;
    public float Cooldown => cooldown;
    public int dmg
    {
        get
        {
            float passiveDmgBonus = PassiveManager.Instance != null ? PassiveManager.Instance.GetStat(PassiveType.AttackPower) : 0f;
            // 최종 대미지 = (기본+강화) * 배율 * (1 + 패시브%)
            return Mathf.RoundToInt((defaultDmg + powerFactor) * magnificationFactor * (1f + passiveDmgBonus / 100));
        }
    }
    

    // 실제 스킬 로직은 여기서 각자 구현하게 될 거야.
    public abstract void Execute(Vector2 mousePosition);
}