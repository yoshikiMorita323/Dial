using UnityEngine;

// �v���C���[�̓��Ă�����
public class ThrowingManager : MonoBehaviour
{
    // ���Ă��J�n�ʒu���w��
    [SerializeField]
    private Transform bottleHolder = null;
    // �E�����r���i�[
    private GameObject[] bottleStock = new GameObject[100];
    // �J�����̈ʒu���W���w��
    [SerializeField]
    private Transform cameraPoint = null;
    // ���Ă��͂��w��
    [SerializeField]
    private Vector3 throwingPower = Vector3.zero;
    public int StorageCount { get => storageCount;private set => storageCount = value; }

    // �r���̃X�g�b�N�����w��
    private int storageCount = 0;

    [SerializeField]
    private ItemUIManager itemUIManager = null;

    new Rigidbody rigidbody;
    AudioSource audioSource;

    [SerializeField]
    private AudioClip throwingOnSound = null;
    [SerializeField]
    private float throwingSoundVolume = 1;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // �E�����r��z�񂲂ƂɊi�[
    public void BottleStorage(GameObject bottle)
    {
        // �擾�����r����bottleHolder�̎q�I�u�W�F�N�g���ɂ��Ĕz��Ɋi�[
        bottleStock[StorageCount] = bottle;
        bottleStock[StorageCount].transform.parent = bottleHolder;

        StorageCount++;
    }
    // ���Ă�����
    public void ThrowingController()
    {
        if(StorageCount > 0)
        {
            // ������A�C�e�����A�N�e�B�u�ɂ���
            bottleStock[StorageCount - 1].SetActive(true);
            // �e�̃I�u�W�F�N�g����؂藣���ē��Ă��̏���������
            bottleStock[StorageCount - 1].GetComponent<Bottle>().OnThrowing(transform.rotation);
            audioSource.PlayOneShot(throwingOnSound, throwingSoundVolume);
            // �v���C���[�������Ă�������ɂ��킹�ė͂�������
            bottleStock[StorageCount - 1].GetComponent<Rigidbody>().
              AddForce((cameraPoint.transform.forward * throwingPower.z + transform.up * throwingPower.y), 
              ForceMode.Impulse);

            StorageCount--;
            // BottleUI���̏����������Z
            itemUIManager.AddItem(-1);
        }
    }

}
