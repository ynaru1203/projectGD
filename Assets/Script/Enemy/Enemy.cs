using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("스펙")]
    public int hp = 5;
    public int sides = 3;
    public Color color = Color.white;
    public bool isdead;
    [Header("설정 범위")]
    public float minSpeed = 2.0f;
    public float maxSpeed = 5.0f;
    public float minRotationSpeed = 30f;
    public float maxRotationSpeed = 150f;
    public float radius = 0.5f; // 반지름

    public GameObject polygon;
    public EnemyEffect effect;
    protected Rigidbody2D rb;
    protected Collider2D col;
    private PolygonSync sync;
    private bool isInside = false;
    private bool isReady = false;




    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        try
        {
            
            sync = GetComponent<PolygonSync>();
        }
        catch
        {

        }
        isReady = true;
    }
    
    public virtual void Init(Vector2 position)
    {
        if (!isReady || rb == null) return;
        isdead = false;
        hp = EnemyManager.maxEnemyHp; // 또는 원하는 기본 HP
        polygon.SetActive(true); // 숨겼던 폴리곤 다시 켜기
        rb.simulated = true; // 물리 다시 켜기!
        if (sync != null)
        {
            sync.UpdateCollider();
        }
        col.enabled = true;  // 콜라이더 다시 켜기!
        gameObject.transform.position = position;
        // 1. 물리 상태 초기화 (이전의 힘을 깨끗이 지우기)
        ResetEnemyState();
        // 2. 새로운 목표와 속도 부여 (아까 만든 로직)
        ApplyRandomMovement();

        polygon.GetComponent<EnemyVisual>().SetEnemyVisual(sides,color);
    }

    protected virtual void ApplyRandomMovement()
    {

        // 1. 화면 안쪽 랜덤한 목표 지점 잡기
        Vector2 randomInsidePos = Camera.main.ViewportToWorldPoint(new Vector2(
            Random.Range(0.2f, 0.8f),
            Random.Range(0.2f, 0.8f)
        ));

        // 2. 랜덤 속도 계산
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        Vector2 direction = (randomInsidePos - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * randomSpeed;

        // 3. 랜덤 회전 속도 계산 (양수면 시계방향, 음수면 반시계방향)
        float randomTorque = Random.Range(minRotationSpeed, maxRotationSpeed);
        // 왼쪽으로 돌지 오른쪽으로 돌지도 랜덤하게 결정!
        rb.angularVelocity = Random.value > 0.5f ? randomTorque : -randomTorque;
    }

    private void ResetEnemyState()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // 2. 트리거 상태로 되돌리기 (화면 밖에서 들어와야 하니까!)
        isInside = false;
        if (col != null) col.isTrigger = true;


    }

    // 카메라 밖 콜라이더(경계선)를 빠져나갈 때 호출
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ScreenBoundary") && !isInside)
        {
            isInside = true;
            col.isTrigger = false; // 이제부터는 벽에 튕긴다!

        }
    }

    public virtual void TakeDamage(int Damage)
    {
        if (isdead) return;
        effect.OnTakeDamage();
        hp -= Damage;
        if (hp <= 0)
        {
            StartCoroutine(DieRoutine());
        }
    }

    protected IEnumerator DieRoutine()
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
        // 경험치 매니저에게 1점을 주라고 시키기
        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperience(1);
        }
        // 3. 이펙트가 충분히 나올 시간 기다리기 (0.5초 정도?)
        yield return new WaitForSeconds(0.5f);
        // 다시 풀로 돌아갈 준비
        if (rb != null) rb.simulated = true;
        if (col != null) col.enabled = true;
        // 4. 매니저에게 나 죽었다고 보고하고 비활성화
        EnemyManager.instance.OnEnemyDespawned(gameObject);
        col.enabled = true; // 다음에 나올 때를 위해 콜라이더는 미리 켜두기
        gameObject.SetActive(false);
    }
}