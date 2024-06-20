using UnityEngine;
using UnityEngine.InputSystem;

// ユーザー入力からのプレイヤーキャラクターの操作を表します。
[RequireComponent(typeof(MoveBehaviour), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    // カメラの位置を指定します。
    [SerializeField] 
    private Transform cameraPoint = null;
    // spotLightを指定します。
    [SerializeField]
    private Transform spotLight = null;
    // ライトのスクリプトを指定します
    [SerializeField]
    private Flashlight flashLight = null;
    // ライトのアニメーターを指定
    [SerializeField]
    private Animator flashLightAnimator = null;
    [SerializeField]
    private Animator stageSceneAnimator = null;
    // flashLightAnimatorを動作させているか否か
    private bool onFlashLightanimator = false;

    // 指定した金庫を格納します。
    private GameObject safeObject;
    // 指定した金庫のSafeManagerスクリプトを格納します。
    private SafeManager safeManager;
    // 金庫のダイヤルを低速で回すかを判定
    private bool dialLowSpeed = false;
    // 取得した鍵の数を指定
    private int keyCount = 0;
    // 敵の位置情報を格納
    private Vector3 enemyPosition = Vector3.zero;

    MoveBehaviour moveBehaviour;
    CenterViewManager centerViewManager;
    ThrowingManager throwingManager;
    HeartbeatManager heartbeatManager;
    AudioSource audioSource;
    PlayerInput playerInput;
    Animator animator;
    Gamepad gamepad;

    // 移動キーの入力を格納
    Vector2 moveInput = Vector2.zero;
    Vector2 lookInput = Vector2.zero;
    // カメラのピッチを格納
    private float cameraPitch = 0;
    // しゃがんでいるか判定
    private bool squatDown = false;

    enum PlayerState
    {
        // 通常状態
        Idle,
        // 走る
        Run,
        // しゃがむ
        Squat,
        // 金庫操作状態
        SafeControl,
        // ポーズ
        Pause,
        // 敵に捕まる
        GetCaught,
        // ゲームオーバー
        GameOver,
        // ゲームクリアー
        GameClear,
    }
    [SerializeField]
    PlayerState currentState = PlayerState.Idle;
    // ポーズ画面に移行する直前のcurrentStateを指定
    PlayerState pauseOnState = PlayerState.Idle;
    // 歩行時の速度を指定
    [SerializeField]
    private float walkSpeed = 3;
    // 走行時の速度を指定
    [SerializeField]
    private float runSpeed = 5;
    // しゃがみ時の速度を指定
    [SerializeField]
    private float squatSpeed = 2;

    #region SoundOption
    // 足音のAudioSourceを指定
    [SerializeField]
    private AudioSource footStepsOnShound = null;
    // 呼吸音のAudioSourceを指定
    [SerializeField]
    private AudioSource shortBreathOnSound = null;
    // ダメージ時の音を指定
    [SerializeField]
    private AudioClip damageOnSound = null;
    // ダメージ音の音量を指定
    [SerializeField]
    private float damageVolume = 1;
    // 走行時のshortBreathOnSoundのボリュームを指定
    [SerializeField]
    private float shortBreathVolume = 0.02f;
    private bool onAudio = false;

    [SerializeField]
    private SphereCollider soundArea = null;
    [SerializeField]
    private float runSoundArea = 8;
    [SerializeField]
    private float walkSoundArea = 4;
    [SerializeField]
    private float squatSoundArea = 0.1f;
    [SerializeField]
    private float runAudioPitchSize = 1.3f;
    [SerializeField]
    private float walkAudioPitchSize = 0.6f;
    [SerializeField]
    private float squatAudioPitchSize = 0.6f;
    [SerializeField]
    private float runVolume = 1;
    [SerializeField]
    private float walkVolume = 0.6f;
    [SerializeField]
    private float squatVolume = 0.2f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        moveBehaviour = GetComponent<MoveBehaviour>();
        centerViewManager = GetComponent<CenterViewManager>();
        throwingManager = GetComponent<ThrowingManager>();
        heartbeatManager = GetComponent<HeartbeatManager>();
        audioSource = GetComponent<AudioSource>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        // 移動速度を歩行速度に変更
        moveBehaviour.MoveSpeedManager(walkSpeed);
        // マウスカーソルを非表示にする
        Cursor.visible = false;
        // マウスカーソルを画面中央に固定する
        Cursor.lockState = CursorLockMode.Locked;
    }

    public bool IsMouse { get { return playerInput.currentControlScheme == "Keyboard&Mouse"; } }
    
    // 移動操作： WASD or 左スティック
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>().normalized;
        }
        else if (context.canceled)
        {
            moveInput = Vector2.zero;
        }

        switch(currentState)
        {
            case PlayerState.GameOver:
                GameOver.Instance.moveInput = moveInput;
                break;
            case PlayerState.Pause:
                Pause.Instance.moveInput = moveInput;
                break;
        }
    }
    // 視点操作；マウス操作 or 右スティック
    public void OnLook(InputAction.CallbackContext context)
    {
        if(!moveBehaviour.rotateStop)
        {
            lookInput = context.ReadValue<Vector2>();
            // コントローラーの場合
            if (!IsMouse)
            {
                lookInput *= Time.deltaTime;
            }
        }
    }
    // 右スティック操作更新処理
    private void Look()
    {
        if(lookInput.sqrMagnitude > 0.01f)
        {
            // キャラクターの方向を回転させる
            moveBehaviour.Rotate(lookInput.x);
            // 視点のピッチ角を回転
            cameraPitch += lookInput.y * moveBehaviour.RotationSpeed;
            // 視点のピッチ角を回転
            cameraPitch += lookInput.y * moveBehaviour.RotationSpeed * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
            cameraPoint.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        }
    }

    // 走る：LShift or L2
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed && currentState == PlayerState.Idle)
        {
            // ステート遷移 （走る）
            SetRunState();
        }
        else if (context.canceled && currentState == PlayerState.Run)
        {
            shortBreathOnSound.Stop();
            // ステート遷移 （通常の状態）
            SetIdleState();
            // flashLightAnimationが継続中の場合、IdleAnimationに移行、角度を初期値に戻す
            if (onFlashLightanimator)
            {
                flashLightAnimator.SetTrigger("Idle");
                flashLightAnimator.enabled = false;
                spotLight.transform.localRotation = Quaternion.identity;
                onFlashLightanimator = false;
            }
        }
    }


    // しゃがむ　or 金庫操作時、ダイヤル低速回転； Contrl or  L1トリガー
    public void OnSquat(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch(currentState)
            {
                case PlayerState.Idle:
                case PlayerState.Run:
                    //  ステート遷移（しゃがむ）
                    SetSquatState();
                    break;
                case PlayerState.SafeControl:
                    // ダイヤル回転速度を低速に更新
                    dialLowSpeed = true;
                    safeManager.LowSpeedDial(dialLowSpeed);
                    break;
            }
        }
        else if (context.canceled)
        {
            switch (currentState)
            {
                case PlayerState.Squat:
                    // ステート遷移（通常の状態）
                    SetIdleState();
                    squatDown = false;
                    break;
                case PlayerState.SafeControl:
                    // ダイヤル回転速度を通常速度に更新
                    dialLowSpeed = false;
                    safeManager.LowSpeedDial(dialLowSpeed);
                    break;
            }
        }
    }

    // ライトON、OFF：Space or R3
    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            flashLight.LightSwitch();
        }
    }

    // クロスヘアによるアクション：左クリック or SouthButton
    public void OnAction(InputAction.CallbackContext context)     
    {
        if (context.performed)
        {
            Action();
        }
    }

    private void Action()
    {
        // rayの当たっているコライダーに応じて処理を行う
        switch (centerViewManager.objectLayer)
        {
            case 7:     // ビン
                GameObject bottle = centerViewManager.hitCrossHairRay.collider.gameObject;
                        // ビンを取得
                bottle.GetComponent<Bottle>().PickUpBottle();
                        // 投てき出来るビンの本数を加算
                throwingManager.BottleStorage(bottle);
                break;
            case 13:    // 金庫
                if (currentState != PlayerState.SafeControl
                    & currentState != PlayerState.Pause)
                {
                        // 金庫の操作に移行
                    SetSafeControlState();
                    return;
                }
                break;
            case 21:    // 閉まっているドア
            case 22:    // 開いているドア
                GameObject door = centerViewManager.hitCrossHairRay.collider.gameObject;
                        // ドアの開閉状況にあわせて動かす
                door.GetComponent<DoorAccess>().MoveDoor();
                break;
            case 24:    // 鍵
                GameObject key = centerViewManager.hitCrossHairRay.collider.gameObject;
                        // 鍵を取得
                key.GetComponent<KeyManager>().GetKey();
                keyCount++;
                break;
            case 28:    // 鍵1つ扉開錠
            case 29:    // 鍵2つ扉開錠
            case 30:    // 鍵3つ扉開錠
                GameObject goolLock = centerViewManager.hitCrossHairRay.collider.gameObject;
                        // 鍵の取得数に応じて動作を処理する
                goolLock.GetComponent<LockKeyManager>().LockOpen(keyCount);
                break;
            default:
                break;
        }
        // ステートに応じて処理
        switch(currentState)
        {
            case PlayerState.SafeControl:   // 金庫操作中
                // 金庫操作解除
                LiftSafeControl();
                // ステート遷移（通常の状態）
                SetIdleState();
                break;
            case PlayerState.GameClear:
                StageClear.Instance.action = true;
                break;
            case PlayerState.GameOver:
                GameOver.Instance.action = true;
                break;
            case PlayerState.Pause:
                Pause.Instance.action = true;
                break;
        }
    }

    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // ステートに応じて処理
            switch (currentState)
            {
                case PlayerState.GameClear:
                    StageClear.Instance.action = true;
                    break;
                case PlayerState.GameOver:
                    GameOver.Instance.action = true;
                    break;
                case PlayerState.Pause:
                    Pause.Instance.action = true;
                    break;
            }
        }
    }

    // 金庫のダイヤルを左に回転
    public void OnSafeDialLeftRotate(InputAction.CallbackContext context)
    {
        if (currentState == PlayerState.SafeControl)
        {
            float move = -1 * context.ReadValue<float>();
            if (move > -0.1f)
            {
                move = 0;
            }
            safeManager.OnDialeRotation(new Vector2(move, 0));
        }
    }
    // 金庫のダイヤルを右に回転
    public void OnSafeDialRightRotate(InputAction.CallbackContext context)
    {
        if (currentState == PlayerState.SafeControl)
        {
            float move = context.ReadValue<float>();
            if (move < 0.1f)
            {
                move = 0;
            }
            safeManager.OnDialeRotation(new Vector2(move, 0));
        }

    }

    // 投てき: 右クリック or R1トリガー
    public void OnThrowing(InputAction.CallbackContext context)
    {
        // ビンの所持数が1以上の時、ビンを投げる
        if(context.performed && throwingManager.StorageCount > 0)
        {
            throwingManager.ThrowingController();
        }
    }

    // バッテリー充電：Q or R2トリガー
    public void OnChage(InputAction.CallbackContext context)
    {
        switch(currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Run:
            case PlayerState.Squat:

                // 長押し中はバッテリー充電
                if (context.performed)
                {
                    flashLight.OnCharge = true;
                }
                else if (context.canceled)      // 充電解除
                {
                    flashLight.OnCharge = false;
                }
                break;
        }
    }

    // 開錠：E or Southボタン
    public void OnSafeOpen(InputAction.CallbackContext context)
    {
        if (context.performed && currentState == PlayerState.SafeControl)
        {
           safeManager.SafeOpenTry();
        }
    }

    // ポーズ画面切り替え；ESC or opsion
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (currentState)
            {
                case PlayerState.Idle:
                case PlayerState.Run:
                case PlayerState.Squat:
                case PlayerState.SafeControl:
                    // ステート遷移（ポーズ）
                    SetPauseState();
                    break;
                case PlayerState.Pause:
                    // ステート遷移（通常の状態）
                    PauseOutState();
                    // ポーズ画面解除
                    Pause.Instance.ButtonNo = 0;
                    Pause.Instance.action = true;
                    break;
            }
        }
    }
    // 通常の状態
    private void SetIdleState()
    {
        // AudioSourceを歩行時のステータスに変更
        AudioSourceManager(footStepsOnShound, walkAudioPitchSize, walkVolume);
        // 足音が発する範囲を歩行時の範囲に変更
        soundArea.radius = walkSoundArea;
        // 移動速度を変更
        moveBehaviour.MoveSpeedManager(walkSpeed);
        currentState = PlayerState.Idle;
    }

    // 走る
    private void SetRunState()
    {
        // AudioSourceを走行時のステータスに変更
        AudioSourceManager(footStepsOnShound, runAudioPitchSize, runVolume);
        shortBreathOnSound.Play();
        // 足音が発する範囲を走行時の範囲に変更
        soundArea.radius = runSoundArea;
        // 移動速度を変更
        moveBehaviour.MoveSpeedManager(runSpeed);
        // ステート遷移（走る）
        currentState = PlayerState.Run;
    }

    // しゃがむ
    private void SetSquatState()
    {
        // AudioSourceをしゃがみ時のステータスに変更
        AudioSourceManager(footStepsOnShound, squatAudioPitchSize, squatVolume);
        // 足音が発する範囲をしゃがみ時の範囲に変更
        soundArea.radius = squatSoundArea;
        squatDown = true;
        // 移動速度を変更
        moveBehaviour.MoveSpeedManager(squatSpeed);
        currentState = PlayerState.Squat;
    }

    // 金庫操作
    private void SetSafeControlState()
    {
        // playerの回転を停止する
        moveBehaviour.rotateStop = true;
        // 指定している金庫オブジェクトを格納
        safeObject = centerViewManager.hitCrossHairRay.collider.gameObject;
        // 指定している金庫のSafeManagerスクリプトを格納
        safeManager = safeObject.GetComponent<SafeManager>();
        // 指定した金庫を操作する
        safeManager.OnSafeControl();
        // spotLightの位置及び角度を金庫のライト固定位に変更
        spotLight.transform.position = safeManager.spotLitePosition.transform.position;
        spotLight.transform.rotation = safeManager.spotLitePosition.transform.rotation;

        // クロスヘアのRay機能を停止してテキストを非表示にする
        centerViewManager.StopRay();
        // ステート遷移
        currentState = PlayerState.SafeControl;
        // 移動速度を変更
        moveBehaviour.MoveSpeedManager(0);
    }

    // 金庫操作モードを解除
    private void LiftSafeControl()
    {
        // playerの回転停止状態を解除する
        moveBehaviour.rotateStop = false;
        // 金庫の操作を停止する
        safeManager.OffSafeControl();
        // spotLightの位置及び角度を通常の位置に戻す
        spotLight.transform.localPosition = Vector3.zero;
        spotLight.transform.localRotation = Quaternion.identity;
        // クロスヘアのRay機能を起動してテキストを表示するようにする
        centerViewManager.StartUpRay();
        safeObject = null;
    }

    // ポーズ
    private void SetPauseState()
    {
        // ポーズ画面に移行する直前のステートを格納
        pauseOnState = currentState;
        // プレイヤーの回転を停止
        moveBehaviour.rotateStop = true;
        // ポーズ画面を表示
        StageScene.Instance.Pause();
        // ステート遷移（ポーズ）
        currentState = PlayerState.Pause;
    }

    // ポーズ画面からゲームを継続する場合の処理
    private void PauseOutState()
    {
        // プレイヤーの回転を有効にする
        moveBehaviour.rotateStop = false;

        switch (pauseOnState)
        {
            case PlayerState.Idle:
            case PlayerState.Run:
            case PlayerState.Squat:
                SetIdleState();
                break;
            case PlayerState.SafeControl:
                currentState = PlayerState.SafeControl;
                break;
        }
    }
    // 敵に捕まる
    private void SetGetCaughtState()
    {
        // マウスカーソルでの回転を無効にする
        moveBehaviour.rotateStop = true;
        // ダメージ音を鳴らす
        audioSource.PlayOneShot(damageOnSound, damageVolume);
        // アニメーションの遷移
        stageSceneAnimator.SetTrigger("Damage");
        flashLightAnimator.enabled = false;
        // rigidbodyのisKinematicをtrueにする
        moveBehaviour.BodyKinematic();
        // 敵の方向を向かせる
        LookManager(enemyPosition);
        // 移動を停止
        moveBehaviour.MoveSpeedManager(0);
        // 足音を停止
        footStepsOnShound.Stop();
        // ステート遷移（捕まる）
        currentState = PlayerState.GetCaught;
    }
    //　ゲームオーバー
    public void SetGameOverState()
    {
        StageScene.Instance.GameOver();
        heartbeatManager.Damage = true;
        currentState = PlayerState.GameOver;
    }
    // ゲームクリア
    private void SetGemeClearState()
    {
        moveBehaviour.MoveSpeedManager(0);
        StageScene.Instance.StageClear();
        currentState = PlayerState.GameClear;
    }

    void Update()
    {
        // 移動入力をプレイヤーの移動に反映
        moveBehaviour.Move(moveInput);
        // 方向入力をプレイヤーの視点移動に反映
        Look();
        // しゃがみ入力を反映
        moveBehaviour.SquatBodyManager(squatDown);
        // プレイヤーの体動による影響を処理
        MoveManager();
        // ポーズ解除時のステート遷移
        if(currentState == PlayerState.Pause)
        {
            if(Time.timeScale == 1)
            {
                PauseOutState();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // ゲームクリアー時の処理
        if(other.gameObject.CompareTag("GameClear"))
        {
            SetGemeClearState();
        }

        // 敵に当たった際の処理
        if (other.gameObject.CompareTag("CatchArea"))
        {
            enemyPosition = other.transform.position;
            SetGetCaughtState();
        }
    }
    // AudioSource内のステータスを変更
    private void AudioSourceManager(AudioSource audioName,float pitchSize,float volumeSize)
    {
        audioName.pitch = pitchSize;
        audioName.volume = volumeSize;
    }

    // プレイヤーの体動による影響を処理
    private void MoveManager()
    {
        // 移動入力の大きさを格納
        float move = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);
        // 移動している際の処理
        if (move >= 0.1f)
        {
            // 停止している状態から初めて移動した場合
            if(!onAudio)
            {
                switch (currentState)
                {
                    case PlayerState.Idle:
                    case PlayerState.Run:
                    case PlayerState.Squat:

                        onAudio = true;
                        soundArea.enabled = true;
                        footStepsOnShound.Play();
                        break;
                }
            }
            // 走っている際の各設定を更新
            if (currentState == PlayerState.Run)
            {
                // 呼吸音を再生
                if (shortBreathOnSound.volume == 0)
                {
                    shortBreathOnSound.volume = shortBreathVolume;
                }
                // flashLightAnimationが継続中の場合、IdleAnimationに戻す
                if (!onFlashLightanimator)
                {
                    flashLightAnimator.enabled = true;
                    flashLightAnimator.SetTrigger("Run");
                    onFlashLightanimator = true;
                }
            }
        }
        // 停止している場合
        else if(move < 0.1f)
        {
            // 移動している状態から停止した際の処理
            if(onAudio)
            {
                onAudio = false;
                soundArea.enabled = false;
                footStepsOnShound.Stop();
            }
            // 走っている際の呼吸音を停止
            if (shortBreathOnSound.volume > 0)
            {
                shortBreathOnSound.volume = 0;
                // flashLightAnimationが継続中の場合、IdleAnimationに移行、角度を初期値に戻す
                if (onFlashLightanimator)
                {
                    flashLightAnimator.SetTrigger("Idle");
                    flashLightAnimator.enabled = false;
                    spotLight.transform.localRotation = Quaternion.identity;
                    onFlashLightanimator = false;
                }
            }
        }
    }

    // 相手の方向を向かせる処理
    private void LookManager(Vector3 partnerPosition)
    {
        Vector3 distanceVector = (partnerPosition - transform.position).normalized;
        float angleY = (Mathf.Atan2(distanceVector.x, distanceVector.z) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angleY, transform.eulerAngles.z);
    }
}
