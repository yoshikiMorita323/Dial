using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// �Q�[���I�[�o�[�V�[������
public class GameOver : MonoBehaviour
{
    // �Z���N�g����{�^�����w��
    [SerializeField]
    private GameObject[] button;
    private int sum = 0;
    private int buttonNo = 0;
    [SerializeField]
    private float fadeOutTime = 1;
    [SerializeField]
    private bool select = true;

    public bool action = false;
    // onGameOver���w��
    [SerializeField]
    private AudioClip onGameOverSound = null;

    public Vector2 moveInput = Vector2.zero;

    AudioSource audioSource;
    Animator animator;

    // ���̃N���X�̃C���X�^���X���擾���܂��B
    public static GameOver Instance { get; private set; }

    private void Awake()
    {
        // �������g��B��̃C���X�^���X�Ƃ��ēo�^
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnStart());
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        audioSource.PlayOneShot(onGameOverSound, 1);
    }

    // �X�e�[�W�N���A�[���o�����s���܂��B
    IEnumerator OnStart()
    {
        // ����L�[�̓��͂�҂��󂯂�
        while (!action)
        {
            yield return null;
        }

        animator.SetTrigger("FadeOut");
        // �A�j���[�V�������I������܂őҋ@
        yield return new WaitForSeconds(fadeOutTime);

        switch (buttonNo)
        {
            case 0:  // ���g���C
                StageScene.Instance.Retry();
                break;
            case 1: // �^�C�g����
                StageScene.Instance.GiveUp();
                break;
        }
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            action = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �Z���N�g�����ButtonNo�̐؂�ւ�
        SerectManager();
        // BottonNo�ɉ������A�N�e�B�u�{�^���̐؂�ւ�
        ButtonManager();
    }
    // �Z���N�g����ɉ������A�N�e�B�u�{�^���̐؂�ւ�
    private void ButtonManager()
    {
        int i;

        if (!select && sum == 0)
        {
            for (i = 0; i < button.Length; i++)
            {
                if (buttonNo == i)
                {
                    button[i].SetActive(true);
                }
                else
                {
                    button[i].SetActive(false);
                }
            }
            sum++;
        }
        if (select)
        {
            sum = 0;
        }
    }
    // �Z���N�g����
    private void SerectManager()
    {
        if (select)
        {
            // ���݂̃Z���N�g�ʒu��1�ȏ�̎��A����͂ɑ΂��ď������s��
            if (moveInput.y >= 0.2f)
            {
                if ((buttonNo > 0) && select)
                {
                    buttonNo--;
                    select = false;
                }
            }
            else if (moveInput.y <= -0.2f)�@ // ���݂̃Z���N�g�ʒu���ő�{�^���������̎��A�����͂ɑ΂��ď������s��
            {
                if ((buttonNo < button.Length - 1) && select)
                {
                    buttonNo++;
                    select = false;
                }
            }
        }
        if (!select)
        {
            if (Mathf.Abs(moveInput.y) < 0.2f)
            {
                select = true;
            }
        }
    }

}
