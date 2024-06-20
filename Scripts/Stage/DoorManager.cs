using UnityEngine;

// 扉の開閉処理
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
    // 対象となるレイヤーマスクを指定
    [SerializeField]
    private LayerMask searchLayer = default;
    private GameObject inTheBack = null;
    private GameObject forGround = null;

    // ドアの開閉速度を指定
    [SerializeField]
    private float moveDoorSpeed = 10;
    // doorTggleのY軸の角度を指定
    private float doorToggleAngleY = 0;
    // ドアが開いているか判定
    private bool openDoor = false;
    // 手前に開いているか奥に開いているかを判定
    private bool openInTheBack = false;
    // ドアへのアクセス数を指定
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
        // エリア内に対象のレイヤーが入っているか判定
        inTheBack = SearchInTheBackBoxCollider(inTheBackCollider, searchLayer);
        forGround = SearchForGroundBoxCollider(forGroundCollider, searchLayer);
        // 奥側
        if (inTheBack != null)
        {
            if (inTheBack.layer == 19)　// 敵がドアの手前側のエリア内に入った場合
            {
                if (!openDoor)
                {
                    accessCount++;
                    // レイヤーをOpenDoorに変更 
                    leftDoor.gameObject.layer = 22;
                    rightDoor.gameObject.layer = 22;
                    openInTheBack = true;
                    openDoor = true;
                }
            }
        }
        // 手前側
        if (forGround != null)
        {
            if (forGround.layer == 19)  // 敵がドアの手前側のエリア内に入った場合
            {
                if (!openDoor)
                {
                    accessCount++;
                    // レイヤーをOpenDoorに変更 
                    leftDoor.gameObject.layer = 22;
                    rightDoor.gameObject.layer = 22;
                    openInTheBack = false;
                    openDoor = true;
                }
            }
        }

        if (accessCount > 0)
        {
            // ドアにアクセスした際の扉の動きを処理
            RotateDoorToggre();
        }
    }

    // ドアの開閉状況に応じて、実施する動作を切り替える
    public void MoveDoorAccess()
    {
        accessCount++;
        if(inTheBack != null) // 手前側に対象がいるとき
        {
            if(!openDoor)
            {
                // レイヤーをOpenDoorに変更 
                leftDoor.gameObject.layer = 22;
                rightDoor.gameObject.layer = 22;
                openInTheBack = true;
                openDoor = true;
            }
            else
            {
                // レイヤーをCloseDoorに変更 
                leftDoor.gameObject.layer = 21;
                rightDoor.gameObject.layer = 21;
                openDoor = false;
            }
        }
        else if(forGround != null) // 奥に対象がいるとき
        {
            if(!openDoor)
            {
                // レイヤーをOpenDoorに変更 
                leftDoor.gameObject.layer = 22;
                rightDoor.gameObject.layer = 22;
                openInTheBack = false;
                openDoor = true;
            }
            else
            {
                // レイヤーをCloseDoorに変更 
                leftDoor.gameObject.layer = 21;
                rightDoor.gameObject.layer = 21;
                openDoor = false;
            }
        }
    }

    // ドアにアクセスした際の扉の動きを処理
    private void RotateDoorToggre()
    {
        if(onSoundCount == 0)
        {
            audioSource.PlayOneShot(openDoorOnSound, 0.3f);
            onSoundCount++;
        }
        if (openDoor)  // 開ける
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
        else if (!openDoor) // 閉める
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

        if (openInTheBack) // 手前側に対象がいるとき
        {
            rightDoorToggle.transform.localRotation = Quaternion.Euler(0, -doorToggleAngleY, 0);
            leftDoorToggle.transform.localRotation = Quaternion.Euler(0, doorToggleAngleY, 0);
        }
        else if(!openInTheBack) // 奥に対象がいるとき
        {
            rightDoorToggle.transform.localRotation = Quaternion.Euler(0, doorToggleAngleY, 0);
            leftDoorToggle.transform.localRotation = Quaternion.Euler(0, -doorToggleAngleY, 0);
        }
    }

    // InTheBackCollider内に入ったcolliderを取得
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
    // ForGroundCollider内に入ったcolliderを取得
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
