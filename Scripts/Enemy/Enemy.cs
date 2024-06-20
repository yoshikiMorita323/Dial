using UnityEngine;

[RequireComponent(typeof(SearchManager))]
[RequireComponent(typeof(NaviMesh))]

// 敵の挙動処理
public class Enemy : MonoBehaviour
{
    SearchManager searchManager;
    NaviMesh naviMesh;
    AudioSource audioSource;

    // WheelChairControllerスクリプトを指定
    [SerializeField]
    private WheelChairController wheelChairController = null;
    // EnemyModelを指定
    [SerializeField]
    private EnemyModel enemyModel = null;
    // playerの位置情報を取得
    [SerializeField]
    private Transform player = null;
    // playerScriptを指定
    [SerializeField]
    private Player playerScript = null;
    [SerializeField]
    private float currentTime = 0;
    // 見失ってから通常状態に遷移するまでの時間を指定
    [SerializeField]
    private float loseSightTime = 2;
    // 警戒状態から警戒する条件が消えてから通常状態に戻るまでの時間を指定
    [SerializeField]
    private float warningTime = 1;
    // プレイヤーを捕まえてからゲームオーバーになるまでの時間を指定。
    [SerializeField]
    private float onGameOverTime = 2;
    // 状態にあわせた移動速度を指定
    [SerializeField]
    private float runSpeed = 3.5f;
    [SerializeField]
    private float walkSpeed = 2;

    // プレイヤーのレイヤー番号を指定
    [SerializeField]
    private int playerLayerNomber = 0;
    [SerializeField]
    private LayerMask playerLayerMask = default;
    // 聞き取る対象のLayerを指定
    [SerializeField]
    private LayerMask hearingSoundLayer = default;
    // 音のなった位置を格納
    [SerializeField]
    private Vector3 soundPosition = Vector3.zero;
    // 目的地を指定
    [SerializeField]
    private Vector3 targetPosition = Vector3.zero;

    // LookSearchAreaを指定
    [SerializeField]
    private Transform lookSearchArea = null;
    // ChatchAreaを指定
    [SerializeField]
    private GameObject catchArea = null;
    // hearableRangeを指定
    [SerializeField]
    private Transform hearableRange = null;
    // プレイヤーを追いかけているか判定
    private bool chase = false;
    // 待機状態か判定
    private bool stayMove = false;
    // プレイヤーを発見した際の音を指定
    [SerializeField]
    private AudioClip discoverOnSound = null;
    // プレイヤーを追いかけている際の音を指定
    [SerializeField]
    private AudioSource chaseOnBGM = null;
    // discoverSoundのVolumeを指定
    [SerializeField]
    private float discoverSoundVolume = 0.1f;
    // chaseAudioのvolumeを指定
    private float chaseAudioVolume = 0;
    // chaseAudioのフェードアウトするまでの間隔を指定
    [SerializeField]
    private float fadeOutTime = 3;
    // 追跡時の音が鳴っているか否か
    private bool onChaseAudio = false;

    private int count = 0;

    // ステートを作成
    enum EnemyState
    {
        // 待機状態
        Stay,
        // 通常状態
        Idle,
        // 発見状態
        Discover,
        // 見失う
        LoseSight,
        // 警戒状態
        Warning,
        // 捕まえる
        Catch
    }
    [SerializeField]
    EnemyState currentState = EnemyState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        searchManager = GetComponent<SearchManager>();
        naviMesh = GetComponent<NaviMesh>();
        audioSource = GetComponent<AudioSource>();
        chaseAudioVolume = chaseOnBGM.volume;

