using UnityEngine;

// 패시브의 종류 정의
public enum PassiveType { AttackPower, MoveSpeed, SpawnRateBonus, ExpGain }

[CreateAssetMenu(fileName = "NewPassive", menuName = "Upgrade/Passive")]
public class PassiveData : ScriptableObject
{
    public string passiveName;
    [TextArea] public string description;
    public PassiveType type;
    public float valuePerLevel; // 레벨당 상승 수치
    public Sprite icon;         // UI에 표시될 아이콘
}