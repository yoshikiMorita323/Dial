using UnityEngine;

// ���̊J����
public class DoorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject leftDoor = null;
    [SerializeField]
    private GameObject rightDoor = null;
    [SerializeField]
    private GameObject rightDoorToggle = null;
    [SerializeField]
    private GameObject leftDoorToggle = null;
    [SerializeField]
    private Transform inTheBackCollider = null;
    [SerializeField]
    private Transform forGroundCollider = null;
    // �ΏۂƂȂ郌�C���[�}�X�N���w��
    [SerializeField]
    private LayerMask searchLayer = default;
    private GameObject inTheBack = null;
    private GameObject forGround = null;

    // �h�A�̊J���x���w��
    [SerializeField]
    private float moveDoorSpeed = 10;
    // doorTggle��Y���̊p�x���w��
    private float doorToggleAngleY = 0;
    // �h�A���J���Ă��邩����
    private bool openDoor = false;
    // ��O�ɊJ���Ă��邩���ɊJ���Ă��邩�𔻒�
    private bool openInTheBack = false;
    // �h�A�ւ̃A�N�Z�X�����w��
    private int accessCount = 0;
    private int onSoundCount = 0;

    AudioSource audioSource;
    [SerializeField]
    private AudioClip openDoorOnSound = null;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        // �G���A���ɑΏۂ̃��C���[�������Ă��邩����
        inTheBack = SearchInTheBackBoxCollider(inTheBackCollider, searchLayer);
        forGround = SearchForGroundBoxCollider(forGroundCollider, searchLayer);
        // ����
        if (inTheBack != null)
        {
            if (inTheBack.layer == 19)�@// �G���h�A�̎�O���̃G���A���ɓ������ꍇ
            {
                if (!openDoor)
                {
                    accessCount++;
                    // ���C���[��OpenDoor�ɕύX 
                    leftDoor.gameObject.layer = 22;
                    rightDoor.gameObject.layer = 22;
                    openInTheBack = true;
                    openDoor = true;
                }
            }
        }
        // ��O��
        if (forGround != null)
        {
            if (forGround.layer == 19)  // �G���h�A�̎�O���̃G���A���ɓ������ꍇ
            {
                if (!openDoor)
                {
                    accessCount++;
                    // ���C���[��OpenDoor�ɕύX 
                    leftDoor.gameObject.layer = 22;
                    rightDoor.gameObject.layer = 22;
                    openInTheBack = false;
                    openDoor = true;
                }
            }
        }

        if (accessCount > 0)
        {
            // �h�A�ɃA�N�Z�X�����ۂ̔��̓���������
            RotateDoorToggre();
        }
    }

    // �h�A�̊J�󋵂ɉ����āA���{���铮���؂�ւ���
    public void MoveDoorAccess()
    {
        accessCount++;
        if(inTheBack != null) // ��O���ɑΏۂ�����Ƃ�
        {
            if(!openDoor)
            {
                // ���C���[��OpenDoor�ɕύX 
                leftDoor.gameObject.layer = 22;
                rightDoor.gameObject.layer = 22;
                openInTheBack = true;
                openDoor = true;
            }
            else
            {
                // ���C���[��CloseDoor�ɕύX 
                leftDoor.gameObject.layer = 21;
                rightDoor.gameObject.layer = 21;
                openDoor = false;
            }
        }
        else if(forGround != null) // ���ɑΏۂ�����Ƃ�
        {
            if(!openDoor)
            {
                // ���C���[��OpenDoor�ɕύX 
                leftDoor.gameObject.layer = 22;
                rightDoor.gameObject.layer = 22;
                openInTheBack = false;
                openDoor = true;
            }
            else
            {
                // ���C���[��CloseDoor�ɕύX 
                leftDoor.gameObject.layer = 21;
                rightDoor.gameObject.layer = 21;
                openDoor = false;
            }
        }
    }

    // �h�A�ɃA�N�Z�X�����ۂ̔��̓���������
    private void RotateDoorToggre()
    {
        if(onSoundCount == 0)
        {
            audioSource.PlayOneShot(openDoorOnSound, 0.3f);
            onSoundCount++;
        }
        if (openDoor)  // �J����
        {
            if (doorToggleAngleY < 90)
            {
                doorToggleAngleY += moveDoorSpeed * Time.deltaTime;
            }
            else if (doorToggleAngleY >= 90)
            {
                doorToggleAngleY = 90;
                accessCount = 0;
                onSoundCount = 0;
            }
        }
        else if (!openDoor) // �߂�
        {
            if (doorToggleAngleY > 0)
            {
                doorToggleAngleY -= moveDoorSpeed * Time.deltaTime;
            }
            else if (doorToggleAngleY <= 0)
            {
                doorToggleAngleY = 0;
                accessCount = 0;
                onSoundCount = 0;
            }
        }

        if (openInTheBack) // ��O���ɑΏۂ�����Ƃ�
        {
            rightDoorToggle.transform.localRotation = Quaternion.Euler(0, -doorToggleAngleY, 0);
            leftDoorToggle.transform.localRotation = Quaternion.Euler(0, doorToggleAngleY, 0);
        }
        else if(!openInTheBack) // ���ɑΏۂ�����Ƃ�
        {
            rightDoorToggle.transform.localRotation = Quaternion.Euler(0, doorToggleAngleY, 0);
            leftDoorToggle.transform.localRotation = Quaternion.Euler(0, -doorToggleAngleY, 0);
        }
    }

    // InTheBackCollider���ɓ�����collider���擾
    public GameObject SearchInTheBackBoxCollider(Transform searchArea, LayerMask overlapLayerMask)
    {
        Collider[] collider = Physics.OverlapBox(
                                searchArea.position,
                                searchArea.transform.localScale / 2,
                                transform.rotation,
                                overlapLayerMask);

        if (collider.Length > 0)
        {
            return collider[0].gameObject;
        }
        else
        {
            return null;
        }
    }
    // ForGroundCollider���ɓ�����collider���擾
    public GameObject SearchForGroundBoxCollider(Transform searchArea, LayerMask overlapLayerMask)
    {
        Collider[] collider = Physics.OverlapBox(
                                searchArea.position,
                                searchArea.transform.localScale / 2,
                                transform.rotation,
                                overlapLayerMask);

        if (collider.Length > 0)
        {
            return collider[0].gameObject; ;
        }
        else
        {
            return null;
        }
    }


}
