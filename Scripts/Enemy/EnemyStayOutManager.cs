using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 停止状態の敵を活動させる処理
public class EnemyStayOutManager : MonoBehaviour
{
    // 待機状態を解除したいEnemyObjectを指定
    [SerializeField]
    private Enemy enemy = null;
    // 待機状態解除
    public void EnemyStayOut()
    {
        enemy.MoveStart();
    }
}
