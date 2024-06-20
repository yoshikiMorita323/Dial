using UnityEngine;
using UnityEngine.InputSystem;

// ���ɂ̃_�C������]�A������ԍ��̐ݒ�A���쎞�A����������̋�������
public class SafeManager : MonoBehaviour
{
    new Collider collider;
    Gamepad gamepad;
    SafeAnimation safeAnimation;

    // �p�b�h�̐U�����[�^�[��]���x���w��
    [SerializeField]
    private float openDialPadMotorSpeed = 1;
    // �p�b�h�̐U�����Ԃ��w��
    [SerializeField]
    private float motorMoveTime = 0.1f;
    private float onMotorMoveTimeCount = 0;
    // �p�b�h���U�������邩����
    private bool onMotorMove = false;
    private int motorMoveCount = 0;
    // ���ɑ��쎞�̃J�������w��
    [SerializeField]
    private GameObject safeCamera = null;
    [SerializeField]
    private GameObject dial = null;
    // crossHairImage���w��
    [SerializeField]
    private GameObject crossHairImage = null;
    // �������s���̉��͈̔͂��w��
    [SerializeField]
    private GameObject safeSoundArea = null;
    //  safeSoundArea�̏o�����Ԃ��w��
    [SerializeField]
    private float safeNotOpenSoundTime = 0.8f;
    // SafeOperation���w��
    [SerializeField]
    private OperationManager safeOperationManager;
    // SpotLitePosition���w��
    public Transform spotLitePosition = null; 

    private float soundAreaOntimeCount = 0;
    // �J���܂ł̒i�K�����w��
    [SerializeField]
    private int openStepCount = 2;
    // ���݂̒i�K���w��
    private int stepCount = 0;
    private bool notOpen = false;
    // �J���ԍ��������_���ɂ��邩
    [SerializeField]
    private bool randomNo = true;
    // �J���ԍ����w��
   [SerializeField]
    private float openKeyNo = 0;
    // �_�C�����ʒu�̊p�x���w��
    private float rotateAngle = 0;
    // ��]�����Ă���p�x���i�[
    private float angleX = 0;
    // �_�C�����ʒu�̊p�x�͈͂��w��
    private float keyAreaAngle = 0;
    // �J���ԍ��ʒu�Ƃ̊p�x�����w��
    private float angleDifference = 0;
    // �_�C�����̍ő�ԍ����w��
    [SerializeField]
    private int maxKeyNo = 15;
    // �_�C�����̉�]���x���w��
    [SerializeField]
    private float moveRotateSpeed = 50;
    [SerializeField]
    private float rotateLowSpeedDouble = 0.5f;
    private float speedGear = 1;

    // �ړ����͒l���i�[
    private Vector2 moveInput = Vector2.zero;
    // ���ɂ𑀍삵�Ă��邩�ۂ�����
    private bool control = false;
    // ���݂̊p�x��������̊p�x�͈͓����ۂ�����
    private bool safeOpen = false;

    AudioSource audioSource;

    #region Audio
    // �_�C�������񂵂Ă��鎞�̉����w��
    [SerializeField]
    private AudioSource safeRotationAudio = null;
    [SerializeField]
    private float minSafePich = 2.5f;
    // ���ɂ��J�������ۂ̉����w��
    [SerializeField]
    private AudioClip openOnSound = null;
    // �J�������s�����ۂ̉����w��
    [SerializeField]
    private AudioClip notOpenOnSound = null;
    // ���s���̉��ʂ��w��
    [SerializeField]
    private float notOpenOnSoundVolume = 1;
    // �i�K���Ƃ̉����������ۂ̉����w��
    [SerializeField]
    private AudioClip unlockOnSound = null;
    // RotationAudio�̃{�����[�����擾
    public float rotationVolume = 0;
    // RotationAudio��pitch���擾
    public float rotationPanStereo = 0;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<Collider>();
        gamepad = Gamepad.current;
        safeAnimation = GetComponent<SafeAnimation>();

