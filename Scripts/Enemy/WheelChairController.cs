using UnityEngine;

// �G�̏���Ă���Ԃ�������������
public class WheelChairController : MonoBehaviour
{
    // Wheel_L���w��
    [SerializeField]
    private GameObject leftWheel = null;
    // Wheel_R���w��
    [SerializeField]
    private GameObject rightWheel = null;
    //Sub_Wheel���w��
    [SerializeField]
    private GameObject subWheel = null;
    // �e�I�u�W�F�N�g�̃X�s�[�h���P�̎��̃^�C���̉�]�X�s�[�h���w��
    [SerializeField]
    private float wheelRotateSpeed = 50;
    // X���̊p�x���w��
    private float rotateAngleX = 0;
    // �e�I�u�W�F�N�g�̃X�s�[�h���w��
    private float objectSpeed = 2;
    // �Ԃ����̓��쉹���w��
    [SerializeField]
    private AudioSource wheelChairSound = null;

    void Update()
    {
        // �e�I�u�W�F�N�g�������Ă���ꍇ�A�^�C������
        if(objectSpeed > 0)
        {
            rotateAngleX += wheelRotateSpeed * objectSpeed;
            leftWheel.transform.localRotation = Quaternion.Euler(rotateAngleX, 0, 0);
            rightWheel.transform.localRotation = Quaternion.Euler(rotateAngleX, 0, 0);
            subWheel.transform.localRotation = Quaternion.Euler(rotateAngleX, 0, 0);
        }
    }
    // �e�I�u�W�F�N�g�̃X�s�[�h���X�V
    public void InputObjectSpeed(float speed)
    {
        objectSpeed = speed;
        // �e�I�u�W�F�N�g�̃X�s�[�h��0�ɂȂ����ꍇ�A�Ԃ����̓��쉹������
        if(speed == 0)
        {
            wheelChairSound.volume = 0;
        }
        else
        {
            wheelChairSound.volume = 1;
        }
    }
}
