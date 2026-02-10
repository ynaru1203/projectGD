using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassiveManager : MonoBehaviour
{
    public static PassiveManager Instance;

    // 누적된 패시브
    private Dictionary<PassiveType, float> passiveStats = new Dictionary<PassiveType, float>();
    private List<PassiveData> allPassives = new List<PassiveData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        // Resources/Passive 폴더에서 모든 패시브 로드
        allPassives.AddRange(Resources.LoadAll<PassiveData>("Passive"));
    }

    // 등급(tier)을 포함하여 패시브 추가
    public void AddPassive(PassiveData data, int tier)
    {
        if (!passiveStats.ContainsKey(data.type))
            passiveStats.Add(data.type, 0f);

        // 최종 증가치 = 기본 수치 * 등급 배율
        float finalValue = data.valuePerLevel * data.tierMultipliers[tier];
        passiveStats[data.type] += finalValue;

        Debug.Log($"{data.passiveName} ({tier}등급) 획득! 현재 총 보너스: {passiveStats[data.type]}");
    }

    public float GetStat(PassiveType type)
    {
        return passiveStats.ContainsKey(type) ? passiveStats[type] : 0f;
    }

    public PassiveData GetRandomPassiveData()
    {
        if (allPassives.Count == 0) return null;
        return allPassives[Random.Range(0, allPassives.Count)];
    }
}