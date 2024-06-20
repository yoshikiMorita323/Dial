using UnityEngine;

// キャラクターの移動や向きなどの運動を表します。
[RequireComponent(typeof(Rigidbody))]
public class MoveBehaviour : MonoBehaviour
{
    // 移動の速さを指定します。
    private float moveSpeed;
    public bool rotateStop = false;
    // Bodyを指定
    [SerializeField]
    private GameObject body = null;
    // しゃがんでいるか判定
    [SerializeField]
    private bool squatDown = false;

    // 回転の速さを取得します。
    public float RotationSpeed
    {
        get => rotationSpeed;
        private set => rotationSpeed = value;
    }
    // 回転の速さを指定します。
    [SerializeField] 
    private float rotationSpeed = 1;

    // しゃがんだときの立ち状態とのスケールの比率を指定
    public float scaleSquatRatio = 0.65f;
    // 現在の頭の位置を指定
    [SerializeField]
    private Transform headPosition = null;
    private Vector3 idleHeadPosition = Vector3.zero;
    // しゃがんだ時の頭の位置を指定
    [SerializeField]
    private Vector3 headSquatPosition = Vector3.zero;
    // しゃがむ速度を指定
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
    // 移動処理
    public void Move(Vector2 normalizedSpeed)
    {
        // 前方向
        var velocity = transform.forward * normalizedSpeed.y;
        // 右方向
        velocity += transform.right * normalizedSpeed.x;
        // 移動速度を反映する
        velocity *= moveSpeed;
        rigidbody.velocity = velocity;
    }
    // 移動速度を更新
    public void MoveSpeedManager(float speed)
    {
        moveSpeed = speed;
    }

    // 視点移動処理
    public void Rotate(float normalizedSpeed)
    {
        if(!rotateStop)
        {
            transform.Rotate(0, RotationSpeed * normalizedSpeed, 0);
        }
    }
    // しゃがみ動作時の体の変化、視点移動処理
    public void SquatBodyManager(bool squat)
    {
        squatDown = squat;
        float squatScaleY = myScale.y * scaleSquatRatio;
        var bodyScale = body.gameObject.transform.localScale;

        if (squat) // しゃがむ
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
        else if(!squat)  // 立つ
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
            // 視点移動
            SquatMoveHead();
        }
    }
    // しゃがみ動作時の視点移動処理
    private void SquatMoveHead()
    {
        var headPos = headPosition.localPosition;
        
        if(squatDown)   // しゃがむ
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
        else if(!squatDown)  // 立つ
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
