using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// アイテム所持個数を表示する処理
public class ItemUIManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] stockNomber = null;
    [SerializeField]
    private Image stockImage = null;
    private int stockCount = 0;

    // countの値に応じてstockImageのspriteを変更
    public void AddItem(int count)
    {
        stockCount += count;
        if(stockCount < stockNomber.Length)
        {
            stockImage.sprite = stockNomber[stockCount];
        }
    }
}
