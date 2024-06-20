using UnityEngine;
using UnityEngine.InputSystem;

// 金庫のダイヤル回転、当たり番号の設定、操作時、操作解除時の挙動処理
public class SafeManager : MonoBehaviour
{
    new Collider collider;
    Gamepad gamepad;
    SafeAnimation safeAnimation;

    // パッドの振動モーター回転速度を指定
    [SerializeField]
    private float openDialPadMotorSpeed = 1;
    // パッドの振動時間を指定
    [SerializeField]
    private float motorMoveTime = 0.1f;
    private float onMotorMoveTimeCount = 0;
    // パッドが振動しいるか判定
    private bool onMotorMove = false;
    private int motorMoveCount = 0;
    // 金庫操作時のカメラを指定
    [SerializeField]
    private GameObject safeCamera = null;
    [SerializeField]
    private GameObject dial = null;
    // crossHairImageを指定
    [SerializeField]
    private GameObject crossHairImage = null;
    // 解除失敗時の音の範囲を指定
    [SerializeField]
    private GameObject safeSoundArea = null;
    //  safeSoundAreaの出現時間を指定
    [SerializeField]
    private float safeNotOpenSoundTime = 0.8f;
    // SafeOperationを指定
    [SerializeField]
    private OperationManager safeOperationManager;
    // SpotLitePositionを指定
    public Transform spotLitePosition = null; 

    private float soundAreaOntimeCount = 0;
    // 開錠までの段階数を指定
    [SerializeField]
    private int openStepCount = 2;
    // 現在の段階を指定
    private int stepCount = 0;
    private bool notOpen = false;
    // 開錠番号をランダムにするか
    [SerializeField]
    private bool randomNo = true;
    // 開錠番号を指定
   [SerializeField]
    private float openKeyNo = 0;
    // ダイヤル位置の角度を指定
    private float rotateAngle = 0;
    // 回転させている角度を格納
    private float angleX = 0;
    // ダイヤル位置の角度範囲を指定
    private float keyAreaAngle = 0;
    // 開錠番号位置との角度差を指定
    private float angleDifference = 0;
    // ダイヤルの最大番号を指定
    [SerializeField]
    private int maxKeyNo = 15;
    // ダイヤルの回転速度を指定
    [SerializeField]
    private float moveRotateSpeed = 50;
    [SerializeField]
    private float rotateLowSpeedDouble = 0.5f;
    private float speedGear = 1;

    // 移動入力値を格納
    private Vector2 moveInput = Vector2.zero;
    // 金庫を操作しているか否か判定
    private bool control = false;
    // 現在の角度が当たりの角度範囲内か否か判定
    private bool safeOpen = false;

    AudioSource audioSource;

    #region Audio
    // ダイヤルを回している時の音を指定
    [SerializeField]
    private AudioSource safeRotationAudio = null;
    [SerializeField]
    private float minSafePich = 2.5f;
    // 金庫を開錠した際の音を指定
    [SerializeField]
    private AudioClip openOnSound = null;
    // 開錠を失敗した際の音を指定
    [SerializeField]
    private AudioClip notOpenOnSound = null;
    // 失敗時の音量を指定
    [SerializeField]
    private float notOpenOnSoundVolume = 1;
    // 段階ごとの解除をした際の音を指定
    [SerializeField]
    private AudioClip unlockOnSound = null;
    // RotationAudioのボリュームを取得
    public float rotationVolume = 0;
    // RotationAudioのpitchを取得
    public float rotationPanStereo = 0;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<Collider>();
        gamepad = Gamepad.current;
        safeAnimation = GetComponent<SafeAnimation>();

