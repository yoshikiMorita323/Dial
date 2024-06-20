using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// ステージシーンのシーン移行処理
public class StageScene : MonoBehaviour
{
    // UI表示用のCanvasを指定します。
    [SerializeField]
    private Transform uiRoot = null;
    // ゲームオーバーUIのプレハブを指定します。
    [SerializeField]
    private GameObject gameOverPrefab = null;
    // ステージクリアーUIのプレハブを指定します。
    [SerializeField]
    private GameObject stageClearPrefab = null;
    // ポーズUIのプレハブを指定します。
    [SerializeField]
    private GameObject pausePrefab = null;
    // Enemyを指定
    [SerializeField]
    private GameObject[] enemy = null;

    private bool pause = false;

    AudioSource audioSource;

    // ポーズ状態の場合はtrue、プレイ状態の場合はfalse
    bool isPaused = false;

    // ステージ画面内の進行状態を表します。
    enum SceneState
    {
        // ステージ開始演出中
        Start,
        // ステージプレイ中
        Play,
        // ゲームオーバーが確定していて演出中
        GameOver,
        // ステージクリアーが確定していて演出中
        StageClear,
    }
    SceneState sceneState = SceneState.Start;

    // このクラスのインスタンスを取得します。
    public static StageScene Instance { get; private set; }

    private void Awake()
    {
        // 自分自身を唯一のインスタンスとして登録
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
                    // ポーズ
                    if (pause)
                    {
                        Pause();
                    }
                }
                break;
            case SceneState.Play:
                if (!isPaused)
                {
                    // ポーズ
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
            // StageClearプレハブをCanvasにインスタンス生成
            Instantiate(stageClearPrefab, uiRoot);
        }
    }
    // 次のステージを読み込みます。
    public void LoadNextScene()
    {
        StartCoroutine(OnLoadNextScene());
    }

    IEnumerator OnLoadNextScene()
    {
        // アニメーションが終了するまで待機
        yield return null;

        // "GameClear"シーンをロードする
        SceneManager.LoadScene("GameClear");
    }

    #region ゲームオーバー
    // このステージをゲームオーバーとします。
    public void GameOver()
    {
        audioSource.Stop();
        EnemyActiveFalse();

        // ステージプレイ中のみ
        if (sceneState == SceneState.Play || sceneState == SceneState.Start)
        {
            sceneState = SceneState.GameOver;
            // GameOverプレハブをCanvasにインスタンス生成
            Instantiate(gameOverPrefab, uiRoot);
        }
    }

    // このステージをリトライします。
    public void Retry()
    {
        // ポーズ中の場合は先に解除する
        Resume();
        StartCoroutine(OnRetry());
    }

    IEnumerator OnRetry()
    {
        // アニメーションが終了するまで待機
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Stage");
    }

    // このステージをギブアップしてタイトル画面に戻ります。
    public void GiveUp()
    {
        // ポーズ中の場合は先に解除する
        Resume();
        StartCoroutine(OnGiveUp());
    }

    IEnumerator OnGiveUp()
    {
        // アニメーションが終了するまで待機
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Title");
    }
    #endregion

    // このステージをポーズします。
    public void Pause()
    {
        if (!isPaused)
        {
            // カーソルを表示する
            Cursor.visible = true;
            // カーソルを自由に動かせるように処理
            Cursor.lockState = CursorLockMode.None;
            isPaused = true;
            Time.timeScale = 0;
            Instantiate(pausePrefab, uiRoot);
        }
    }

    // ポーズ状態を解除します。
    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;
            pause = false;
            Time.timeScale = 1;
            // カーソルを非表示
            Cursor.visible = false;
            // カーソルを画面中央に固定
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
