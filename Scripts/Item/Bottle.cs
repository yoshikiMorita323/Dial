using UnityEngine;

// 瓶の挙動処理
public class Bottle : ItemManager
{
    new Rigidbody rigidbody;
    new ParticleSystem particleSystem;

    // 投てきされているかを判定
    private bool onThrow = false;
    // 投てき時の回転速度を指定
    [SerializeField]
    private Vector3 throwingRotation = Vector3.zero;
    private float rotateAngleX = 0;
    // 投てきされた瓶が壁や床に衝突した際の音の聞こえる範囲を指定
    [SerializeField]
    private GameObject soundArea = null;
    // 破壊時のエフェクトを指定
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
        // 投てき時の回転処理
        if (onThrow)
        {
            rotateAngleX += throwingRotation.x * Time.deltaTime;
            transform.rotation = Quaternion.Euler(rotateAngleX, transform.eulerAngles.y, 0);
        }
    }

    // 親オブジェクトから切り離して独立させる
    public void ParentLift()
    {
        gameObject.transform.parent = null;
    }
    // ビン取得処理
    public void PickUpBottle()
    {
        PickUp();
        rigidbody.useGravity = true;
    }
    // アイテム追加処理
    protected override void AddItemStock()
    {
        // CanvasのBottleUIに1加算
        itemUIManager.AddItem(1);
    }

    // 投てき処理
    public void OnThrowing(Quaternion parentRotation)
    {
        ParentLift();
        model.SetActive(true);
        collider.enabled = true;
        rigidbody.isKinematic = false;
        // 親オブジェクトと向いている角度をあわせる
        transform.rotation = parentRotation;
        onThrow = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        // 投てきされている状態で衝突した際の処理
        if (onThrow)
        {
            onThrow = false;
            rigidbody.isKinematic = true;
            model.SetActive(false);
            // 衝突時の音を鳴らす
            audioSource.PlayOneShot(collisionOnSound, collisionSoundVolume);
            // 音の範囲を指定
            soundArea.SetActive(true);
            brokenEffect.Play();
            Destroy(soundArea, 0.1f);
            Destroy(gameObject, 3);
        }
    }
}
