using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public int poolSize = 200; // 미리 만들어둘 개수
    private List<GameObject> pool = new List<GameObject>();
    private EnemyManager spawner;
    void Start()
    {
        spawner = EnemyManager.instance;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = spawner.CreateEnemy();
            if (obj != null)
            {
                obj.SetActive(false); // 처음엔 꺼두기
                pool.Add(obj);
            }
            else
            {
                break;
            }
        }
    }

    public GameObject GetEnemy()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy) // 쉬고 있는 녀석 찾기
            {
                return obj;
            }
        }
        // 만약 창고가 비었다면 새로 하나 더 만들기 (유동적 확장)
        GameObject newObj = spawner.CreateEnemy();
        pool.Add(newObj);
        return newObj;
    }
}