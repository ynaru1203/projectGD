using UnityEngine;

// 패시브의 종류 정의
public enum PassiveType { CriticalChance, AttackPower, SkillSize, Cooldown, MaxSpawnEnemy, ExpGain, SpawnRate, Luck }

[CreateAssetMenu(fileName = "NewPassive", menuName = "Upgrade/Passive")]
public class PassiveData : ScriptableObject
{
    public string passiveName;
    [TextArea] public string description;
    public PassiveType type;
    public float valuePerLevel; // 레벨당 상승 수치
    public float[] tierMultipliers = { 1.0f, 1.1f, 1.2f, 1.3f }; // 일반, 레어, 에픽, 전설
    public Sprite icon;         // UI에 표시될 아이콘
}