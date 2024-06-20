using UnityEngine;

// 鍵がかかっている扉の鍵所持個数に応じた挙動処理
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

    // 鍵を解除した際にEnemyの待機状態を解除する処理をおこなうか指定
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

    // 鍵の所持個数に応じた処理
    public void LockOpen(int keyCount)
    {
        // 条件取得数以上、鍵を所持していれば開錠する
        if(keyTotalNumber <= keyCount)
        {
            doorManager.enabled = true;
            rightDoor.gameObject.layer = 21;
            leftDoor.gameObject.layer = 21;
            collider.enabled = false;
            audioSource.PlayOneShot(openOnSound,openSoundVolume);
            if(lockEnemy)
            {
                // Enemyの待機状態を解除する
                enemyStayOutManager.EnemyStayOut();
            }
        }
        else
        {
            audioSource.PlayOneShot(notOpenOnSound, notOpenSoundVolume);
        }
    }
}
