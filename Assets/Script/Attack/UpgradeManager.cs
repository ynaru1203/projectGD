using System.Collections.Generic;
using UnityEngine;
using System.Linq; // 가중치 계산을 위해 추가

public class UpgradeManager : MonoBehaviour
{
    private List<UpgradeData> allUpgrades = new List<UpgradeData>();

    void Awake()
    {
        // 1. Resources/Upgrade 폴더 내의 모든 UpgradeData를 자동으로 로드해
        UpgradeData[] loadedUpgrades = Resources.LoadAll<UpgradeData>("Upgrade");
        allUpgrades.AddRange(loadedUpgrades);

        Debug.Log($" {allUpgrades.Count}개의 업그레이드 데이터를 찾아냈어.");
    }

    public List<SelectedUpgrade> GetRandomUpgrades(int count = 3)
    {
        List<SelectedUpgrade> selections = new List<SelectedUpgrade>();
        // 원본 보호를 위해 복사본 생성
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0) break;

            // 2. 가중치 기반 랜덤 선택 (더 정교하게!)
            UpgradeData selectedData = GetWeightedRandom(pool);

            // 3. 등급 결정 (전설 포함)
            int tier = DetermineTier();

            selections.Add(new SelectedUpgrade(selectedData, tier));
            pool.Remove(selectedData); // 중복 방지
        }
        return selections;
    }

    private UpgradeData GetWeightedRandom(List<UpgradeData> pool)
    {
        float totalWeight = pool.Sum(x => x.weight);
        float pivot = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var upgrade in pool)
        {
            currentWeight += upgrade.weight;
            if (pivot <= currentWeight) return upgrade;
        }
        return pool[0];
    }

    public int DetermineTier()
    {
        // 각 등급이 나올 확률을 % 단위로 딱 정해두자
        float legendaryChance = 3f;  // 3%
        float epicChance = 7f;      // 7%
        float rareChance = 15f;      // 15%
                                     // 나머지는 일반(75%)

        float rand = Random.Range(0f, 100f); // 0~100 사이 숫자를 뽑아

        if (rand < legendaryChance)
            return 3; // 0 ~ 3 미만

        if (rand < legendaryChance + epicChance)
            return 2; // 3 ~ 10 미만

        if (rand < legendaryChance + epicChance + rareChance)
            return 1; // 10 ~ 25 미만

        return 0; // 25 ~ 100
    }

    public void ApplyUpgrade(SelectedUpgrade upgrade, SkillData targetSkill)
    {
        // 등급별 최종 수치 계산 (baseValue * tierMultiplier)
        float finalValue = upgrade.data.baseValue * upgrade.data.tierMultipliers[upgrade.tier];

        switch (upgrade.data.upgradeType)
        {
            case UpgradeType.AttackRange:
                // 사거리 증가 (5라고 적으면 0.05만큼 증가)
                targetSkill.range += (finalValue / 100f);
                break;

            case UpgradeType.AttackDamage:
                // 공격력 포인트는 정수형이므로 그대로 반올림하여 합산
                targetSkill.powerFactor += Mathf.RoundToInt(finalValue);
                break;

            case UpgradeType.AttackSpeed:
                // 투사체 속도 수치 합산 (5라고 적으면 0.05만큼 증가)
                targetSkill.speed += (finalValue / 100f);
                break;

            case UpgradeType.Cooldown:
                // 5라고 적으면 0.05(5%)로 변환해서 계산
                float cooldownReductionRate = finalValue / 100f;
                // 쿨다운 = 현재 쿨다운 * (1 - 감소율)
                targetSkill.cooldown *= (1f - cooldownReductionRate);
                break;

            case UpgradeType.multiply:
                // 대미지 배율도 5라고 적으면 0.05(5%) 만큼 증가하도록 변환
                float magnificationBonus = finalValue / 100f;
                targetSkill.magnificationFactor += magnificationBonus;
                break;
        }

        Debug.Log($"{upgrade.data.upgradeName}({upgrade.tier}등급) 적용 완료!");
    }
}

public class SelectedUpgrade
{
    public UpgradeData data; // 설계도 (이름, 아이콘, 기본 수치 등)
    public int tier;         // 뽑힌 등급 (0:일반, 1:레어, 2:에픽, 3:전설)

    // 생성자: 뽑는 순간 데이터와 등급을 딱 붙여서 고정해줘
    public SelectedUpgrade(UpgradeData d, int t)
    {
        data = d;
        tier = t;
    }
}