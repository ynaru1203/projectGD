using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private static MaterialPropertyBlock _block; // static으로 만들어서 하나만 공유하자
    private Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_block == null) _block = new MaterialPropertyBlock();
    }

    public void SetEnemyVisual(int sides, Color color)
    {
        // 1. 현재 렌더러의 프로퍼티를 가져와서 (이미 설정된 게 있을 수 있으니)
        _renderer.GetPropertyBlock(_block);

        // 2. 원하는 값을 쪽지(블록)에 적어줘
        // 쉐이더 그래프에서 만든 프로퍼티의 Reference 이름(예: _Sides)을 써야 해!
        _block.SetFloat("_Sides", sides);
        _block.SetColor("_BaseColor", color);

        // 3. 다시 렌더러에게 쪽지를 건네줘
        _renderer.SetPropertyBlock(_block);
    }



}