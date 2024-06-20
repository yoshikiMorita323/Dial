using UnityEngine;

// �r�̋�������
public class Bottle : ItemManager
{
    new Rigidbody rigidbody;
    new ParticleSystem particleSystem;

    // ���Ă�����Ă��邩�𔻒�
    private bool onThrow = false;
    // ���Ă����̉�]���x���w��
    [SerializeField]
    private Vector3 throwingRotation = Vector3.zero;
    private float rotateAngleX = 0;
    // ���Ă����ꂽ�r���ǂ⏰�ɏՓ˂����ۂ̉��̕�������͈͂��w��
    [SerializeField]
    private GameObject soundArea = null;
    // �j�󎞂̃G�t�F�N�g���w��
    [SerializeField]
    private ParticleSystem brokenEffect = null;
    [SerializeField]
    private AudioClip collisionOnSound = null;
    [SerializeField]
    private float collisionSoundVolume = 1;

    [SerializeField]
    private ItemUIManager itemUIManager = null;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // ���Ă����̉�]����
        if (onThrow)
        {
            rotateAngleX += throwingRotation.x * Time.deltaTime;
            transform.rotation = Quaternion.Euler(rotateAngleX, transform.eulerAngles.y, 0);
        }
    }

    // �e�I�u�W�F�N�g����؂藣���ēƗ�������
    public void ParentLift()
    {
        gameObject.transform.parent = null;
    }
    // �r���擾����
    public void PickUpBottle()
    {
        PickUp();
        rigidbody.useGravity = true;
    }
    // �A�C�e���ǉ�����
    protected override void AddItemStock()
    {
        // Canvas��BottleUI��1���Z
        itemUIManager.AddItem(1);
    }

    // ���Ă�����
    public void OnThrowing(Quaternion parentRotation)
    {
        ParentLift();
        model.SetActive(true);
        collider.enabled = true;
        rigidbody.isKinematic = false;
        // �e�I�u�W�F�N�g�ƌ����Ă���p�x�����킹��
        transform.rotation = parentRotation;
        onThrow = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        // ���Ă�����Ă����ԂŏՓ˂����ۂ̏���
        if (onThrow)
        {
            onThrow = false;
            rigidbody.isKinematic = true;
            model.SetActive(false);
            // �Փˎ��̉���炷
            audioSource.PlayOneShot(collisionOnSound, collisionSoundVolume);
            // ���͈̔͂��w��
            soundArea.SetActive(true);
            brokenEffect.Play();
            Destroy(soundArea, 0.1f);
            Destroy(gameObject, 3);
        }
    }
}
