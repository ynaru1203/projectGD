using UnityEngine;
public enum UpgradeType
{
    AttackRange,    // 범위
    AttackDamage,   // 공격력
    AttackSpeed,     // 투사체 속도
    Cooldown,       //쿨다운
    multiply        //배율
}
[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrade/UpgradeOption")]
public class UpgradeData : ScriptableObject
{
    public UpgradeType upgradeType; // 이 옵션이 무엇을 강화하는지 선택!
    public string upgradeName;    // 강화 이름 (예: 사거리 확장)
    [TextArea]
    public string description;    // 설명
    public Sprite icon;           // UI에 표시될 아이콘

    public float weight = 10f;    // 등장 확률 가중치 (높을수록 잘 나옴)

    [Header("등급별 수치 (일반/레어/에픽/전설)")]
    public float[] tierMultipliers = { 1.0f, 1.5f, 2.0f, 2.5f }; // 등급별 배율
    public float baseValue;       // 기본 증가 수치
}