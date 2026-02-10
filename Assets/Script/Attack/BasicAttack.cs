using UnityEngine;

[CreateAssetMenu(fileName = "NewBasicAttack", menuName = "Skills/BasicAttack")]
public class BasicAttack : SkillData
{
    [Header("추가 설정")]
    public float returnSpeed = 5.0f; // 돌아오는 속도

    public override void Execute(Vector2 mousePosition)
    {
        // 1. 커서 오브젝트를 찾아서 연출을 실행해 (태그나 싱글톤 활용)
        GameObject cursor = GameObject.FindWithTag("Cursor");
        if (cursor != null)
        {
            var animator = cursor.GetComponent<CursorAnimator>();
            // 여기서 본인의 attackRadius를 넘겨주는 거지!
            if (animator != null) animator.PlayBurst(range, returnSpeed);
        }

        // 2. 실제 공격 판정 (부드럽게 뽁!)
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(mousePosition, range);
        foreach (var enemy in hitEnemies)
        {
            if (enemy != null) 
            {
                try
                {
                    enemy.gameObject.GetComponent<Enemy>().TakeDamage(dmg);
                }
                catch
                {

                }
            }
        }
    }
}