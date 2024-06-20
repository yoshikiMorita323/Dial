using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// ゲームオーバーシーン処理
public class GameOver : MonoBehaviour
{
    // セレクトするボタンを指定
    [SerializeField]
    private GameObject[] button;
    private int sum = 0;
    private int buttonNo = 0;
    [SerializeField]
    private float fadeOutTime = 1;
    [SerializeField]
    private bool select = true;

    public bool action = false;
    // onGameOverを指定
    [SerializeField]
    private AudioClip onGameOverSound = null;

    public Vector2 moveInput = Vector2.zero;

    AudioSource audioSource;
    Animator animator;

    // このクラスのインスタンスを取得します。
    public static GameOver Instance { get; private set; }

    private void Awake()
    {
        // 自分自身を唯一のインスタンスとして登録
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

    // ステージクリアー演出を実行します。
    IEnumerator OnStart()
    {
        // 決定キーの入力を待ち受ける
        while (!action)
        {
            yield return null;
        }

        animator.SetTrigger("FadeOut");
        // アニメーションが終了するまで待機
        yield return new WaitForSeconds(fadeOutTime);

        switch (buttonNo)
        {
            case 0:  // リトライ
                StageScene.Instance.Retry();
                break;
            case 1: // タイトルへ
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
        // セレクト操作とButtonNoの切り替え
        SerectManager();
        // BottonNoに応じたアクティブボタンの切り替え
        ButtonManager();
    }
    // セレクト操作に応じたアクティブボタンの切り替え
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
    // セレクト操作
    private void SerectManager()
    {
        if (select)
        {
            // 現在のセレクト位置が1以上の時、上入力に対して処理を行う
            if (moveInput.y >= 0.2f)
            {
                if ((buttonNo > 0) && select)
                {
                    buttonNo--;
                    select = false;
                }
            }
            else if (moveInput.y <= -0.2f)　 // 現在のセレクト位置が最大ボタン数未満の時、下入力に対して処理を行う
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