        if (randomNo)
        {
            // 開錠番号をランダムで指定
            openKeyNo = Random.Range(0, maxKeyNo + 1);
        }
        // 開錠番号の角度を指定
        keyAreaAngle = (360 / maxKeyNo) * openKeyNo;
    }

    public void OnDialeRotation(Vector2 move)
    {
        moveInput = move;
    }

    public void LowSpeedDial(bool dialLowSpeed)
    {
        if(dialLowSpeed)
        {
            speedGear = rotateLowSpeedDouble;
        }
        else
        {
            speedGear = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(control)
        {
            SafeOperation();
        }
        // 開かなかった場合の音の範囲であるsadeSoundAreaを一定時間アクティブにする
        if (notOpen)
        {
            safeSoundArea.SetActive(true);
            soundAreaOntimeCount += Time.deltaTime;
            if (soundAreaOntimeCount >= safeNotOpenSoundTime)
            {
                safeSoundArea.SetActive(false);
                notOpen = false;
                soundAreaOntimeCount = 0;
            }
        }

    }
    // 金庫操作開始
    public void OnSafeControl()
    {
        safeCamera.SetActive(true);
        crossHairImage.SetActive(false);
        safeRotationAudio.Play();
        safeOperationManager.OnOperation(true);
        control = true;
    }
    // 金庫操作終了
    public void OffSafeControl()
    {
        safeRotationAudio.Stop();
        safeCamera.SetActive(false);
        crossHairImage.SetActive(true);
        safeOperationManager.OnOperation(false);
        control = false;
        moveInput = Vector2.zero;
    }

    private void SafeOperation()
    {
        // ダイヤルの角度を0～360度の範囲に修正
        var dialAngle = angleX % 360;
        if (angleX < 0)
        {
            dialAngle = Mathf.Abs(angleX % 360 + 360);
        }

        // ダイヤルを回す
        if (Mathf.Abs(moveInput.x) >= 0.1f)
        {
            // 開錠番号位置と現在のダイヤル番号位置の差を0～359までの値で取得
            angleDifference = (dialAngle - keyAreaAngle) % 360;
            if (angleDifference < 0)
            {
                angleDifference = (dialAngle - keyAreaAngle) % 360 + 360;
            }
            // ダイヤル回転速度に応じてダイヤル回転音のpitchを変更
            safeRotationAudio.pitch = Mathf.Abs(moveInput.x * speedGear) / 2 + minSafePich;

            if((angleDifference <= 5) || (angleDifference >= 355))
            {
                safeRotationAudio.volume = 1;
            }
            else if(angleDifference <= 15 || angleDifference >= 345)
            {
                safeRotationAudio.volume = 0.8f;
            }
            else if(angleDifference <= 30 || angleDifference >= 320)
            {
                safeRotationAudio.volume = 0.7f;
            }
            else if(angleDifference <= 90 || angleDifference >= 270)
            {
                safeRotationAudio.volume = 0.4f;
            }
            else
            {
                safeRotationAudio.volume = 0.2f;
            }
            // ダイヤルの位置に応じてStereoPanを操作
            AudioSourceStereoPan();
            // ダイヤルを回転させるとともに、angleXに現在のダイヤル回転角度を記録
            rotateAngle += moveRotateSpeed * speedGear * moveInput.x * Time.deltaTime;
            angleX += rotateAngle;
            dial.transform.rotation *= Quaternion.Euler(0, 0, rotateAngle);
            rotateAngle = 0;
        }
        else
        {
            safeRotationAudio.volume = 0;
        }

        // 当たり角度から隣り合わせの番号になるまでの範囲を指定
        float angleRange = 360 / maxKeyNo;
        // 当たり番号の角度範囲に入った際の処理
        if((dialAngle >= (keyAreaAngle - (angleRange / 2)))
            && (dialAngle <= (keyAreaAngle + (angleRange / 2))))
        {
            safeOpen = true;
            // ゲームパッド使用時の処理
            if(gamepad != null)
            {
                // 現在のダイヤル位置がゲームパッドのモーター作動位置に入った場合の処理
                if(Mathf.Abs(dialAngle - keyAreaAngle) <= 1)
                {
                    if(!onMotorMove && motorMoveCount == 0)
                    {
                        motorMoveCount++;
                        onMotorMoveTimeCount = 0;
                        onMotorMove = true;
                    }
                }
                else if(motorMoveCount > 0)
                {
                    motorMoveCount = 0;
                }
            }
        }
        else
        {
            safeOpen = false;
        }

        // 正解のダイヤル番号に近い場合、ゲームパッドのモーターを操作します。
        if(onMotorMove)
        {
            OnGamePadMoter();
        }
    }
    // ゲームパッドのモーター操作
    private void OnGamePadMoter()
    {
        // 経過時間を加算
        onMotorMoveTimeCount += Time.deltaTime;
        // モーターを稼働させる
        if ((onMotorMoveTimeCount < motorMoveTime) && motorMoveCount == 1)
        {
            motorMoveCount++;
            gamepad.SetMotorSpeeds(openDialPadMotorSpeed, openDialPadMotorSpeed);
        }
        else if(onMotorMoveTimeCount > motorMoveTime) // 指定時間外になったらモーターを停止
        {
            gamepad.SetMotorSpeeds(0, 0);
            onMotorMove = false;
        }
    }
    // 金庫を開ける操作をした際の処理
    public void SafeOpenTry()
    {
        if(control && !notOpen)
        {
            // ダイヤルが当たりの位置にある場合
            if (safeOpen)
            {
                stepCount++;
                safeAnimation.NextAnimator(stepCount, openStepCount);

                // 開錠
                if(stepCount == openStepCount)
                {
                    audioSource.PlayOneShot(openOnSound, 0.1f);
                    collider.enabled = false;
                }
                else if(stepCount < openStepCount)　// 次のダイヤル位置へ
                {
                    audioSource.PlayOneShot(unlockOnSound, 1);
                    // 開錠番号を再度ランダムで指定
                    if (randomNo)
                    {
                        openKeyNo = Random.Range(0, maxKeyNo + 1);
                    }
                    // 当たり番号の角度を指定
                    keyAreaAngle = (360 / maxKeyNo) * openKeyNo;
                }
            }
            else
            {
                audioSource.PlayOneShot(notOpenOnSound, notOpenOnSoundVolume);
                notOpen = true;
            }
        }
    }
    // safeRotationAudioのStereoPanを操作
    public void AudioSourceStereoPan()
    {
        if(angleDifference < 360 && angleDifference >= 270)
        {
            safeRotationAudio.panStereo =  -4 + (angleDifference / 90);
        }
        else if(angleDifference > 180 && angleDifference <= 270)
        {
            safeRotationAudio.panStereo = 2 - (angleDifference / 90);
        }
        else if(angleDifference < 180 && angleDifference >= 90)
        {
            safeRotationAudio.panStereo = 2 - (angleDifference / 90);
        }
        else if(angleDifference > 0 && angleDifference <= 90)
        {
            safeRotationAudio.panStereo = angleDifference / 90;
        }
        else
        {
            safeRotationAudio.panStereo = 0;
        }
        rotationPanStereo = safeRotationAudio.panStereo;
    }
}
