using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��~��Ԃ̓G�����������鏈��
public class EnemyStayOutManager : MonoBehaviour
{
    // �ҋ@��Ԃ�����������EnemyObject���w��
    [SerializeField]
    private Enemy enemy = null;
    // �ҋ@��ԉ���
    public void EnemyStayOut()
    {
        enemy.MoveStart();
    }
}
