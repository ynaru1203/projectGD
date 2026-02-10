using System.Collections;
using UnityEngine;

public class LightTrailEnemy : Enemy
{
    [Header("빛의 궤적 설정")]
    public float dashSpeed = 15f;
    private Vector2 _currentDir;
    private Rigidbody2D _rb;

    protected override void Awake()
    {
        base.Awake(); // 부모의 캐싱 실행
        _rb = GetComponent<Rigidbody2D>();

        // 물리 엔진이 멋대로 회전시키지 못하게 고정
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        // 다른 적을 밀어낼 힘은 유지하되, 마찰로 멈추지 않게 질량 설정
        _rb.mass = 1000f;
    }
    public override void Init(Vector2 position)
    {
        base.Init(position);
        hp = EnemyManager.maxEnemyHp * 5;
    }
    protected override void ApplyRandomMovement()
    {
        Vector2 targetPos = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0.3f, 0.7f), Random.Range(0.3f, 0.7f)));
        _currentDir = (targetPos - (Vector2)transform.position).normalized;

        LookAtDirection(_currentDir);
    }
    private void FixedUpdate() // 물리 이동은 FixedUpdate가 국룰이야!
    {
        if (isdead) return;

        // 2. MovePosition을 사용하여 물리 판정을 유지하며 강제 이동
        // 이러면 앞길을 막는 적들을 질량 1000의 힘으로 다 튕겨내며 지나가
        Vector2 nextPos = (Vector2)transform.position + (_currentDir * dashSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(nextPos);
    }
    private void LookAtDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ScreenBoundary") || collision.gameObject.CompareTag("LightTrail"))
        {
            // 3. 벽에 닿으면 즉시 Reflect로 방향 꺾기
            Vector2 normal = collision.contacts[0].normal;
            _currentDir = Vector2.Reflect(_currentDir, normal).normalized;

            // 4. 벽에 끼어서 부들거리지 않게 살짝 떼어주기
            _rb.position += normal * 0.1f;

            LookAtDirection(_currentDir);
        }
        // 일반 적(Enemy)과 부딪히면? 
        // 질량 차이 때문에 방향 안 바꾸고 그냥 밀어버리며 지나갈 거야!
    }
    public override void TakeDamage(int Damage)
    {
        if (isdead) return;
        effect.OnTakeDamage();
        hp -= Damage;
        if (hp <= 0)
        {
            StartCoroutine(DieRoutine2());
        }
    }
    protected IEnumerator DieRoutine2()
    {
        isdead = true;

        // 1. 물리 엔진 계산에서 완전히 제외시키기
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false; // 물리 연산 중지 및 다른 물리 객체와 충돌 안 함
        }

        // 2. 트리거와 콜라이더도 확실히 꺼서 판정 없애기
        if (col != null)
        {
            col.enabled = false;
        }
        if (polygon != null)
        {
            polygon.SetActive(false);
        }
        // 경험치 매니저에게 5점을 주라고 시키기
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(5);
        }
        // 3. 이펙트가 충분히 나올 시간 기다리기 (0.5초 정도?)
        yield return new WaitForSeconds(0.5f);
        // 다시 풀로 돌아갈 준비
        if (rb != null) rb.simulated = true;
        if (col != null) col.enabled = true;
        // 4. 매니저에게 나 죽었다고 보고하고 비활성화
        EnemyManager.instance.OnEnemyDespawned(gameObject);
        col.enabled = true; // 다음에 나올 때를 위해 콜라이더는 미리 켜두기
        Destroy(gameObject);
    }
}