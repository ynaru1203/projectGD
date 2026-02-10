using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class AttackManager : MonoBehaviour
{
    public static AttackManager instance;

    [Header("스킬 데이터")]
    // 게임에 존재하는 모든 스킬 (데이터베이스 역할)
    public List<SkillData> allSkillDatabase = new List<SkillData>();

    // 선생이 실제로 획득한 스킬들 (이것만 Update에서 실행돼!)
    public List<SkillData> acquiredSkills = new List<SkillData>();

    private Dictionary<int, float> lastUsedTime = new Dictionary<int, float>();

    void Awake()
    {
        instance = this;
        LoadAllSkills();
    }

    void LoadAllSkills()
    {
        // 1. 폴더에서 모든 스킬 로드
        SkillData[] loaded = Resources.LoadAll<SkillData>("Skills");

        foreach (var original in loaded)
        {
            // 2. 인스턴스 복사해서 DB에 저장 (원본 보호)
            SkillData copy = Instantiate(original);
            allSkillDatabase.Add(copy);
        }

        // 으헤~ 게임 시작할 때 기본 공격 하나는 들고 시작해야겠지?
        // 예: No가 0인 스킬을 기본으로 준다고 가정해볼게.
        AcquireSkill(0);
    }

    public void AcquireSkill(int skillNo)
    {
        // 이미 배운 스킬인지 확인
        if (acquiredSkills.Any(s => s.No == skillNo))
        {
            Debug.Log($"{skillNo}번 스킬은 이미 가지고 있어");
            return;
        }

        // DB에서 해당 번호의 스킬을 찾아서 내 인벤토리에 추가!
        SkillData skillToLearn = allSkillDatabase.Find(s => s.No == skillNo);
        if (skillToLearn != null)
        {
            acquiredSkills.Add(skillToLearn);
            Debug.Log($"새로운 스킬 '{skillToLearn.skillName}' 획득!");
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ★ 중요: 이제 '획득한 스킬' 리스트만 돌면서 실행해!
        foreach (var skill in acquiredSkills)
        {
            if (!lastUsedTime.ContainsKey(skill.No))
                lastUsedTime[skill.No] = -skill.cooldown;

            if (Time.time >= lastUsedTime[skill.No] + skill.cooldown)
            {
                skill.Execute(mousePos);
                lastUsedTime[skill.No] = Time.time;
            }
        }
    }
}