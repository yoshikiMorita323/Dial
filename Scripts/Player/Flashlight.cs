using UnityEngine;

// �v���C���[���������Ă���X�|�b�g���C�g�̏���
public class Flashlight : MonoBehaviour
{
    [SerializeField]
    private Light spotLight;
    // �t���[�d���̃o�b�e���[�ϋv���Ԃ��w��
    [SerializeField]
    private float capacity = 180;
    // �o�b�e���[��0�̏�Ԃ���t���[�d����܂ł̕b�����w��
    [SerializeField]
    private float chageSpeed = 30;
    // ���C�g�̃o�b�e���[���w��
    private float battery = 100;
    // ���C�g�̍ő�o�b�e���[���w��
    private float maxBattery = 100;
    // batteryImage1������̃o�b�e���[�e�ʁi���j
    private int batteryCellPower = 0;
    // batteryImage���w��
    [SerializeField]
    private GameObject[] batteryImage = null;
    private int batteryCellTmp = 0;
    private int modeAdvantage = 1;
    // ���C�g�̓d����ON/OFF�𔻒�
    public bool onLight = true;
    public bool OnCharge { get => onCharge; set => onCharge = value; }
    // �[�d�����ۂ��𔻒�
    private bool onCharge = true;

    private int count = 0;

    // �o�b�e���[�`���[�W���̉����w��
    [SerializeField]
    private GameObject batteryChageAudio = null;
    // �X�C�b�`���������ۂ̉����w��
    [SerializeField]
    private AudioClip lightSwitchOnSound = null;
    // �X�C�b�`���̉��ʂ��w��
    [SerializeField]
    private float lightSwitchSoundVolume = 0.5f;
    // �o�b�e���[�`���[�W���̉��͈̔͂��w��
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

        // batteryImage1������̃o�b�e���[�e�ʁi���j���v�Z
        batteryCellPower = (int)(maxBattery / batteryImage.Length);
    }

    // Update is called once per frame
    void Update()
    {
        // ���C�g��ON���Ă���ۂ̏���
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
        // �o�b�e���[���`���[�W���܂��B
        BatteryCharging();
        // �o�b�e���[UI����
        BatteryImageManager();
    }
    // ���C�g��ON/OFF�؂�ւ�����
    public void LightSwitch()
    {
        if(spotLight.enabled)   // ���C�gON����OFF��
        {
            onLight = false;
            spotLight.enabled = false;
            audioSource.PlayOneShot(lightSwitchOnSound, lightSwitchSoundVolume);
        }
        else if(!spotLight.enabled && battery > 0 && !OnCharge)   // ���C�gOFF����ON�ց@:�o�b�e���[�c�ʂ��P�ȏォ�A
        {                                                           // �[�d���łȂ���Έڍs�o����.
            modeAdvantage = 1;
            onLight = true;
            spotLight.enabled = true;
            audioSource.PlayOneShot(lightSwitchOnSound, lightSwitchSoundVolume);
        }
    }
    // �o�b�e���[�[�d���̏���
    private void BatteryCharging()
    {
        // �`���[�W��
        if (OnCharge)
        {
            if (count <= 0)
            {
                modeAdvantage = 0;
                batteryChageAudio.SetActive(true);
                batteryChargeSoundArea.SetActive(true);
                count++;
            }
            // �[�d����ۂɃ��C�g�����Ă��������
            if (onLight == true)
            {
                LightSwitch();
            }
            // �o�b�e���[�̏[�d���Ԃɉ����Ẵo�b�e���[��������
            battery += Time.deltaTime * maxBattery / chageSpeed;
            // �o�b�e���[��100���ɂȂ����ۂ̏���
            if (battery >= 100)
            {
                battery = 100;
                OnCharge = false;
            }
        }

        //�`���[�W����
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

    // �o�b�e���[UI����
    private void BatteryImageManager()
    {
        // battery�c�ʂɉ����ĕ\������batteryImage�̐����v�Z
        int batteryCellCount = (int)(battery / batteryCellPower);
        // batteryImage�\������
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
