using UnityEngine;

[RequireComponent(typeof(SearchManager))]
[RequireComponent(typeof(NaviMesh))]

// �G�̋�������
public class Enemy : MonoBehaviour
{
    SearchManager searchManager;
    NaviMesh naviMesh;
    AudioSource audioSource;

    // WheelChairController�X�N���v�g���w��
    [SerializeField]
    private WheelChairController wheelChairController = null;
    // EnemyModel���w��
    [SerializeField]
    private EnemyModel enemyModel = null;
    // player�̈ʒu�����擾
    [SerializeField]
    private Transform player = null;
    // playerScript���w��
    [SerializeField]
    private Player playerScript = null;
    [SerializeField]
    private float currentTime = 0;
    // �������Ă���ʏ��ԂɑJ�ڂ���܂ł̎��Ԃ��w��
    [SerializeField]
    private float loseSightTime = 2;
    // �x����Ԃ���x����������������Ă���ʏ��Ԃɖ߂�܂ł̎��Ԃ��w��
    [SerializeField]
    private float warningTime = 1;
    // �v���C���[��߂܂��Ă���Q�[���I�[�o�[�ɂȂ�܂ł̎��Ԃ��w��B
    [SerializeField]
    private float onGameOverTime = 2;
    // ��Ԃɂ��킹���ړ����x���w��
    [SerializeField]
    private float runSpeed = 3.5f;
    [SerializeField]
    private float walkSpeed = 2;

    // �v���C���[�̃��C���[�ԍ����w��
    [SerializeField]
    private int playerLayerNomber = 0;
    [SerializeField]
    private LayerMask playerLayerMask = default;
    // �������Ώۂ�Layer���w��
    [SerializeField]
    private LayerMask hearingSoundLayer = default;
    // ���̂Ȃ����ʒu���i�[
    [SerializeField]
    private Vector3 soundPosition = Vector3.zero;
    // �ړI�n���w��
    [SerializeField]
    private Vector3 targetPosition = Vector3.zero;

    // LookSearchArea���w��
    [SerializeField]
    private Transform lookSearchArea = null;
    // ChatchArea���w��
    [SerializeField]
    private GameObject catchArea = null;
    // hearableRange���w��
    [SerializeField]
    private Transform hearableRange = null;
    // �v���C���[��ǂ������Ă��邩����
    private bool chase = false;
    // �ҋ@��Ԃ�����
    private bool stayMove = false;
    // �v���C���[�𔭌������ۂ̉����w��
    [SerializeField]
    private AudioClip discoverOnSound = null;
    // �v���C���[��ǂ������Ă���ۂ̉����w��
    [SerializeField]
    private AudioSource chaseOnBGM = null;
    // discoverSound��Volume���w��
    [SerializeField]
    private float discoverSoundVolume = 0.1f;
    // chaseAudio��volume���w��
    private float chaseAudioVolume = 0;
    // chaseAudio�̃t�F�[�h�A�E�g����܂ł̊Ԋu���w��
    [SerializeField]
    private float fadeOutTime = 3;
    // �ǐՎ��̉������Ă��邩�ۂ�
    private bool onChaseAudio = false;

    private int count = 0;

    // �X�e�[�g���쐬
    enum EnemyState
    {
        // �ҋ@���
        Stay,
        // �ʏ���
        Idle,
        // �������
        Discover,
        // ������
        LoseSight,
        // �x�����
        Warning,
        // �߂܂���
        Catch
    }
    [SerializeField]
    EnemyState currentState = EnemyState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        searchManager = GetComponent<SearchManager>();
        naviMesh = GetComponent<NaviMesh>();
        audioSource = GetComponent<AudioSource>();
        chaseAudioVolume = chaseOnBGM.volume;

