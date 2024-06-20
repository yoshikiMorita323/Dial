using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// �v���C���[�ْ̋���ԁi�S���E��ʂ̕ω��j�̏���
public class HeartbeatManager : MonoBehaviour
{
    // �R���|�[�l���g���擾
    AudioSource audioSource;
    Image image;
    Animator animator;
    Gamepad gamepad;

    // HearinArea���w��
    [SerializeField]
    private GameObject hearingArea = null;
    // �G�̃��C���[���w��
    [SerializeField]
    private LayerMask enemyLayer = default;
    // �����ƓG�Ƃ̋������w��
    private float distance = 0;
    // �G�̂��Ƃ��������邩�ۂ�
    private bool onHearing = false;  
    private int count = 0;

    // heartBresAudio���w��
    [SerializeField]
    private AudioSource heartBresAudio = null;
    // �s�b�`��1����ǂꂭ�炢�㏸�������������w��
    [SerializeField]
    private float pitchPlus = 0.7f;
    // HeartBresImage���w��
    [SerializeField]
    private Image heartBresImage = null;
    // �A���x�h�l���w��
    private float albedoValue = 0;
    [SerializeField]
    private float maxAlbedoValue = 80;
    // �S������������Œ������̃A���x�h�l���ő�܂��͍ŏ��ɂȂ�܂ł̎��Ԃ��w�肵�܂��B
    [SerializeField]
    private float albedoChangeTime = 1.5f;
    // �A���x�h�l���㏸�����邩���~�����邩�𔻒�
    [SerializeField]
    private bool up = true;
    // �R���g���[���[�̃��[�^�[�X�s�[�h�ɉ��Z����l���w�肵�܂��B
    [SerializeField]
    private float motorSpeedPluse = 0.2f;
    public bool Damage { get => damage; set => damage = value; }
    // �G�ɕ߂܂�A�U�����󂯂�������
    private bool damage = false;

    // HartBresAnimator�̃p�����[�^�[ID
    static readonly int speedId = Animator.StringToHash("Speed");
    static readonly int startId = Animator.StringToHash("Start");
    static readonly int idleId = Animator.StringToHash("Idle");

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
        gamepad = Gamepad.current;
    }

    void Update()
    {
        // �w�肵�����C���[���͈͓��ɓ������ۂɁA�ΏۂƎ��g�̋������v�Z���܂��B
        SearchEnemyForDistance(hearingArea.transform, enemyLayer);
        
        if(!Damage)
        {
            if (onHearing)   // �G���߂��ɂ��邱�ƂɋC�t���ْ����Ă�����
            {
                if (count == 0)
                {
                    animator.SetTrigger(idleId);
                    count++;
                }
                // �ΏۂƎ��g�̋����ɉ�����heartBresAudio�̃{�����[���ƃs�b�`��ύX���܂��B
                HeartBresDistanceAudioManager();
                // �ΏۂƎ��g�̋����ɉ�����ImageManager�̃A���x�h�l�̍X�V���������܂��B
                HeartBresImageManager();
            }
            else if (!onHearing && count > 0)   // �ْ����Ă��Ȃ����
            {
                animator.SetTrigger(startId);
                OffBresBeat();
                albedoValue = 0;
                heartBresImage.color = new Color(255, 255, 255, albedoValue);
                count = 0;
            }
        }
        else if(count > 0 && Damage)
        {
            // �Q�[���I�[�o�[���̌ۓ��̌�������
            HeartBreasGraduallySmaller();
        }
    }

    // �w�肵�����C���[���͈͓��ɓ������ۂɁA�ΏۂƎ��g�̋������v�Z���܂��B
    private void SearchEnemyForDistance(Transform searchArea, LayerMask overlapLayerMask)
    {
        // ���肷��͈͂ƃ��C���[���w�肵�A�͈͓��Ƀ��C���[���������ꍇcollider�Ɋi�[
        Collider[] collider = Physics.OverlapSphere(
                                searchArea.position,
                                searchArea.transform.localScale.x,
                                overlapLayerMask);

        // collider�Ƀ��C���[���i�[���Ă���ꍇ��������B
        if (collider.Length > 0)
        {
            onHearing = true;
            Vector3 myPosition = transform.position;
            
            if(collider.Length == 1)    // collider�Ɋi�[���Ă��鐔��1�������ꍇ
            {
                distance = (myPosition - collider[0].transform.position).magnitude;
                return;
            }
            if (collider.Length > 1)     // collider�Ɋi�[���Ă��鐔��2�ȏゾ�����ꍇ
            {
                // colllider����0�z��ڂ̈ʒu�ƁA�����Ƃ̋������擾
                float minDistance = (myPosition - collider[0].transform.position).magnitude;
                int i;
                // ��ԋ������߂��I�u�W�F�N�g�̋�����I��
                for(i = 0;i < collider.Length - 1;i++)
                {
                    float colliderDistance = (myPosition - collider[i + 1].gameObject.transform.position).magnitude;
                    if (minDistance > colliderDistance)
                    {
                        minDistance = colliderDistance;
                    }
                }
                distance = minDistance;
            }
        }
        else
        {
            onHearing = false;
        }
    }
    // �ΏۂƎ��g�̋����ɉ�����heartBresAudio�̃{�����[���ƃs�b�`��ύX���܂��B
    private void HeartBresDistanceAudioManager()
    {
        heartBresAudio.volume = (hearingArea.transform.localScale.x - distance) / hearingArea.transform.localScale.x;
        heartBresAudio.pitch = (hearingArea.transform.localScale.x - distance) / hearingArea.transform.localScale.x + pitchPlus;
        animator.SetFloat(speedId, heartBresAudio.pitch);
    }

