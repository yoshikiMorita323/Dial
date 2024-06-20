using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �A�C�e����������\�����鏈��
public class ItemUIManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] stockNomber = null;
    [SerializeField]
    private Image stockImage = null;
    private int stockCount = 0;

    // count�̒l�ɉ�����stockImage��sprite��ύX
    public void AddItem(int count)
    {
        stockCount += count;
        if(stockCount < stockNomber.Length)
        {
            stockImage.sprite = stockNomber[stockCount];
        }
    }
}
