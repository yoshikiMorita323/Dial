using UnityEngine;

// �L�����N�^�[�̈ړ�������Ȃǂ̉^����\���܂��B
[RequireComponent(typeof(Rigidbody))]
public class MoveBehaviour : MonoBehaviour
{
    // �ړ��̑������w�肵�܂��B
    private float moveSpeed;
    public bool rotateStop = false;
    // Body���w��
    [SerializeField]
    private GameObject body = null;
    // ���Ⴊ��ł��邩����
    [SerializeField]
    private bool squatDown = false;

    // ��]�̑������擾���܂��B
    public float RotationSpeed
    {
        get => rotationSpeed;
        private set => rotationSpeed = value;
    }
    // ��]�̑������w�肵�܂��B
    [SerializeField] 
    private float rotationSpeed = 1;

    // ���Ⴊ�񂾂Ƃ��̗�����ԂƂ̃X�P�[���̔䗦���w��
    public float scaleSquatRatio = 0.65f;
    // ���݂̓��̈ʒu���w��
    [SerializeField]
    private Transform headPosition = null;
    private Vector3 idleHeadPosition = Vector3.zero;
    // ���Ⴊ�񂾎��̓��̈ʒu���w��
    [SerializeField]
    private Vector3 headSquatPosition = Vector3.zero;
    // ���Ⴊ�ޑ��x���w��
    [SerializeField]
    private float squatAmount = 1;
    private Vector3 myScale = Vector3.zero;

    new Rigidbody rigidbody;
    new Collider collider;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = body.GetComponent<Collider>();

        myScale = transform.localScale;
        if(headPosition != null)
        idleHeadPosition = headPosition.localPosition;
    }
    // �ړ�����
    public void Move(Vector2 normalizedSpeed)
    {
        // �O����
        var velocity = transform.forward * normalizedSpeed.y;
        // �E����
        velocity += transform.right * normalizedSpeed.x;
        // �ړ����x�𔽉f����
        velocity *= moveSpeed;
        rigidbody.velocity = velocity;
    }
    // �ړ����x���X�V
    public void MoveSpeedManager(float speed)
    {
        moveSpeed = speed;
    }

    // ���_�ړ�����
    public void Rotate(float normalizedSpeed)
    {
        if(!rotateStop)
        {
            transform.Rotate(0, RotationSpeed * normalizedSpeed, 0);
        }
    }
    // ���Ⴊ�ݓ��쎞�̑̂̕ω��A���_�ړ�����
    public void SquatBodyManager(bool squat)
    {
        squatDown = squat;
        float squatScaleY = myScale.y * scaleSquatRatio;
        var bodyScale = body.gameObject.transform.localScale;

        if (squat) // ���Ⴊ��
        {
            if(bodyScale.y > squatScaleY)
            {
                bodyScale.y -= squatAmount * Time.deltaTime;
                body.gameObject.transform.localScale = bodyScale;
            }
            else
            {
                body.gameObject.transform.localScale = new Vector3(1, squatScaleY, 1);
            }
        }
        else if(!squat)  // ����
        {
            if(bodyScale.y < myScale.y)
            {
                bodyScale.y += squatAmount * Time.deltaTime;
                body.gameObject.transform.localScale = bodyScale;
            }
            else
            {
                body.gameObject.transform.localScale = myScale;
            }
        }

        if(headPosition != null)
        {
            // ���_�ړ�
            SquatMoveHead();
        }
    }
    // ���Ⴊ�ݓ��쎞�̎��_�ړ�����
    private void SquatMoveHead()
    {
        var headPos = headPosition.localPosition;
        
        if(squatDown)   // ���Ⴊ��
        {
            if (headPos.y > headSquatPosition.y)
            {
                headPos.y -= squatAmount * Time.deltaTime;
            }
            else
            {
                headPos.y = headSquatPosition.y;
            }

        }
        else if(!squatDown)  // ����
        {
            if (headPos.y < idleHeadPosition.y)
            {
                headPos.y += squatAmount * Time.deltaTime;
            }
            else
            {
                headPos.y = idleHeadPosition.y;
            }
        }
        headPosition.localPosition = headPos;
    }

    public void BodyKinematic()
    {
        rigidbody.isKinematic = true;
        collider.isTrigger = true;
    }

}
