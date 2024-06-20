using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ポーズシーン処理
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

    // このクラスのインスタンスを取得します。
    public static Pause Instance { get; private set; }

    private void Awake()
    {
        // 自分自身を唯一のインスタンスとして登録
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
        // 決定キーの入力を待ち受ける
        while (!action)
        {
            yield return null;
        }
        switch (ButtonNo)
        {
            case 0:  // 続ける
                StageScene.Instance.Resume();
                break;
            case 1: // リトライ
                StageScene.Instance.Retry();
                break;
            case 2: // タイトル
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
    // セレクト操作に応じたアクティブボタンの切り替え
    private void SelectButtonManager()
    {
        if (select)
        {
            if (moveInput.y >= 0.2f) // 上入力
            {
                if (ButtonNo > 0)
                {
                    beforeButton = ButtonNo;
                    ButtonNo--;
                    select = false;
                }
            }
            else if (moveInput.y <= -0.2f) // 下入力
            {
                if (ButtonNo < button.Length - 1)
                {
                    beforeButton = ButtonNo;
                    ButtonNo++;
                    select = false;
                }
            }
        }

        // ボタン選択されたあとに方向入力が解除された場合、再度選択可能にする
        if (!select && (Mathf.Abs(moveInput.y) < 0.2f))
        {
            select = true;
        }

        // ボタン選択された時のボタンアクティブ処理
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
