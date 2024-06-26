using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 鍵取得時の処理
public class KeyManager : ItemManager
{
    [SerializeField]
    private ItemUIManager itemUIManager = null;

    public void GetKey()
    {
        PickUp();
        Destroy(gameObject, 2);
    }
    // アイテム追加処理
    protected override void AddItemStock()
    {
        // CanvasのBottleUIに1加算
        itemUIManager.AddItem(1);
    }

}
