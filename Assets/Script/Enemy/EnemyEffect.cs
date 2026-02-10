using UnityEngine;

public class EnemyEffect : MonoBehaviour
{
    [Header("이펙트 설정")]
    public int emitCount = 10;       // 한 번 맞을 때 튀어나올 알갱이 수
    private ParticleSystem hitEffect; // 자식으로 있는 반전 파티클
    private void Awake()
    {
        hitEffect = GetComponent<ParticleSystem>();
    }
    public void OnTakeDamage()
    {
        // 맞을 때마다 즉시 파티클을 뿜어내! 
        // 이렇게 하면 연속으로 맞아도 뿜어져 나오는 양이 늘어나서 훨씬 찰져.
        if (hitEffect != null)
        {
            // Stop이나 Play 대신 Emit을 쓰면 연타 대응이 가능해
            hitEffect.Emit(emitCount);
        }

    }
}
