using UnityEngine;

// プレイヤーが所持しているスポットライトの処理
public class Flashlight : MonoBehaviour
{
    [SerializeField]
    private Light spotLight;
    // フル充電時のバッテリー耐久時間を指定
    [SerializeField]
    private float capacity = 180;
    // バッテリーが0の状態からフル充電するまでの秒数を指定
    [SerializeField]
    private float chageSpeed = 30;
    // ライトのバッテリーを指定
    private float battery = 100;
    // ライトの最大バッテリーを指定
    private float maxBattery = 100;
    // batteryImage1つあたりのバッテリー容量（％）
    private int batteryCellPower = 0;
    // batteryImageを指定
    [SerializeField]
    private GameObject[] batteryImage = null;
    private int batteryCellTmp = 0;
    private int modeAdvantage = 1;
    // ライトの電源のON/OFFを判定
    public bool onLight = true;
    public bool OnCharge { get => onCharge; set => onCharge = value; }
    // 充電中か否かを判定
    private bool onCharge = true;

    private int count = 0;

    // バッテリーチャージ中の音を指定
    [SerializeField]
    private GameObject batteryChageAudio = null;
    // スイッチを押した際の音を指定
    [SerializeField]
    private AudioClip lightSwitchOnSound = null;
    // スイッチ音の音量を指定
    [SerializeField]
    private float lightSwitchSoundVolume = 0.5f;
    // バッテリーチャージ中の音の範囲を指定
    [SerializeField]
    private GameObject batteryChargeSoundArea = null;

    AudioSource audioSource;
    new Light light;


    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
        spotLight.enabled = true;

        // batteryImage1つあたりのバッテリー容量（％）を計算
        batteryCellPower = (int)(maxBattery / batteryImage.Length);
    }

    // Update is called once per frame
    void Update()
    {
        // ライトをONしている際の処理
        if(onLight)
        {
            battery -= Time.deltaTime * maxBattery / capacity;
            if(battery <= 0)
            {
                batteryImage[0].SetActive(false);
                battery = 0;
                LightSwitch();
            }
        }
        // バッテリーをチャージします。
        BatteryCharging();
        // バッテリーUI処理
        BatteryImageManager();
    }
    // ライトのON/OFF切り替え処理
    public void LightSwitch()
    {
        if(spotLight.enabled)   // ライトONからOFFへ
        {
            onLight = false;
            spotLight.enabled = false;
            audioSource.PlayOneShot(lightSwitchOnSound, lightSwitchSoundVolume);
        }
        else if(!spotLight.enabled && battery > 0 && !OnCharge)   // ライトOFFからONへ　:バッテリー残量が１以上かつ、
        {                                                           // 充電中でなければ移行出来る.
            modeAdvantage = 1;
            onLight = true;
            spotLight.enabled = true;
            audioSource.PlayOneShot(lightSwitchOnSound, lightSwitchSoundVolume);
        }
    }
    // バッテリー充電中の処理
    private void BatteryCharging()
    {
        // チャージ中
        if (OnCharge)
        {
            if (count <= 0)
            {
                modeAdvantage = 0;
                batteryChageAudio.SetActive(true);
                batteryChargeSoundArea.SetActive(true);
                count++;
            }
            // 充電する際にライトがついていたら消す
            if (onLight == true)
            {
                LightSwitch();
            }
            // バッテリーの充電時間に応じてのバッテリー増加処理
            battery += Time.deltaTime * maxBattery / chageSpeed;
            // バッテリーが100％になった際の処理
            if (battery >= 100)
            {
                battery = 100;
                OnCharge = false;
            }
        }

        //チャージ解除
        if (!OnCharge) 
        {
            if(count >= 1)
            {
                batteryChageAudio.SetActive(false);
                batteryChargeSoundArea.SetActive(false);
                count = 0;
            }
        }
    }

    // バッテリーUI処理
    private void BatteryImageManager()
    {
        // battery残量に応じて表示するbatteryImageの数を計算
        int batteryCellCount = (int)(battery / batteryCellPower);
        // batteryImage表示処理
        if (batteryCellCount != batteryCellTmp)
        {
            int i;
            for (i = 0; i < batteryImage.Length; i++)
            {
                if (i < batteryCellCount + modeAdvantage)
                {
                    batteryImage[i].SetActive(true);
                }
                else
                {
                    batteryImage[i].SetActive(false);
                }
            }
        }

        batteryCellTmp = batteryCellCount;
    }
}
