using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ɂȂǂ̍΍����@����ʂɕ\���E��\���ɂ��鏈��
public class OperationManager : MonoBehaviour
{
    // operationImage���w��
    [SerializeField]
    private GameObject[] operationImage = null;

    // �w�肵�Ă���OperationImage�������ɉ����ď���
    public void OnOperation(bool active)
    {
        int i;
        for(i = 0;i < operationImage.Length;i++)
        {
            operationImage[i].SetActive(active);
        }
    }
}
