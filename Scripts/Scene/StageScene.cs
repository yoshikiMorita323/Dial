using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// �X�e�[�W�V�[���̃V�[���ڍs����
public class StageScene : MonoBehaviour
{
    // UI�\���p��Canvas���w�肵�܂��B
    [SerializeField]
    private Transform uiRoot = null;
    // �Q�[���I�[�o�[UI�̃v���n�u���w�肵�܂��B
    [SerializeField]
    private GameObject gameOverPrefab = null;
    // �X�e�[�W�N���A�[UI�̃v���n�u���w�肵�܂��B
    [SerializeField]
    private GameObject stageClearPrefab = null;
    // �|�[�YUI�̃v���n�u���w�肵�܂��B
    [SerializeField]
    private GameObject pausePrefab = null;
    // Enemy���w��
    [SerializeField]
    private GameObject[] enemy = null;

    private bool pause = false;

    AudioSource audioSource;

    // �|�[�Y��Ԃ̏ꍇ��true�A�v���C��Ԃ̏ꍇ��false
    bool isPaused = false;

    // �X�e�[�W��ʓ��̐i�s��Ԃ�\���܂��B
    enum SceneState
    {
        // �X�e�[�W�J�n���o��
        Start,
        // �X�e�[�W�v���C��
        Play,
        // �Q�[���I�[�o�[���m�肵�Ă��ĉ��o��
        GameOver,
        // �X�e�[�W�N���A�[���m�肵�Ă��ĉ��o��
        StageClear,
    }
    SceneState sceneState = SceneState.Start;

    // ���̃N���X�̃C���X�^���X���擾���܂��B
    public static StageScene Instance { get; private set; }

    private void Awake()
    {
        // �������g��B��̃C���X�^���X�Ƃ��ēo�^
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        switch (sceneState)
        {
            case SceneState.Start:
                if (!isPaused)
                {
                    // �|�[�Y
                    if (pause)
                    {
                        Pause();
                    }
                }
                break;
            case SceneState.Play:
                if (!isPaused)
                {
                    // �|�[�Y
                    if (pause)
                    {
                        Pause();
                    }
                }
                break;

            case SceneState.GameOver:
            case SceneState.StageClear:
            default:
                break;
        }
    }

    public void StageClear()
    {
        if (sceneState == SceneState.Play || sceneState == SceneState.Start)
        {
            audioSource.Stop();
            EnemyActiveFalse();
            // StageClear�v���n�u��Canvas�ɃC���X�^���X����
            Instantiate(stageClearPrefab, uiRoot);
        }
    }
    // ���̃X�e�[�W��ǂݍ��݂܂��B
    public void LoadNextScene()
    {
        StartCoroutine(OnLoadNextScene());
    }

    IEnumerator OnLoadNextScene()
    {
        // �A�j���[�V�������I������܂őҋ@
        yield return null;

        // "GameClear"�V�[�������[�h����
        SceneManager.LoadScene("GameClear");
    }

    #region �Q�[���I�[�o�[
    // ���̃X�e�[�W���Q�[���I�[�o�[�Ƃ��܂��B
    public void GameOver()
    {
        audioSource.Stop();
        EnemyActiveFalse();

        // �X�e�[�W�v���C���̂�
        if (sceneState == SceneState.Play || sceneState == SceneState.Start)
        {
            sceneState = SceneState.GameOver;
            // GameOver�v���n�u��Canvas�ɃC���X�^���X����
            Instantiate(gameOverPrefab, uiRoot);
        }
    }

    // ���̃X�e�[�W�����g���C���܂��B
    public void Retry()
    {
        // �|�[�Y���̏ꍇ�͐�ɉ�������
        Resume();
        StartCoroutine(OnRetry());
    }

    IEnumerator OnRetry()
    {
        // �A�j���[�V�������I������܂őҋ@
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Stage");
    }

    // ���̃X�e�[�W���M�u�A�b�v���ă^�C�g����ʂɖ߂�܂��B
    public void GiveUp()
    {
        // �|�[�Y���̏ꍇ�͐�ɉ�������
        Resume();
        StartCoroutine(OnGiveUp());
    }

    IEnumerator OnGiveUp()
    {
        // �A�j���[�V�������I������܂őҋ@
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Title");
    }
    #endregion

    // ���̃X�e�[�W���|�[�Y���܂��B
    public void Pause()
    {
        if (!isPaused)
        {
            // �J�[�\����\������
            Cursor.visible = true;
            // �J�[�\�������R�ɓ�������悤�ɏ���
            Cursor.lockState = CursorLockMode.None;
            isPaused = true;
            Time.timeScale = 0;
            Instantiate(pausePrefab, uiRoot);
        }
    }

    // �|�[�Y��Ԃ��������܂��B
    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;
            pause = false;
            Time.timeScale = 1;
            // �J�[�\�����\��
            Cursor.visible = false;
            // �J�[�\������ʒ����ɌŒ�
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void EnemyActiveFalse()
    {
        for(int i = 0;i < enemy.Length;i++)
        {
            enemy[i].SetActive(false);
        }
    }
}
