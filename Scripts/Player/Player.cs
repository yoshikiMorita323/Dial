using UnityEngine;
using UnityEngine.InputSystem;

// ���[�U�[���͂���̃v���C���[�L�����N�^�[�̑����\���܂��B
[RequireComponent(typeof(MoveBehaviour), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    // �J�����̈ʒu���w�肵�܂��B
    [SerializeField] 
    private Transform cameraPoint = null;
    // spotLight���w�肵�܂��B
    [SerializeField]
    private Transform spotLight = null;
    // ���C�g�̃X�N���v�g���w�肵�܂�
    [SerializeField]
    private Flashlight flashLight = null;
    // ���C�g�̃A�j���[�^�[���w��
    [SerializeField]
    private Animator flashLightAnimator = null;
    [SerializeField]
    private Animator stageSceneAnimator = null;
    // flashLightAnimator�𓮍삳���Ă��邩�ۂ�
    private bool onFlashLightanimator = false;

    // �w�肵�����ɂ��i�[���܂��B
    private GameObject safeObject;
    // �w�肵�����ɂ�SafeManager�X�N���v�g���i�[���܂��B
    private SafeManager safeManager;
    // ���ɂ̃_�C������ᑬ�ŉ񂷂��𔻒�
    private bool dialLowSpeed = false;
    // �擾�������̐����w��
    private int keyCount = 0;
    // �G�̈ʒu�����i�[
    private Vector3 enemyPosition = Vector3.zero;

    MoveBehaviour moveBehaviour;
    CenterViewManager centerViewManager;
    ThrowingManager throwingManager;
    HeartbeatManager heartbeatManager;
    AudioSource audioSource;
    PlayerInput playerInput;
    Animator animator;
    Gamepad gamepad;

    // �ړ��L�[�̓��͂��i�[
    Vector2 moveInput = Vector2.zero;
    Vector2 lookInput = Vector2.zero;
    // �J�����̃s�b�`���i�[
    private float cameraPitch = 0;
    // ���Ⴊ��ł��邩����
    private bool squatDown = false;

    enum PlayerState
    {
        // �ʏ���
        Idle,
        // ����
        Run,
        // ���Ⴊ��
        Squat,
        // ���ɑ�����
        SafeControl,
        // �|�[�Y
        Pause,
        // �G�ɕ߂܂�
        GetCaught,
        // �Q�[���I�[�o�[
        GameOver,
        // �Q�[���N���A�[
        GameClear,
    }
    [SerializeField]
    PlayerState currentState = PlayerState.Idle;
    // �|�[�Y��ʂɈڍs���钼�O��currentState���w��
    PlayerState pauseOnState = PlayerState.Idle;
    // ���s���̑��x���w��
    [SerializeField]
    private float walkSpeed = 3;
    // ���s���̑��x���w��
    [SerializeField]
    private float runSpeed = 5;
    // ���Ⴊ�ݎ��̑��x���w��
    [SerializeField]
    private float squatSpeed = 2;

    #region SoundOption
    // ������AudioSource���w��
    [SerializeField]
    private AudioSource footStepsOnShound = null;
    // �ċz����AudioSource���w��
    [SerializeField]
    private AudioSource shortBreathOnSound = null;
    // �_���[�W���̉����w��
    [SerializeField]
    private AudioClip damageOnSound = null;
    // �_���[�W���̉��ʂ��w��
    [SerializeField]
    private float damageVolume = 1;
    // ���s����shortBreathOnSound�̃{�����[�����w��
    [SerializeField]
    private float shortBreathVolume = 0.02f;
    private bool onAudio = false;

    [SerializeField]
    private SphereCollider soundArea = null;
    [SerializeField]
    private float runSoundArea = 8;
    [SerializeField]
    private float walkSoundArea = 4;
    [SerializeField]
    private float squatSoundArea = 0.1f;
    [SerializeField]
    private float runAudioPitchSize = 1.3f;
    [SerializeField]
    private float walkAudioPitchSize = 0.6f;
    [SerializeField]
    private float squatAudioPitchSize = 0.6f;
    [SerializeField]
    private float runVolume = 1;
    [SerializeField]
    private float walkVolume = 0.6f;
    [SerializeField]
    private float squatVolume = 0.2f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        moveBehaviour = GetComponent<MoveBehaviour>();
        centerViewManager = GetComponent<CenterViewManager>();
        throwingManager = GetComponent<ThrowingManager>();
        heartbeatManager = GetComponent<HeartbeatManager>();
        audioSource = GetComponent<AudioSource>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        // �ړ����x����s���x�ɕύX
        moveBehaviour.MoveSpeedManager(walkSpeed);
        // �}�E�X�J�[�\�����\���ɂ���
        Cursor.visible = false;
        // �}�E�X�J�[�\������ʒ����ɌŒ肷��
        Cursor.lockState = CursorLockMode.Locked;
    }

    public bool IsMouse { get { return playerInput.currentControlScheme == "Keyboard&Mouse"; } }
    
    // �ړ�����F WASD or ���X�e�B�b�N
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>().normalized;
        }
        else if (context.canceled)
        {
            moveInput = Vector2.zero;
        }

        switch(currentState)
        {
            case PlayerState.GameOver:
                GameOver.Instance.moveInput = moveInput;
                break;
            case PlayerState.Pause:
                Pause.Instance.moveInput = moveInput;
                break;
        }
    }
    // ���_����G�}�E�X���� or �E�X�e�B�b�N
    public void OnLook(InputAction.CallbackContext context)
    {
        if(!moveBehaviour.rotateStop)
        {
            lookInput = context.ReadValue<Vector2>();
            // �R���g���[���[�̏ꍇ
            if (!IsMouse)
            {
                lookInput *= Time.deltaTime;
            }
        }
    }
    // �E�X�e�B�b�N����X�V����
    private void Look()
    {
        if(lookInput.sqrMagnitude > 0.01f)
        {
            // �L�����N�^�[�̕�������]������
            moveBehaviour.Rotate(lookInput.x);
            // ���_�̃s�b�`�p����]
            cameraPitch += lookInput.y * moveBehaviour.RotationSpeed;
            // ���_�̃s�b�`�p����]
            cameraPitch += lookInput.y * moveBehaviour.RotationSpeed * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
            cameraPoint.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        }
    }

    // ����FLShift or L2
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed && currentState == PlayerState.Idle)
        {
            // �X�e�[�g�J�� �i����j
            SetRunState();
        }
        else if (context.canceled && currentState == PlayerState.Run)
        {
            shortBreathOnSound.Stop();
            // �X�e�[�g�J�� �i�ʏ�̏�ԁj
            SetIdleState();
            // flashLightAnimation���p�����̏ꍇ�AIdleAnimation�Ɉڍs�A�p�x�������l�ɖ߂�
            if (onFlashLightanimator)
            {
                flashLightAnimator.SetTrigger("Idle");
                flashLightAnimator.enabled = false;
                spotLight.transform.localRotation = Quaternion.identity;
                onFlashLightanimator = false;
            }
        }
    }


    // ���Ⴊ�ށ@or ���ɑ��쎞�A�_�C�����ᑬ��]�G Contrl or  L1�g���K�[
    public void OnSquat(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch(currentState)
            {
                case PlayerState.Idle:
                case PlayerState.Run:
                    //  �X�e�[�g�J�ځi���Ⴊ�ށj
                    SetSquatState();
                    break;
                case PlayerState.SafeControl:
                    // �_�C������]���x��ᑬ�ɍX�V
                    dialLowSpeed = true;
                    safeManager.LowSpeedDial(dialLowSpeed);
                    break;
            }
        }
        else if (context.canceled)
        {
            switch (currentState)
            {
                case PlayerState.Squat:
                    // �X�e�[�g�J�ځi�ʏ�̏�ԁj
                    SetIdleState();
                    squatDown = false;
                    break;
                case PlayerState.SafeControl:
                    // �_�C������]���x��ʏ푬�x�ɍX�V
                    dialLowSpeed = false;
                    safeManager.LowSpeedDial(dialLowSpeed);
                    break;
            }
        }
    }

    // ���C�gON�AOFF�FSpace or R3
    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            flashLight.LightSwitch();
        }
    }

    // �N���X�w�A�ɂ��A�N�V�����F���N���b�N or SouthButton
    public void OnAction(InputAction.CallbackContext context)     
    {
        if (context.performed)
        {
            Action();
        }
    }

    private void Action()
    {
        // ray�̓������Ă���R���C�_�[�ɉ����ď������s��
        switch (centerViewManager.objectLayer)
        {
            case 7:     // �r��
                GameObject bottle = centerViewManager.hitCrossHairRay.collider.gameObject;
                        // �r�����擾
                bottle.GetComponent<Bottle>().PickUpBottle();
                        // ���Ă��o����r���̖{�������Z
                throwingManager.BottleStorage(bottle);
                break;
            case 13:    // ����
                if (currentState != PlayerState.SafeControl
                    & currentState != PlayerState.Pause)
                {
                        // ���ɂ̑���Ɉڍs
                    SetSafeControlState();
                    return;
                }
                break;
            case 21:    // �܂��Ă���h�A
            case 22:    // �J���Ă���h�A
                GameObject door = centerViewManager.hitCrossHairRay.collider.gameObject;
                        // �h�A�̊J�󋵂ɂ��킹�ē�����
                door.GetComponent<DoorAccess>().MoveDoor();
                break;
            case 24:    // ��
                GameObject key = centerViewManager.hitCrossHairRay.collider.gameObject;
                        // �����擾
                key.GetComponent<KeyManager>().GetKey();
                keyCount++;
                break;
            case 28:    // ��1���J��
            case 29:    // ��2���J��
            case 30:    // ��3���J��
                GameObject goolLock = centerViewManager.hitCrossHairRay.collider.gameObject;
                        // ���̎擾���ɉ����ē������������
                goolLock.GetComponent<LockKeyManager>().LockOpen(keyCount);
                break;
            default:
                break;
        }
        // �X�e�[�g�ɉ����ď���
        switch(currentState)
        {
            case PlayerState.SafeControl:   // ���ɑ��쒆
                // ���ɑ������
                LiftSafeControl();
                // �X�e�[�g�J�ځi�ʏ�̏�ԁj
                SetIdleState();
                break;
            case PlayerState.GameClear:
                StageClear.Instance.action = true;
                break;
            case PlayerState.GameOver:
                GameOver.Instance.action = true;
                break;
            case PlayerState.Pause:
                Pause.Instance.action = true;
                break;
        }
    }

    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // �X�e�[�g�ɉ����ď���
            switch (currentState)
            {
                case PlayerState.GameClear:
                    StageClear.Instance.action = true;
                    break;
                case PlayerState.GameOver:
                    GameOver.Instance.action = true;
                    break;
                case PlayerState.Pause:
                    Pause.Instance.action = true;
                    break;
            }
        }
    }

    // ���ɂ̃_�C���������ɉ�]
    public void OnSafeDialLeftRotate(InputAction.CallbackContext context)
    {
        if (currentState == PlayerState.SafeControl)
        {
            float move = -1 * context.ReadValue<float>();
            if (move > -0.1f)
            {
                move = 0;
            }
            safeManager.OnDialeRotation(new Vector2(move, 0));
        }
    }
    // ���ɂ̃_�C�������E�ɉ�]
    public void OnSafeDialRightRotate(InputAction.CallbackContext context)
    {
        if (currentState == PlayerState.SafeControl)
        {
            float move = context.ReadValue<float>();
            if (move < 0.1f)
            {
                move = 0;
            }
            safeManager.OnDialeRotation(new Vector2(move, 0));
        }

    }

    // ���Ă�: �E�N���b�N or R1�g���K�[
    public void OnThrowing(InputAction.CallbackContext context)
    {
        // �r���̏�������1�ȏ�̎��A�r���𓊂���
        if(context.performed && throwingManager.StorageCount > 0)
        {
            throwingManager.ThrowingController();
        }
    }

    // �o�b�e���[�[�d�FQ or R2�g���K�[
    public void OnChage(InputAction.CallbackContext context)
    {
        switch(currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Run:
            case PlayerState.Squat:

                // ���������̓o�b�e���[�[�d
                if (context.performed)
                {
                    flashLight.OnCharge = true;
                }
                else if (context.canceled)      // �[�d����
                {
                    flashLight.OnCharge = false;
                }
                break;
        }
    }

    // �J���FE or South�{�^��
    public void OnSafeOpen(InputAction.CallbackContext context)
    {
        if (context.performed && currentState == PlayerState.SafeControl)
        {
           safeManager.SafeOpenTry();
        }
    }

    // �|�[�Y��ʐ؂�ւ��GESC or opsion
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (currentState)
            {
                case PlayerState.Idle:
                case PlayerState.Run:
                case PlayerState.Squat:
                case PlayerState.SafeControl:
                    // �X�e�[�g�J�ځi�|�[�Y�j
                    SetPauseState();
                    break;
                case PlayerState.Pause:
                    // �X�e�[�g�J�ځi�ʏ�̏�ԁj
                    PauseOutState();
                    // �|�[�Y��ʉ���
                    Pause.Instance.ButtonNo = 0;
                    Pause.Instance.action = true;
                    break;
            }
        }
    }
    // �ʏ�̏��
    private void SetIdleState()
    {
        // AudioSource����s���̃X�e�[�^�X�ɕύX
        AudioSourceManager(footStepsOnShound, walkAudioPitchSize, walkVolume);
        // ������������͈͂���s���͈̔͂ɕύX
        soundArea.radius = walkSoundArea;
        // �ړ����x��ύX
        moveBehaviour.MoveSpeedManager(walkSpeed);
        currentState = PlayerState.Idle;
    }

    // ����
    private void SetRunState()
    {
        // AudioSource�𑖍s���̃X�e�[�^�X�ɕύX
        AudioSourceManager(footStepsOnShound, runAudioPitchSize, runVolume);
        shortBreathOnSound.Play();
        // ������������͈͂𑖍s���͈̔͂ɕύX
        soundArea.radius = runSoundArea;
        // �ړ����x��ύX
        moveBehaviour.MoveSpeedManager(runSpeed);
        // �X�e�[�g�J�ځi����j
        currentState = PlayerState.Run;
    }

    // ���Ⴊ��
    private void SetSquatState()
    {
        // AudioSource�����Ⴊ�ݎ��̃X�e�[�^�X�ɕύX
        AudioSourceManager(footStepsOnShound, squatAudioPitchSize, squatVolume);
        // ������������͈͂����Ⴊ�ݎ��͈̔͂ɕύX
        soundArea.radius = squatSoundArea;
        squatDown = true;
        // �ړ����x��ύX
        moveBehaviour.MoveSpeedManager(squatSpeed);
        currentState = PlayerState.Squat;
    }

    // ���ɑ���
    private void SetSafeControlState()
    {
        // player�̉�]���~����
        moveBehaviour.rotateStop = true;
        // �w�肵�Ă�����ɃI�u�W�F�N�g���i�[
        safeObject = centerViewManager.hitCrossHairRay.collider.gameObject;
        // �w�肵�Ă�����ɂ�SafeManager�X�N���v�g���i�[
        safeManager = safeObject.GetComponent<SafeManager>();
        // �w�肵�����ɂ𑀍삷��
        safeManager.OnSafeControl();
        // spotLight�̈ʒu�y�ъp�x�����ɂ̃��C�g�Œ�ʂɕύX
        spotLight.transform.position = safeManager.spotLitePosition.transform.position;
        spotLight.transform.rotation = safeManager.spotLitePosition.transform.rotation;

        // �N���X�w�A��Ray�@�\���~���ăe�L�X�g���\���ɂ���
        centerViewManager.StopRay();
        // �X�e�[�g�J��
        currentState = PlayerState.SafeControl;
        // �ړ����x��ύX
        moveBehaviour.MoveSpeedManager(0);
    }

    // ���ɑ��샂�[�h������
    private void LiftSafeControl()
    {
        // player�̉�]��~��Ԃ���������
        moveBehaviour.rotateStop = false;
        // ���ɂ̑�����~����
        safeManager.OffSafeControl();
        // spotLight�̈ʒu�y�ъp�x��ʏ�̈ʒu�ɖ߂�
        spotLight.transform.localPosition = Vector3.zero;
        spotLight.transform.localRotation = Quaternion.identity;
        // �N���X�w�A��Ray�@�\���N�����ăe�L�X�g��\������悤�ɂ���
        centerViewManager.StartUpRay();
        safeObject = null;
    }

    // �|�[�Y
    private void SetPauseState()
    {
        // �|�[�Y��ʂɈڍs���钼�O�̃X�e�[�g���i�[
        pauseOnState = currentState;
        // �v���C���[�̉�]���~
        moveBehaviour.rotateStop = true;
        // �|�[�Y��ʂ�\��
        StageScene.Instance.Pause();
        // �X�e�[�g�J�ځi�|�[�Y�j
        currentState = PlayerState.Pause;
    }

    // �|�[�Y��ʂ���Q�[�����p������ꍇ�̏���
    private void PauseOutState()
    {
        // �v���C���[�̉�]��L���ɂ���
        moveBehaviour.rotateStop = false;

        switch (pauseOnState)
        {
            case PlayerState.Idle:
            case PlayerState.Run:
            case PlayerState.Squat:
                SetIdleState();
                break;
            case PlayerState.SafeControl:
                currentState = PlayerState.SafeControl;
                break;
        }
    }
    // �G�ɕ߂܂�
    private void SetGetCaughtState()
    {
        // �}�E�X�J�[�\���ł̉�]�𖳌��ɂ���
        moveBehaviour.rotateStop = true;
        // �_���[�W����炷
        audioSource.PlayOneShot(damageOnSound, damageVolume);
        // �A�j���[�V�����̑J��
        stageSceneAnimator.SetTrigger("Damage");
        flashLightAnimator.enabled = false;
        // rigidbody��isKinematic��true�ɂ���
        moveBehaviour.BodyKinematic();
        // �G�̕�������������
        LookManager(enemyPosition);
        // �ړ����~
        moveBehaviour.MoveSpeedManager(0);
        // �������~
        footStepsOnShound.Stop();
        // �X�e�[�g�J�ځi�߂܂�j
        currentState = PlayerState.GetCaught;
    }
    //�@�Q�[���I�[�o�[
    public void SetGameOverState()
    {
        StageScene.Instance.GameOver();
        heartbeatManager.Damage = true;
        currentState = PlayerState.GameOver;
    }
    // �Q�[���N���A
    private void SetGemeClearState()
    {
        moveBehaviour.MoveSpeedManager(0);
        StageScene.Instance.StageClear();
        currentState = PlayerState.GameClear;
    }

    void Update()
    {
        // �ړ����͂��v���C���[�̈ړ��ɔ��f
        moveBehaviour.Move(moveInput);
        // �������͂��v���C���[�̎��_�ړ��ɔ��f
        Look();
        // ���Ⴊ�ݓ��͂𔽉f
        moveBehaviour.SquatBodyManager(squatDown);
        // �v���C���[�̑̓��ɂ��e��������
        MoveManager();
        // �|�[�Y�������̃X�e�[�g�J��
        if(currentState == PlayerState.Pause)
        {
            if(Time.timeScale == 1)
            {
                PauseOutState();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // �Q�[���N���A�[���̏���
        if(other.gameObject.CompareTag("GameClear"))
        {
            SetGemeClearState();
        }

        // �G�ɓ��������ۂ̏���
        if (other.gameObject.CompareTag("CatchArea"))
        {
            enemyPosition = other.transform.position;
            SetGetCaughtState();
        }
    }
    // AudioSource���̃X�e�[�^�X��ύX
    private void AudioSourceManager(AudioSource audioName,float pitchSize,float volumeSize)
    {
        audioName.pitch = pitchSize;
        audioName.volume = volumeSize;
    }

    // �v���C���[�̑̓��ɂ��e��������
    private void MoveManager()
    {
        // �ړ����͂̑傫�����i�[
        float move = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);
        // �ړ����Ă���ۂ̏���
        if (move >= 0.1f)
        {
            // ��~���Ă����Ԃ��珉�߂Ĉړ������ꍇ
            if(!onAudio)
            {
                switch (currentState)
                {
                    case PlayerState.Idle:
                    case PlayerState.Run:
                    case PlayerState.Squat:

                        onAudio = true;
                        soundArea.enabled = true;
                        footStepsOnShound.Play();
                        break;
                }
            }
            // �����Ă���ۂ̊e�ݒ���X�V
            if (currentState == PlayerState.Run)
            {
                // �ċz�����Đ�
                if (shortBreathOnSound.volume == 0)
                {
                    shortBreathOnSound.volume = shortBreathVolume;
                }
                // flashLightAnimation���p�����̏ꍇ�AIdleAnimation�ɖ߂�
                if (!onFlashLightanimator)
                {
                    flashLightAnimator.enabled = true;
                    flashLightAnimator.SetTrigger("Run");
                    onFlashLightanimator = true;
                }
            }
        }
        // ��~���Ă���ꍇ
        else if(move < 0.1f)
        {
            // �ړ����Ă����Ԃ����~�����ۂ̏���
            if(onAudio)
            {
                onAudio = false;
                soundArea.enabled = false;
                footStepsOnShound.Stop();
            }
            // �����Ă���ۂ̌ċz�����~
            if (shortBreathOnSound.volume > 0)
            {
                shortBreathOnSound.volume = 0;
                // flashLightAnimation���p�����̏ꍇ�AIdleAnimation�Ɉڍs�A�p�x�������l�ɖ߂�
                if (onFlashLightanimator)
                {
                    flashLightAnimator.SetTrigger("Idle");
                    flashLightAnimator.enabled = false;
                    spotLight.transform.localRotation = Quaternion.identity;
                    onFlashLightanimator = false;
                }
            }
        }
    }

    // ����̕������������鏈��
    private void LookManager(Vector3 partnerPosition)
    {
        Vector3 distanceVector = (partnerPosition - transform.position).normalized;
        float angleY = (Mathf.Atan2(distanceVector.x, distanceVector.z) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angleY, transform.eulerAngles.z);
    }
}
