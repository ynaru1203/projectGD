using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public GameObject enemyPrefab;
    private float spawnInterval;
    private float spawnTimer;
    public int maxEnemyCount = 200;
    public static int maxEnemyHp = 5;

    private float cumulativeTime;

    [Header("고급 난이도 매개변수")]
    public int baseHp = 5;

    [Tooltip("성장 가속도 계수 (추천: 0.05)")]
    public float growthRate = 0.05f;

    [Tooltip("난이도 곡선 변곡점 (추천: 1.2)")]
    public float complexityFactor = 1.2f;

    public List<GameObject> activeEnemies = new List<GameObject>();
    private EnemyPool enemyPool;

    [Header("스폰 속도 조절")]
    public AnimationCurve spawnCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float minSpawnInterval = 0.001f; // 적이 0마리일 때 (최고 속도)
    public float maxSpawnInterval = 0.1f; // 적이 최대치(maxEnemyCount)에 도달할 때 (최저 속도)

    [Header("보너스 적 설정")]
    public GameObject lightTrailEnemyPrefab; // 빛의 궤적 적 프리팹
    public float bonusSpawnInterval = 15f;
    public int bonusSpawnCount = 10;
    private float bonusSpawnTimer = 0f;

    private void Awake()
    {
        instance = this;
        enemyPool = GetComponent<EnemyPool>();
    }
    void Update()
    {
        // 1. 누적 시간 및 HP 계산
        cumulativeTime += Time.deltaTime;


        // 2. [추가] 30초마다 5마리 추가 스폰 로직
        // 30초 보너스 타이머
        bonusSpawnTimer += Time.deltaTime;
        if (bonusSpawnTimer >= bonusSpawnInterval)
        {
            SpawnSpecialEnemies(bonusSpawnCount);
            bonusSpawnTimer = 0f;
        }


        float enemyCountRatio = (float)activeEnemies.Count / maxEnemyCount;

        // 인스펙터에서 그린 곡선에 따라 비율을 재계산해
        float evaluation = spawnCurve.Evaluate(enemyCountRatio);
        // 선형 보간(Lerp)을 사용해서 간격을 부드럽게 조절해
        spawnInterval = Mathf.Lerp(minSpawnInterval, maxSpawnInterval, evaluation);

        // 3. 스폰 타이머 로직 (기존 코드 유지)
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }

    }
    public void SpawnEnemy()
    {
        // 필드에 자리가 있을 때만 스폰!
        if (activeEnemies.Count >= maxEnemyCount) return;

        GameObject enemy = enemyPool.GetEnemy();
        if (enemy != null)
        {
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().Init(GetRandomSpawnPosition());
            OnEnemySpawned(enemy); // 리스트에 추가!
        }
    }
    void SpawnSpecialEnemies(int count)
    {

        for (int i = 0; i < count; i++)
        {
            if (activeEnemies.Count < maxEnemyCount)
            {
                // 1. 랜덤 스폰 위치 가져오기
                Vector2 spawnPos = GetRandomSpawnPosition();

                // 2. 보너스 적 프리팹 생성
                GameObject specialEnemy = Instantiate(lightTrailEnemyPrefab, spawnPos, Quaternion.identity);

                // 3. Enemy 컴포넌트 가져와서 초기화
                // (상속받았으니 Enemy 타입으로 취급 가능!)
                Enemy enemyScript = specialEnemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.Init(spawnPos);
                    OnEnemySpawned(specialEnemy); // 매니저 리스트에 등록
                }
            }
        }
        Debug.Log($"특수 적 {count}마리 투입");
    }
    public GameObject CreateEnemy()
    {
        GameObject enemy;
        if (activeEnemies.Count < maxEnemyCount)
        {
            enemy = Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            return enemy;
        }
        else
            return null;
    }


    Vector2 GetRandomSpawnPosition()
    {
        // 0: 왼쪽, 1: 오른쪽, 2: 위, 3: 아래
        int side = Random.Range(0, 4);
        Vector2 spawnPos = Vector2.zero;

        switch (side)
        {
            case 0: // 왼쪽 밖
                spawnPos = Camera.main.ViewportToWorldPoint(new Vector2(-0.2f, Random.Range(0f, 1f)));
                break;
            case 1: // 오른쪽 밖
                spawnPos = Camera.main.ViewportToWorldPoint(new Vector2(1.2f, Random.Range(0f, 1f)));
                break;
            case 2: // 위쪽 밖
                spawnPos = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0f, 1f), 1.2f));
                break;
            case 3: // 아래쪽 밖
                spawnPos = Camera.main.ViewportToWorldPoint(new Vector2(Random.Range(0f, 1f), -0.2f));
                break;
        }
        return spawnPos;
    }


    public void OnEnemySpawned(GameObject enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void OnEnemyDespawned(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);
    }

    // 현재 몇 마리인지 궁금할 때
    public int GetActiveEnemyCount() => activeEnemies.Count;
}
