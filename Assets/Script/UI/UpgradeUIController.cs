using System.Collections.Generic;
using UnityEngine;
public class UpgradeUIController : MonoBehaviour
{
    public UpgradeCard[] cards; // 유니티 에디터에서 카드 3개를 드래그해서 넣어줘!

    // 이제 SelectedUpgrade가 아니라 LevelUpOption 리스트를 받아!
    public void Init(List<LevelUpOption> options)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (i < options.Count)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].Setup(options[i]); // 카드에 옵션 정보 전달
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }
    }
}