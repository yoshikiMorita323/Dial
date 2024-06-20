using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 金庫などの歳差方法を画面に表示・非表示にする処理
public class OperationManager : MonoBehaviour
{
    // operationImageを指定
    [SerializeField]
    private GameObject[] operationImage = null;

    // 指定しているOperationImageを引数に応じて処理
    public void OnOperation(bool active)
    {
        int i;
        for(i = 0;i < operationImage.Length;i++)
        {
            operationImage[i].SetActive(active);
        }
    }
}
