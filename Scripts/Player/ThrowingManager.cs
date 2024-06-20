using UnityEngine;

// プレイヤーの投てき処理
public class ThrowingManager : MonoBehaviour
{
    // 投てき開始位置を指定
    [SerializeField]
    private Transform bottleHolder = null;
    // 拾った瓶を格納
    private GameObject[] bottleStock = new GameObject[100];
    // カメラの位置座標を指定
    [SerializeField]
    private Transform cameraPoint = null;
    // 投てき力を指定
    [SerializeField]
    private Vector3 throwingPower = Vector3.zero;
    public int StorageCount { get => storageCount;private set => storageCount = value; }

    // ビンのストック数を指定
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

    // 拾った瓶を配列ごとに格納
    public void BottleStorage(GameObject bottle)
    {
        // 取得したビンをbottleHolderの子オブジェクト化にして配列に格納
        bottleStock[StorageCount] = bottle;
        bottleStock[StorageCount].transform.parent = bottleHolder;

        StorageCount++;
    }
    // 投てき処理
    public void ThrowingController()
    {
        if(StorageCount > 0)
        {
            // 投げるアイテムをアクティブにする
            bottleStock[StorageCount - 1].SetActive(true);
            // 親のオブジェクトから切り離して投てきの準備をする
            bottleStock[StorageCount - 1].GetComponent<Bottle>().OnThrowing(transform.rotation);
            audioSource.PlayOneShot(throwingOnSound, throwingSoundVolume);
            // プレイヤーが向いている方向にあわせて力を加える
            bottleStock[StorageCount - 1].GetComponent<Rigidbody>().
              AddForce((cameraPoint.transform.forward * throwingPower.z + transform.up * throwingPower.y), 
              ForceMode.Impulse);

            StorageCount--;
            // BottleUI内の所持数を減算
            itemUIManager.AddItem(-1);
        }
    }

}
