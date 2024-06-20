using System.Collections;
using UnityEngine;

// �X�e�[�W�N���A�V�[������
public class StageClear : MonoBehaviour
{
    Animator animator;

    public bool action = false;
    // �t�F�[�h�A�E�g����܂ł̎��Ԃ��w��
    [SerializeField]
    private float fadeOutTime = 1;

    // ���̃N���X�̃C���X�^���X���擾���܂��B
    public static StageClear Instance { get; private set; }
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

        StageScene.Instance.GiveUp();
    }
}