        if (randomNo)
        {
            // �J���ԍ��������_���Ŏw��
            openKeyNo = Random.Range(0, maxKeyNo + 1);
        }
        // �J���ԍ��̊p�x���w��
        keyAreaAngle = (360 / maxKeyNo) * openKeyNo;
    }

    public void OnDialeRotation(Vector2 move)
    {
        moveInput = move;
    }

    public void LowSpeedDial(bool dialLowSpeed)
    {
        if(dialLowSpeed)
        {
            speedGear = rotateLowSpeedDouble;
        }
        else
        {
            speedGear = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(control)
        {
            SafeOperation();
        }
        // �J���Ȃ������ꍇ�̉��͈̔͂ł���sadeSoundArea����莞�ԃA�N�e�B�u�ɂ���
        if (notOpen)
        {
            safeSoundArea.SetActive(true);
            soundAreaOntimeCount += Time.deltaTime;
            if (soundAreaOntimeCount >= safeNotOpenSoundTime)
            {
                safeSoundArea.SetActive(false);
                notOpen = false;
                soundAreaOntimeCount = 0;
            }
        }

    }
    // ���ɑ���J�n
    public void OnSafeControl()
    {
        safeCamera.SetActive(true);
        crossHairImage.SetActive(false);
        safeRotationAudio.Play();
        safeOperationManager.OnOperation(true);
        control = true;
    }
    // ���ɑ���I��
    public void OffSafeControl()
    {
        safeRotationAudio.Stop();
        safeCamera.SetActive(false);
        crossHairImage.SetActive(true);
        safeOperationManager.OnOperation(false);
        control = false;
        moveInput = Vector2.zero;
    }

    private void SafeOperation()
    {
        // �_�C�����̊p�x��0�`360�x�͈̔͂ɏC��
        var dialAngle = angleX % 360;
        if (angleX < 0)
        {
            dialAngle = Mathf.Abs(angleX % 360 + 360);
        }

        // �_�C��������
        if (Mathf.Abs(moveInput.x) >= 0.1f)
        {
            // �J���ԍ��ʒu�ƌ��݂̃_�C�����ԍ��ʒu�̍���0�`359�܂ł̒l�Ŏ擾
            angleDifference = (dialAngle - keyAreaAngle) % 360;
            if (angleDifference < 0)
            {
                angleDifference = (dialAngle - keyAreaAngle) % 360 + 360;
            }
            // �_�C������]���x�ɉ����ă_�C������]����pitch��ύX
            safeRotationAudio.pitch = Mathf.Abs(moveInput.x * speedGear) / 2 + minSafePich;

            if((angleDifference <= 5) || (angleDifference >= 355))
            {
                safeRotationAudio.volume = 1;
            }
            else if(angleDifference <= 15 || angleDifference >= 345)
            {
                safeRotationAudio.volume = 0.8f;
            }
            else if(angleDifference <= 30 || angleDifference >= 320)
            {
                safeRotationAudio.volume = 0.7f;
            }
            else if(angleDifference <= 90 || angleDifference >= 270)
            {
                safeRotationAudio.volume = 0.4f;
            }
            else
            {
                safeRotationAudio.volume = 0.2f;
            }
            // �_�C�����̈ʒu�ɉ�����StereoPan�𑀍�
            AudioSourceStereoPan();
            // �_�C��������]������ƂƂ��ɁAangleX�Ɍ��݂̃_�C������]�p�x���L�^
            rotateAngle += moveRotateSpeed * speedGear * moveInput.x * Time.deltaTime;
            angleX += rotateAngle;
            dial.transform.rotation *= Quaternion.Euler(0, 0, rotateAngle);
            rotateAngle = 0;
        }
        else
        {
            safeRotationAudio.volume = 0;
        }

        // ������p�x����ׂ荇�킹�̔ԍ��ɂȂ�܂ł͈̔͂��w��
        float angleRange = 360 / maxKeyNo;
        // ������ԍ��̊p�x�͈͂ɓ������ۂ̏���
        if((dialAngle >= (keyAreaAngle - (angleRange / 2)))
            && (dialAngle <= (keyAreaAngle + (angleRange / 2))))
        {
            safeOpen = true;
            // �Q�[���p�b�h�g�p���̏���
            if(gamepad != null)
            {
                // ���݂̃_�C�����ʒu���Q�[���p�b�h�̃��[�^�[�쓮�ʒu�ɓ������ꍇ�̏���
                if(Mathf.Abs(dialAngle - keyAreaAngle) <= 1)
                {
                    if(!onMotorMove && motorMoveCount == 0)
                    {
                        motorMoveCount++;
                        onMotorMoveTimeCount = 0;
                        onMotorMove = true;
                    }
                }
                else if(motorMoveCount > 0)
                {
                    motorMoveCount = 0;
                }
            }
        }
        else
        {
            safeOpen = false;
        }

        // �����̃_�C�����ԍ��ɋ߂��ꍇ�A�Q�[���p�b�h�̃��[�^�[�𑀍삵�܂��B
        if(onMotorMove)
        {
            OnGamePadMoter();
        }
    }
    // �Q�[���p�b�h�̃��[�^�[����
    private void OnGamePadMoter()
    {
        // �o�ߎ��Ԃ����Z
        onMotorMoveTimeCount += Time.deltaTime;
        // ���[�^�[���ғ�������
        if ((onMotorMoveTimeCount < motorMoveTime) && motorMoveCount == 1)
        {
            motorMoveCount++;
            gamepad.SetMotorSpeeds(openDialPadMotorSpeed, openDialPadMotorSpeed);
        }
        else if(onMotorMoveTimeCount > motorMoveTime) // �w�莞�ԊO�ɂȂ����烂�[�^�[���~
        {
            gamepad.SetMotorSpeeds(0, 0);
            onMotorMove = false;
        }
    }
    // ���ɂ��J���鑀��������ۂ̏���
    public void SafeOpenTry()
    {
        if(control && !notOpen)
        {
            // �_�C������������̈ʒu�ɂ���ꍇ
            if (safeOpen)
            {
                stepCount++;
                safeAnimation.NextAnimator(stepCount, openStepCount);

                // �J��
                if(stepCount == openStepCount)
                {
                    audioSource.PlayOneShot(openOnSound, 0.1f);
                    collider.enabled = false;
                }
                else if(stepCount < openStepCount)�@// ���̃_�C�����ʒu��
                {
                    audioSource.PlayOneShot(unlockOnSound, 1);
                    // �J���ԍ����ēx�����_���Ŏw��
                    if (randomNo)
                    {
                        openKeyNo = Random.Range(0, maxKeyNo + 1);
                    }
                    // ������ԍ��̊p�x���w��
                    keyAreaAngle = (360 / maxKeyNo) * openKeyNo;
                }
            }
            else
            {
                audioSource.PlayOneShot(notOpenOnSound, notOpenOnSoundVolume);
                notOpen = true;
            }
        }
    }
    // safeRotationAudio��StereoPan�𑀍�
    public void AudioSourceStereoPan()
    {
        if(angleDifference < 360 && angleDifference >= 270)
        {
            safeRotationAudio.panStereo =  -4 + (angleDifference / 90);
        }
        else if(angleDifference > 180 && angleDifference <= 270)
        {
            safeRotationAudio.panStereo = 2 - (angleDifference / 90);
        }
        else if(angleDifference < 180 && angleDifference >= 90)
        {
            safeRotationAudio.panStereo = 2 - (angleDifference / 90);
        }
        else if(angleDifference > 0 && angleDifference <= 90)
        {
            safeRotationAudio.panStereo = angleDifference / 90;
        }
        else
        {
            safeRotationAudio.panStereo = 0;
        }
        rotationPanStereo = safeRotationAudio.panStereo;
    }
}
