using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// �^�C�g���V�[������
public class TitleScene : MonoBehaviour
{
    // �R���|�[�l���g���Q��
    Animator animator;
    AudioSource audioSource;

    private int sum = 0;
    // �����������w��
    [SerializeField]
    private GameObject[] howToPlay = null;
    // �{�^�����͉����w��
    [SerializeField]
    private AudioClip onActionSound = null;

    // Start is called before the first frame update
    void Start()
    {
        // �R���|�[�l���g���Q��
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // �}�E�X�J�[�\�����\���ɂ���
        Cursor.visible = false;
        // �}�E�X�J�[�\������ʒ����ɌŒ肷��
        Cursor.lockState = CursorLockMode.Locked;
        // �R���[�`�����J�n
        StartCoroutine(OnStart());
    }
    // ����������1�y�[�W�i�ށB�ő�y�[�W���ȏ�ɂȂ����ꍇ�A�V�[�����ڍs����
    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.performed && sum <= howToPlay.Length)
        {
            NextPage();
        }
    }
    // ����������1�y�[�W�i�ށB�ő�y�[�W���ȏ�ɂȂ����ꍇ�A�V�[�����ڍs����
    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.performed && sum <= howToPlay.Length)
        {
            NextPage();
        }
    }
    // ���������̃y�[�W���P�i�߂�
    private void NextPage()
    {
        sum++;
        audioSource.PlayOneShot(onActionSound, 1);
        if (sum <= howToPlay.Length)
        {
            howToPlay[sum - 1].SetActive(true);
            if (sum > 1)
            {
                howToPlay[sum - 2].SetActive(false);
            }
        }
    }
    // ����������2�y�[�W�ȏ��\�����Ă���ꍇ�A1�y�[�W�߂�
    public void OnBack(InputAction.CallbackContext context)
    {
        if (context.performed && sum > 1 && sum <= howToPlay.Length)
        {
            sum--;
            audioSource.PlayOneShot(onActionSound, 1);
            howToPlay[sum - 1].SetActive(true);
            howToPlay[sum].SetActive(false);
        }
    }
    IEnumerator OnStart()
    {
        // �Q�b�Ԃ������͂��󂯕t���Ȃ�
        yield return new WaitForSeconds(2);
        // �����������sum�̒l���傫���Ȃ����ꍇ�A�V�[�����ڍs����
        while (true)
        {
            if (sum > howToPlay.Length)
            {
                break;
            }
            yield return null;
        }
        // FadeOut�A�j���[�V�����֑J��
        animator.SetTrigger("FadeOut");
        // 1�b�ԑҋ@
        yield return new WaitForSeconds(1.5f);
        // ���̃V�[����ǂݍ���
        SceneManager.LoadScene("Stage");
    }

}