        // �ړ����x����s���ɕύX
        naviMesh.ChengeSpeed(walkSpeed);
        // �ԗւ̉�]���x��ύX
        wheelChairController.InputObjectSpeed(walkSpeed);
    }

    // �����~�܂��Ă�����
    private void SetStayState()
    {
        // �ړI�n�ɂ����ۂɗ����~�܂�悤�ɏ���
        naviMesh.AutoBraking(true);
        // �ԗւ̉�]���x��ύX
        wheelChairController.InputObjectSpeed(0);
        // �ړ��ڕW�����ݒn�ɂ��āA�ҋ@������B
        naviMesh.SetTargetPosition(transform.position);
        // �ҋ@
        stayMove = true;
    }
    // 
    public void MoveStart()
    {
        SetIdleState();
        // �ړI�n�ɂ��Ă������~�܂�Ȃ���ɏ���
        naviMesh.AutoBraking(true);
    }

    private void SetIdleState()
    {
        // ���񃋁[�g�ɖ߂�
        naviMesh.RootSetDestination();
        // �ړ����x����s���ɕύX
        naviMesh.ChengeSpeed(walkSpeed);
        // �ԗւ̉�]���x��ύX
        wheelChairController.InputObjectSpeed(walkSpeed);
        // �ǐՉ���
        chase = false;
        // �X�e�[�g��J��
        currentState = EnemyState.Idle;
    }
    private void SetDiscoverState()
    {
        // EnemyModel�̃X�e�[�g�J�ڏ���
        enemyModel.SetModelDiscover();
        // �ړ����x��ύX
        naviMesh.ChengeSpeed(runSpeed);
        // �ԗւ̉�]���x��ύX
        wheelChairController.InputObjectSpeed(runSpeed);
        // �X�e�[�g��J��
        currentState = EnemyState.Discover;
        // �ʏ��Ԃ���X�e�[�g�J�ڂ����ۂɉ���炷
        if(!chase)
        {
            audioSource.PlayOneShot(discoverOnSound, discoverSoundVolume);
            chaseOnBGM.Play();
            chaseOnBGM.volume = chaseAudioVolume;
        }
        chase = true;
        onChaseAudio = true;
        // GameOver����G���A������
        catchArea.SetActive(true);
    }
    private void SetLoseSightState()
    {
        // �ړI�n�ɂ����ۂɗ����~�܂�悤�ɏ���
        naviMesh.AutoBraking(true);
        // GameOver����G���A������
        catchArea.SetActive(false);
        // EnemyModel�̃X�e�[�g�J�ڏ���
        enemyModel.SetModelLoseSight();
        // �X�e�[�g��J��
        currentState = EnemyState.LoseSight;
    }

    private void SetWarningState()
    {
        // �ړ����x����s���ɕύX
        naviMesh.ChengeSpeed(walkSpeed);
        // �ړ��ڕW�����������ʒu�ɂ���
        naviMesh.SetTargetPosition(soundPosition);
        // �ړI�n�ɂ����ۂɗ����~�܂�悤�ɏ���
        naviMesh.AutoBraking(true);
        // EnemyModel�̃X�e�[�g�J�ڏ���
        enemyModel.SetModelWarning();
        // �X�e�[�g��J��
        currentState = EnemyState.Warning;
    }

    private void SetCatchState()
    {
        // �ړ����~
        naviMesh.ChengeSpeed(0);
        // �Ԃ����̉�]���~
        wheelChairController.InputObjectSpeed(0);
        // EnemyModel�̃X�e�[�g�J�ڏ���
        enemyModel.SetModelCatch();
        naviMesh.NaviMeshLift();
        currentState = EnemyState.Catch;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            // �ҋ@���
            case EnemyState.Stay:
                if (!stayMove) SetStayState();
                break;
            // �ʏ���
            case EnemyState.Idle:
                UpdateForIdleState();
                break;
            // �������
            case EnemyState.Discover:
                UpdateForDiscoverState();
                break;
            // �ǐՉ���
            case EnemyState.LoseSight:
                UpdateForLoseSightState();
                break;
            // �x�����
            case EnemyState.Warning:
                UpdateForWarningState();
                break;
            // �v���C���[��߂܂������
            case EnemyState.Catch:
                UpdateForCatchState();
                break;
            default:
                break;
        }
    }

    private void UpdateForIdleState()   // �ʏ���
    {
        // �Ώۑ�����w�肵�Ď����̎��E�͈͂ɂ����ꍇsearch��true�ɂ���
        searchManager.SearchAreaManager(lookSearchArea,playerLayerMask,playerLayerNomber);
        // �Ώۂ̉��𕷂����
        Collider playerSoundCollider = searchManager.SoundSearchCollider(hearableRange,hearingSoundLayer);
        // �������������ꍇ
        if (playerSoundCollider != null)
        {
            soundPosition = playerSoundCollider.transform.position;
            // �X�e�[�g�J��
            SetWarningState();   
        }
        // �v���C���[��ڎ��Ō������ꍇ
        if(searchManager.Search)
        {
            // �X�e�[�g�J��
            SetDiscoverState();   
        }
        if(onChaseAudio)
        {
            // chaseOnBGM���t�F�[�h�A�E�g
            AudioSourceFadeOutManager(chaseOnBGM, chaseAudioVolume, fadeOutTime);
        }
    }
    private void UpdateForDiscoverState()   // �v���C���[�𔭌��������
    {
        // �Ώۑ�����w�肵�Ď����̎��E�͈͂ɂ����ꍇsearch��true�ɂ���
        searchManager.SearchAreaManager(lookSearchArea, playerLayerMask, playerLayerNomber);
        if(searchManager.Search)
        {
            targetPosition = player.transform.position;
        }
        else if (!searchManager.Search)  // ������
        {
            // �v���C���[���甭���鉹�𕷂����
            Collider soundCollider = searchManager.SoundSearchCollider(hearableRange, hearingSoundLayer);

            if(soundCollider != null)
            {
                // ���̔������ʒu�ɖړI�n��ύX����
                targetPosition = soundCollider.gameObject.transform.position;
                // ���������ʒu���X�V
                targetPosition = soundCollider.gameObject.transform.position;
            }
            else
            {
                // ���������ʒu�ɖړI�n��ύX����
                targetPosition = searchManager.LostPosition;
                //�@�X�e�[�g�J��
                SetLoseSightState();
            }
        }
        // �ړI�n���X�V
        naviMesh.SetTargetPosition(targetPosition);
    }
    private void UpdateForLoseSightState()  // �v���C���[��ǐՂł��Ȃ��Ȃ������
    {
        // �Ώۑ�����w�肵�Ď����̎��E�͈͂ɂ����ꍇsearch��true�ɂ���
        searchManager.SearchAreaManager(lookSearchArea, playerLayerMask, playerLayerNomber);
        // chaseOnBGM���t�F�[�h�A�E�g
        AudioSourceFadeOutManager(chaseOnBGM,chaseAudioVolume,fadeOutTime);
        // �Ώۂ̉��𕷂����
        Collider playerSoundCollider = searchManager.SoundSearchCollider(hearableRange, hearingSoundLayer);
        // �ړI�n�Ƃ̋������v�Z
        float distance = PlaneDistanceManager(transform.position, targetPosition);
        // �ړI�n�ɂ��ė����~�܂����ۂɎԗւ̉�]���~
        if (distance < 0.1f && count == 0)
        {
            wheelChairController.InputObjectSpeed(0);
            count++;
        }
        // �������Ԃ��w�肷��
        currentTime += Time.deltaTime;
        // IdleState�ɑJ��;
        if(currentTime > loseSightTime)
        {
            SetIdleState();
            currentTime = 0;
            chase = false;
            naviMesh.AutoBraking(false);
            count = 0;
        }
        // �������ۂ�DiscoverState�ɑJ��
        if(searchManager.Search)
        {
            SetDiscoverState();
            currentTime = 0;
            naviMesh.AutoBraking(false);
            chaseOnBGM.volume = chaseAudioVolume;
            count = 0;
        }
        // �������������ꍇ
        if (playerSoundCollider != null)
        {
            soundPosition = playerSoundCollider.transform.position;
            SetWarningState();   // �X�e�[�g�J��
            count = 0;
        }
    }

    private void UpdateForWarningState()    // �v���C���[�̃A�N�V�����ɂ��x�����Ă�����
    {
        // �v���C���[���甭���鉹�𕷂����
        Collider soundCollider = searchManager.SoundSearchCollider(hearableRange,hearingSoundLayer);
        
        // �Ώۑ�����w�肵�Ď����̎��E�͈͂ɂ����ꍇsearch��true�ɂ���
        searchManager.SearchAreaManager(lookSearchArea, playerLayerMask, playerLayerNomber);

        if (soundCollider != null)
        {
            soundPosition = soundCollider.transform.position;
            // �ړ��ڕW�����̈ʒu�ɍX�V����
            naviMesh.SetTargetPosition(soundCollider.transform.position);
            if (currentTime > 0) // �T�m�O�Ɉ�x�o���ꍇ�̏���
            {
                currentTime = 0;
                count = 0;
            }
        }
        // �����������Ȃ��Ȃ����ꍇ
        if (soundCollider == null)
        {
            // �ړI�n�Ƃ̋������v�Z
            float distance = PlaneDistanceManager(transform.position, soundPosition);
            if(distance < 2)
            {
                // �ړI�n�ɂ��ė����~�܂����ۂɎԗւ̉�]���~
                if (distance < 0.1f && count == 0)
                {
                    wheelChairController.InputObjectSpeed(0);
                    count++;
                }
                // �o�ߎ��Ԃ����Z
                currentTime += Time.deltaTime;
                if (currentTime > warningTime)
                {
                    // �X�e�[�g�J��
                    SetIdleState();
                    currentTime = 0;
                    naviMesh.AutoBraking(false);
                    // EnemyModel�̌x����������
                    enemyModel.SetModelReleaseWarning();
                    count = 0;
                }
            }
        }
        // �ڎ��Ńv���C���[��������
        if (searchManager.Search)
        {
            // �X�e�[�g�J��
            SetIdleState();
            naviMesh.AutoBraking(false);
            currentTime = 0;
            count = 0;
        }
    }
    // �v���C���[��߂܂������
    private void UpdateForCatchState()  
    {
        // �v���C���[�̕�������������
        LookManager(player.transform.position);

        currentTime += Time.deltaTime;
        if((currentTime > onGameOverTime) && count == 0)
        {
            playerScript.SetGameOverState();
            count++;
        }
    }

    // AudioSource�̃{�����[�����t�F�[�h�A�E�g�����܂��B
    // (�Ώۂ�AudioSource,�Ώۂ̌��݂̃{�����[�� ,�{�����[�����O�ɂȂ�܂ł̎���)
    private void AudioSourceFadeOutManager(AudioSource audioSource,float audioVolume, float fadeOutTime)
    {
        audioSource.volume -= Time.deltaTime * (audioVolume / fadeOutTime);
        if (audioSource.volume <= 0)
        {
            audioSource.volume = 0;
            onChaseAudio = false;
            audioSource.Stop();
        }
    }
    // ���������ł���ꍇ�̋������v�Z
    private float PlaneDistanceManager(Vector3 myPosition,Vector3 partnerPosition)
    {
        float distance = (new Vector3(partnerPosition.x,0,partnerPosition.z)
                          - new Vector3(myPosition.x,0,myPosition.z)).magnitude;
        return distance;
    }

    // ����̕������������鏈��
    private void LookManager(Vector3 partnerPosition)
    {
        Vector3 distanceVector = (partnerPosition - transform.position).normalized;
        float angleY = (Mathf.Atan2(distanceVector.x, distanceVector.z) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Euler(0, angleY, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�ƐڐG�����ꍇ
        if(other.gameObject.CompareTag("Player"))
        {
            if(currentState != EnemyState.Catch)
            {
                // �X�e�[�g�J��
                SetCatchState();
            }
        }
    }
}
