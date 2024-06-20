using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���擾���̏���
public class KeyManager : ItemManager
{
    [SerializeField]
    private ItemUIManager itemUIManager = null;

    public void GetKey()
    {
        PickUp();
        Destroy(gameObject, 2);
    }
    // �A�C�e���ǉ�����
    protected override void AddItemStock()
    {
        // Canvas��BottleUI��1���Z
        itemUIManager.AddItem(1);
    }

}
