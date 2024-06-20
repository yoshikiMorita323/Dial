using UnityEngine;

// �����������Ă�����̌��������ɉ�������������
public class LockKeyManager : MonoBehaviour
{
    new Collider collider;
    [SerializeField]
    private DoorManager doorManager = null;
    [SerializeField]
    private GameObject rightDoor = null;
    [SerializeField]
    private GameObject leftDoor = null;
    [SerializeField]
    private int keyTotalNumber = 3;

    AudioSource audioSource;
    [SerializeField]
    private AudioClip openOnSound = null;
    [SerializeField]
    private float openSoundVolume = 0.7f;
    [SerializeField]
    private AudioClip notOpenOnSound = null;
    [SerializeField]
    private float notOpenSoundVolume = 1f;

    // �������������ۂ�Enemy�̑ҋ@��Ԃ��������鏈���������Ȃ����w��
    [SerializeField]
    private bool lockEnemy = false;
    private EnemyStayOutManager enemyStayOutManager;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<Collider>();
        doorManager.enabled = false;
        if(lockEnemy)
        {
            enemyStayOutManager = GetComponent<EnemyStayOutManager>();
        }
    }

    // ���̏������ɉ���������
    public void LockOpen(int keyCount)
    {
        // �����擾���ȏ�A�����������Ă���ΊJ������
        if(keyTotalNumber <= keyCount)
        {
            doorManager.enabled = true;
            rightDoor.gameObject.layer = 21;
            leftDoor.gameObject.layer = 21;
            collider.enabled = false;
            audioSource.PlayOneShot(openOnSound,openSoundVolume);
            if(lockEnemy)
            {
                // Enemy�̑ҋ@��Ԃ���������
                enemyStayOutManager.EnemyStayOut();
            }
        }
        else
        {
            audioSource.PlayOneShot(notOpenOnSound, notOpenSoundVolume);
        }
    }
}