        // 移動速度を歩行時に変更
        naviMesh.ChengeSpeed(walkSpeed);
        // 車輪の回転速度を変更
        wheelChairController.InputObjectSpeed(walkSpeed);
    }

    // 立ち止まっている状態
    private void SetStayState()
    {
        // 目的地についた際に立ち止まるように処理
        naviMesh.AutoBraking(true);
        // 車輪の回転速度を変更
        wheelChairController.InputObjectSpeed(0);
        // 移動目標を現在地にして、待機させる。
        naviMesh.SetTargetPosition(transform.position);
        // 待機
        stayMove = true;
    }
    // 
    public void MoveStart()
    {
        SetIdleState();
        // 目的地についても立ち止まらないよに処理
        naviMesh.AutoBraking(true);
    }

    private void SetIdleState()
    {
        // 巡回ルートに戻る
        naviMesh.RootSetDestination();
        // 移動速度を歩行時に変更
        naviMesh.ChengeSpeed(walkSpeed);
        // 車輪の回転速度を変更
        wheelChairController.InputObjectSpeed(walkSpeed);
        // 追跡解除
        chase = false;
        // ステートを遷移
        currentState = EnemyState.Idle;
    }
    private void SetDiscoverState()
    {
        // EnemyModelのステート遷移処理
        enemyModel.SetModelDiscover();
        // 移動速度を変更
        naviMesh.ChengeSpeed(runSpeed);
        // 車輪の回転速度を変更
        wheelChairController.InputObjectSpeed(runSpeed);
        // ステートを遷移
        currentState = EnemyState.Discover;
        // 通常状態からステート遷移した際に音を鳴らす
        if(!chase)
        {
            audioSource.PlayOneShot(discoverOnSound, discoverSoundVolume);
            chaseOnBGM.Play();
            chaseOnBGM.volume = chaseAudioVolume;
        }
        chase = true;
        onChaseAudio = true;
        // GameOver判定エリアをだす
        catchArea.SetActive(true);
    }
    private void SetLoseSightState()
    {
        // 目的地についた際に立ち止まるように処理
        naviMesh.AutoBraking(true);
        // GameOver判定エリアを消す
        catchArea.SetActive(false);
        // EnemyModelのステート遷移処理
        enemyModel.SetModelLoseSight();
        // ステートを遷移
        currentState = EnemyState.LoseSight;
    }

    private void SetWarningState()
    {
        // 移動速度を歩行時に変更
        naviMesh.ChengeSpeed(walkSpeed);
        // 移動目標を音が鳴った位置にする
        naviMesh.SetTargetPosition(soundPosition);
        // 目的地についた際に立ち止まるように処理
        naviMesh.AutoBraking(true);
        // EnemyModelのステート遷移処理
        enemyModel.SetModelWarning();
        // ステートを遷移
        currentState = EnemyState.Warning;
    }

    private void SetCatchState()
    {
        // 移動を停止
        naviMesh.ChengeSpeed(0);
        // 車いすの回転を停止
        wheelChairController.InputObjectSpeed(0);
        // EnemyModelのステート遷移処理
        enemyModel.SetModelCatch();
        naviMesh.NaviMeshLift();
        currentState = EnemyState.Catch;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            // 待機状態
            case EnemyState.Stay:
                if (!stayMove) SetStayState();
                break;
            // 通常状態
            case EnemyState.Idle:
                UpdateForIdleState();
                break;
            // 発見状態
            case EnemyState.Discover:
                UpdateForDiscoverState();
                break;
            // 追跡解除
            case EnemyState.LoseSight:
                UpdateForLoseSightState();
                break;
            // 警戒状態
            case EnemyState.Warning:
                UpdateForWarningState();
                break;
            // プレイヤーを捕まえた状態
            case EnemyState.Catch:
                UpdateForCatchState();
                break;
            default:
                break;
        }
    }

    private void UpdateForIdleState()   // 通常状態
    {
        // 対象相手を指定して自分の視界範囲にいた場合searchをtrueにする
        searchManager.SearchAreaManager(lookSearchArea,playerLayerMask,playerLayerNomber);
        // 対象の音を聞き取る
        Collider playerSoundCollider = searchManager.SoundSearchCollider(hearableRange,hearingSoundLayer);
        // 音が聞こえた場合
        if (playerSoundCollider != null)
        {
            soundPosition = playerSoundCollider.transform.position;
            // ステート遷移
            SetWarningState();   
        }
        // プレイヤーを目視で見つけた場合
        if(searchManager.Search)
        {
            // ステート遷移
            SetDiscoverState();   
        }
        if(onChaseAudio)
        {
            // chaseOnBGMをフェードアウト
            AudioSourceFadeOutManager(chaseOnBGM, chaseAudioVolume, fadeOutTime);
        }
    }
    private void UpdateForDiscoverState()   // プレイヤーを発見した状態
    {
        // 対象相手を指定して自分の視界範囲にいた場合searchをtrueにする
        searchManager.SearchAreaManager(lookSearchArea, playerLayerMask, playerLayerNomber);
        if(searchManager.Search)
        {
            targetPosition = player.transform.position;
        }
        else if (!searchManager.Search)  // 見失う
        {
            // プレイヤーから発する音を聞き取る
            Collider soundCollider = searchManager.SoundSearchCollider(hearableRange, hearingSoundLayer);

            if(soundCollider != null)
            {
                // 音の発した位置に目的地を変更する
                targetPosition = soundCollider.gameObject.transform.position;
                // 見失った位置を更新
                targetPosition = soundCollider.gameObject.transform.position;
            }
            else
            {
                // 見失った位置に目的地を変更する
                targetPosition = searchManager.LostPosition;
                //　ステート遷移
                SetLoseSightState();
            }
        }
        // 目的地を更新
        naviMesh.SetTargetPosition(targetPosition);
    }
    private void UpdateForLoseSightState()  // プレイヤーを追跡できなくなった状態
    {
        // 対象相手を指定して自分の視界範囲にいた場合searchをtrueにする
        searchManager.SearchAreaManager(lookSearchArea, playerLayerMask, playerLayerNomber);
        // chaseOnBGMをフェードアウト
        AudioSourceFadeOutManager(chaseOnBGM,chaseAudioVolume,fadeOutTime);
        // 対象の音を聞き取る
        Collider playerSoundCollider = searchManager.SoundSearchCollider(hearableRange, hearingSoundLayer);
        // 目的地との距離を計算
        float distance = PlaneDistanceManager(transform.position, targetPosition);
        // 目的地について立ち止まった際に車輪の回転を停止
        if (distance < 0.1f && count == 0)
        {
            wheelChairController.InputObjectSpeed(0);
            count++;
        }
        // 処理時間を指定する
        currentTime += Time.deltaTime;
        // IdleStateに遷移;
        if(currentTime > loseSightTime)
        {
            SetIdleState();
            currentTime = 0;
            chase = false;
            naviMesh.AutoBraking(false);
            count = 0;
        }
        // 見つけた際にDiscoverStateに遷移
        if(searchManager.Search)
        {
            SetDiscoverState();
            currentTime = 0;
            naviMesh.AutoBraking(false);
            chaseOnBGM.volume = chaseAudioVolume;
            count = 0;
        }
        // 音が聞こえた場合
        if (playerSoundCollider != null)
        {
            soundPosition = playerSoundCollider.transform.position;
            SetWarningState();   // ステート遷移
            count = 0;
        }
    }

    private void UpdateForWarningState()    // プレイヤーのアクションにより警戒している状態
    {
        // プレイヤーから発する音を聞き取る
        Collider soundCollider = searchManager.SoundSearchCollider(hearableRange,hearingSoundLayer);
        
        // 対象相手を指定して自分の視界範囲にいた場合searchをtrueにする
        searchManager.SearchAreaManager(lookSearchArea, playerLayerMask, playerLayerNomber);

        if (soundCollider != null)
        {
            soundPosition = soundCollider.transform.position;
            // 移動目標を音の位置に更新する
            naviMesh.SetTargetPosition(soundCollider.transform.position);
            if (currentTime > 0) // 探知外に一度出た場合の処理
            {
                currentTime = 0;
                count = 0;
            }
        }
        // 音が聞こえなくなった場合
        if (soundCollider == null)
        {
            // 目的地との距離を計算
            float distance = PlaneDistanceManager(transform.position, soundPosition);
            if(distance < 2)
            {
                // 目的地について立ち止まった際に車輪の回転を停止
                if (distance < 0.1f && count == 0)
                {
                    wheelChairController.InputObjectSpeed(0);
                    count++;
                }
                // 経過時間を加算
                currentTime += Time.deltaTime;
                if (currentTime > warningTime)
                {
                    // ステート遷移
                    SetIdleState();
                    currentTime = 0;
                    naviMesh.AutoBraking(false);
                    // EnemyModelの警戒解除処理
                    enemyModel.SetModelReleaseWarning();
                    count = 0;
                }
            }
        }
        // 目視でプレイヤーを見つけた
        if (searchManager.Search)
        {
            // ステート遷移
            SetIdleState();
            naviMesh.AutoBraking(false);
            currentTime = 0;
            count = 0;
        }
    }
    // プレイヤーを捕まえた状態
    private void UpdateForCatchState()  
    {
        // プレイヤーの方向を向かせる
        LookManager(player.transform.position);

        currentTime += Time.deltaTime;
        if((currentTime > onGameOverTime) && count == 0)
        {
            playerScript.SetGameOverState();
            count++;
        }
    }

    // AudioSourceのボリュームをフェードアウトさせます。
    // (対象のAudioSource,対象の現在のボリューム ,ボリュームが０になるまでの時間)
    private void AudioSourceFadeOutManager(AudioSource audioSource,float audioVolume, float fadeOutTime)
    {
        audioSource.volume -= Time.deltaTime * (audioVolume / fadeOutTime);
        if (audioSource.volume <= 0)
        {
            audioSource.volume = 0;
            onChaseAudio = false;
            audioSource.Stop();
        }
    }
    // 高さが一定である場合の距離を計算
    private float PlaneDistanceManager(Vector3 myPosition,Vector3 partnerPosition)
    {
        float distance = (new Vector3(partnerPosition.x,0,partnerPosition.z)
                          - new Vector3(myPosition.x,0,myPosition.z)).magnitude;
        return distance;
    }

    // 相手の方向を向かせる処理
    private void LookManager(Vector3 partnerPosition)
    {
        Vector3 distanceVector = (partnerPosition - transform.position).normalized;
        float angleY = (Mathf.Atan2(distanceVector.x, distanceVector.z) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Euler(0, angleY, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーと接触した場合
        if(other.gameObject.CompareTag("Player"))
        {
            if(currentState != EnemyState.Catch)
            {
                // ステート遷移
                SetCatchState();
            }
        }
    }
}
