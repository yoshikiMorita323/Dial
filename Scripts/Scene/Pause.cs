using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// �|�[�Y�V�[������
public class Pause : MonoBehaviour
{
    [SerializeField]
    private GameObject[] button;
    private int sum = 0;
    public int ButtonNo { get => buttonNo; set => buttonNo = value; }
    private int buttonNo = 0;

    private int beforeButton = 0;

    [SerializeField]
    private bool select = true;
    public bool action = false;

    public Vector2 moveInput = Vector2.zero;
    public int pauseCount = 0;

    // ���̃N���X�̃C���X�^���X���擾���܂��B
    public static Pause Instance { get; private set; }

    private void Awake()
    {
        // �������g��B��̃C���X�^���X�Ƃ��ēo�^
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnStart());
        pauseCount++;
    }

    IEnumerator OnStart()
    {
        // ����L�[�̓��͂�҂��󂯂�
        while (!action)
        {
            yield return null;
        }
        switch (ButtonNo)
        {
            case 0:  // ������
                StageScene.Instance.Resume();
                break;
            case 1: // ���g���C
                StageScene.Instance.Retry();
                break;
            case 2: // �^�C�g��
                StageScene.Instance.GiveUp();
                break;
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        SelectButtonManager();
    }
    // �Z���N�g����ɉ������A�N�e�B�u�{�^���̐؂�ւ�
    private void SelectButtonManager()
    {
        if (select)
        {
            if (moveInput.y >= 0.2f) // �����
            {
                if (ButtonNo > 0)
                {
                    beforeButton = ButtonNo;
                    ButtonNo--;
                    select = false;
                }
            }
            else if (moveInput.y <= -0.2f) // ������
            {
                if (ButtonNo < button.Length - 1)
                {
                    beforeButton = ButtonNo;
                    ButtonNo++;
                    select = false;
                }
            }
        }

        // �{�^���I�����ꂽ���Ƃɕ������͂��������ꂽ�ꍇ�A�ēx�I���\�ɂ���
        if (!select && (Mathf.Abs(moveInput.y) < 0.2f))
        {
            select = true;
        }

        // �{�^���I�����ꂽ���̃{�^���A�N�e�B�u����
        if (!select && (sum == 0))
        {
            button[beforeButton].SetActive(false);
            button[ButtonNo].SetActive(true);
            sum++;
        }

        if (select)
        {
            sum = 0;
        }
    }


}
