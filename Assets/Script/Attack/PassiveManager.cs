using System.Collections.Generic;
using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    public static PassiveManager Instance;

    // 각 패시브 타입별 누적 수치를 저장하는 딕셔너리
    private Dictionary<PassiveType, float> passiveStats = new Dictionary<PassiveType, float>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddPassive(PassiveData data)
    {
        if (!passiveStats.ContainsKey(data.type))
            passiveStats.Add(data.type, 0f);

        passiveStats[data.type] += data.valuePerLevel;
    }

    // 다른 스크립트(EnemyManager 등)에서 보너스 수치를 가져갈 때 사용
    public float GetStat(PassiveType type)
    {
        return passiveStats.ContainsKey(type) ? passiveStats[type] : 0f;
    }
}