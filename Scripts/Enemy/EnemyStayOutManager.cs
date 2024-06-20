using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ’â~ó‘Ô‚Ì“G‚ğŠˆ“®‚³‚¹‚éˆ—
public class EnemyStayOutManager : MonoBehaviour
{
    // ‘Ò‹@ó‘Ô‚ğ‰ğœ‚µ‚½‚¢EnemyObject‚ğw’è
    [SerializeField]
    private Enemy enemy = null;
    // ‘Ò‹@ó‘Ô‰ğœ
    public void EnemyStayOut()
    {
        enemy.MoveStart();
    }
}
