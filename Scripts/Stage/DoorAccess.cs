using UnityEngine;

// �v���C���[�ƓG�����I�u�W�F�N�g�փA�N�V���������ۂ�DoorManager�ւ̏����ڍs
public class DoorAccess : MonoBehaviour
{
    // DoorManager���w��
    [SerializeField]
    private DoorManager doorManager;
    
    // DoorManager�̊J�����Ɉڍs
    public void MoveDoor()
    {
        doorManager.MoveDoorAccess();
    }
}