�@�@// �G�̍U�����󂯁A�ۓ������X�ɏ��������A�x���Ȃ�悤�ɏ���
    private void HeartBreasGraduallySmaller()
    {
        if(heartBresAudio.volume > 0)
        {
            // volume������������
            heartBresAudio.volume -= Time.deltaTime * 0.1f;
            // pitch������
            heartBresAudio.pitch -= Time.deltaTime * 0.1f;
            // pitch�ɂ��킹��Animator�̑��x�𑀍�
            animator.SetFloat(speedId, heartBresAudio.pitch);
        }
        else  
        {
            count = 0;
            // �A�j���[�^�[��J�ڂ��Čۓ����~�߂�
            animator.SetTrigger(startId);
            OffBresBeat();
        }
    }

    // �ΏۂƎ��g�̋����ɉ�����ImageManager�̃A���x�h�l�̍X�V���������܂��B
    private void HeartBresImageManager()
    {
        // �G�Ƃ̋����ɉ����Ă�heartBresImage�̍ő�A���x�h�l���w�肵�܂��B
        float albedoTopValue = ((hearingArea.transform.localScale.x - distance) / hearingArea.transform.localScale.x) * maxAlbedoValue;
        // ���݂̃A���x�h�l���ő�܂��͍ŏ��ɂȂ�܂ł̎��Ԃ��w�肵�܂��B
        float albedoChangeTimeNow = albedoChangeTime - (hearingArea.transform.localScale.x - distance) / hearingArea.transform.localScale.x;
        if ((0 > albedoValue) && !up) // ���~���ɃA���x�h�l���ŏ��l�𒴂����ꍇ
        {
            up = true;
        }
        else if((albedoTopValue < albedoValue) && up) // �㏸���ɃA���x�h�l���ő�l�𒴂����ꍇ
        {
            up = false;
        }

        if(up)  // �A���x�h�l���㏸������B
        {
            albedoValue += Time.deltaTime * albedoTopValue / albedoChangeTimeNow;
        }
        else // �A���x�h�l�����~������B
        {
            albedoValue -= Time.deltaTime * albedoTopValue / albedoChangeTimeNow;
        }
        // �A���x�h�l���X�V
        heartBresImage.color = new Color(1, 1, 1,albedoValue / 100);
    }
    // �R���g���[���[��U��������
    public void HeartBresBeet()
    {
        if(gamepad != null)
        {
            float motorSpeed = heartBresAudio.volume + motorSpeedPluse;
            gamepad.SetMotorSpeeds(motorSpeed, motorSpeed);
        }
    }
    // �R���g���[���[�̐U�����~����
    public void OffBresBeat()
    {
        if(gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }

}
