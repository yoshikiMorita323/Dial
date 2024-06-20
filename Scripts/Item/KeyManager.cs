using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Œ®æ“¾‚Ìˆ—
public class KeyManager : ItemManager
{
    [SerializeField]
    private ItemUIManager itemUIManager = null;

    public void GetKey()
    {
        PickUp();
        Destroy(gameObject, 2);
    }
    // ƒAƒCƒeƒ€’Ç‰Áˆ—
    protected override void AddItemStock()
    {
        // Canvas‚ÌBottleUI‚É1‰ÁZ
        itemUIManager.AddItem(1);
    }

}
