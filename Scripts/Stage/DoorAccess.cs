using UnityEngine;

// プレイヤーと敵が扉オブジェクトへアクションした際のDoorManagerへの処理移行
public class DoorAccess : MonoBehaviour
{
    // DoorManagerを指定
    [SerializeField]
    private DoorManager doorManager;
    
    // DoorManagerの開閉処理に移行
    public void MoveDoor()
    {
        doorManager.MoveDoorAccess();
    }
}
