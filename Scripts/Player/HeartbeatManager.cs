using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// プレイヤーの緊張状態（心音・画面の変化）の処理
public class HeartbeatManager : MonoBehaviour
{
    // コンポーネントを取得
    AudioSource audioSource;
    Image image;
    Animator animator;
    Gamepad gamepad;

    // HearinAreaを指定
    [SerializeField]
    private GameObject hearingArea = null;
    // 敵のレイヤーを指定
    [SerializeField]
    private LayerMask enemyLayer = default;
    // 自分と敵との距離を指定
    private float distance = 0;
    // 敵のおとが聞こえるか否か
    private bool onHearing = false;  
    private int count = 0;

    // heartBresAudioを指定
    [SerializeField]
    private AudioSource heartBresAudio = null;
    // ピッチを1からどれくらい上昇させたいかを指定
    [SerializeField]
    private float pitchPlus = 0.7f;
    // HeartBresImageを指定
    [SerializeField]
    private Image heartBresImage = null;
    // アルベド値を指定
    private float albedoValue = 0;
    [SerializeField]
    private float maxAlbedoValue = 80;
    // 心拍が反応する最長距離のアルベド値が最大または最小になるまでの時間を指定します。
    [SerializeField]
    private float albedoChangeTime = 1.5f;
    // アルベド値を上昇させるか下降させるかを判定
    [SerializeField]
    private bool up = true;
    // コントローラーのモータースピードに加算する値を指定します。
    [SerializeField]
    private float motorSpeedPluse = 0.2f;
    public bool Damage { get => damage; set => damage = value; }
    // 敵に捕まり、攻撃を受けたか判定
    private bool damage = false;

    // HartBresAnimatorのパラメーターID
    static readonly int speedId = Animator.StringToHash("Speed");
    static readonly int startId = Animator.StringToHash("Start");
    static readonly int idleId = Animator.StringToHash("Idle");

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
        gamepad = Gamepad.current;
    }

    void Update()
    {
        // 指定したレイヤーが範囲内に入った際に、対象と自身の距離を計算します。
        SearchEnemyForDistance(hearingArea.transform, enemyLayer);
        
        if(!Damage)
        {
            if (onHearing)   // 敵が近くにいることに気付き緊張している状態
            {
                if (count == 0)
                {
                    animator.SetTrigger(idleId);
                    count++;
                }
                // 対象と自身の距離に応じてheartBresAudioのボリュームとピッチを変更します。
                HeartBresDistanceAudioManager();
                // 対象と自身の距離に応じてImageManagerのアルベド値の更新処理をします。
                HeartBresImageManager();
            }
            else if (!onHearing && count > 0)   // 緊張していない状態
            {
                animator.SetTrigger(startId);
                OffBresBeat();
                albedoValue = 0;
                heartBresImage.color = new Color(255, 255, 255, albedoValue);
                count = 0;
            }
        }
        else if(count > 0 && Damage)
        {
            // ゲームオーバー時の鼓動の減少処理
            HeartBreasGraduallySmaller();
        }
    }

    // 指定したレイヤーが範囲内に入った際に、対象と自身の距離を計算します。
    private void SearchEnemyForDistance(Transform searchArea, LayerMask overlapLayerMask)
    {
        // 判定する範囲とレイヤーを指定し、範囲内にレイヤーが入った場合colliderに格納
        Collider[] collider = Physics.OverlapSphere(
                                searchArea.position,
                                searchArea.transform.localScale.x,
                                overlapLayerMask);

        // colliderにレイヤーを格納している場合処理する。
        if (collider.Length > 0)
        {
            onHearing = true;
            Vector3 myPosition = transform.position;
            
            if(collider.Length == 1)    // colliderに格納している数が1つだった場合
            {
                distance = (myPosition - collider[0].transform.position).magnitude;
                return;
            }
            if (collider.Length > 1)     // colliderに格納している数が2つ以上だった場合
            {
                // colllider内の0配列目の位置と、自分との距離を取得
                float minDistance = (myPosition - collider[0].transform.position).magnitude;
                int i;
                // 一番距離が近いオブジェクトの距離を選定
                for(i = 0;i < collider.Length - 1;i++)
                {
                    float colliderDistance = (myPosition - collider[i + 1].gameObject.transform.position).magnitude;
                    if (minDistance > colliderDistance)
                    {
                        minDistance = colliderDistance;
                    }
                }
                distance = minDistance;
            }
        }
        else
        {
            onHearing = false;
        }
    }
    // 対象と自身の距離に応じてheartBresAudioのボリュームとピッチを変更します。
    private void HeartBresDistanceAudioManager()
    {
        heartBresAudio.volume = (hearingArea.transform.localScale.x - distance) / hearingArea.transform.localScale.x;
        heartBresAudio.pitch = (hearingArea.transform.localScale.x - distance) / hearingArea.transform.localScale.x + pitchPlus;
        animator.SetFloat(speedId, heartBresAudio.pitch);
    }

　　// 敵の攻撃を受け、鼓動が徐々に小さくかつ、遅くなるように処理
    private void HeartBreasGraduallySmaller()
    {
        if(heartBresAudio.volume > 0)
        {
            // volumeを小さくする
            heartBresAudio.volume -= Time.deltaTime * 0.1f;
            // pitchを減少
            heartBresAudio.pitch -= Time.deltaTime * 0.1f;
            // pitchにあわせてAnimatorの速度を操作
            animator.SetFloat(speedId, heartBresAudio.pitch);
        }
        else  
        {
            count = 0;
            // アニメーターを遷移して鼓動を止める
            animator.SetTrigger(startId);
            OffBresBeat();
        }
    }

    // 対象と自身の距離に応じてImageManagerのアルベド値の更新処理をします。
    private void HeartBresImageManager()
    {
        // 敵との距離に応じてのheartBresImageの最大アルベド値を指定します。
        float albedoTopValue = ((hearingArea.transform.localScale.x - distance) / hearingArea.transform.localScale.x) * maxAlbedoValue;
        // 現在のアルベド値が最大または最小になるまでの時間を指定します。
        float albedoChangeTimeNow = albedoChangeTime - (hearingArea.transform.localScale.x - distance) / hearingArea.transform.localScale.x;
        if ((0 > albedoValue) && !up) // 下降中にアルベド値が最小値を超えた場合
        {
            up = true;
        }
        else if((albedoTopValue < albedoValue) && up) // 上昇中にアルベド値が最大値を超えた場合
        {
            up = false;
        }

        if(up)  // アルベド値を上昇させる。
        {
            albedoValue += Time.deltaTime * albedoTopValue / albedoChangeTimeNow;
        }
        else // アルベド値を下降させる。
        {
            albedoValue -= Time.deltaTime * albedoTopValue / albedoChangeTimeNow;
        }
        // アルベド値を更新
        heartBresImage.color = new Color(1, 1, 1,albedoValue / 100);
    }
    // コントローラーを振動させる
    public void HeartBresBeet()
    {
        if(gamepad != null)
        {
            float motorSpeed = heartBresAudio.volume + motorSpeedPluse;
            gamepad.SetMotorSpeeds(motorSpeed, motorSpeed);
        }
    }
    // コントローラーの振動を停止する
    public void OffBresBeat()
    {
        if(gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }

}
