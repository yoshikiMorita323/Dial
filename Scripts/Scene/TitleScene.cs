using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// タイトルシーン処理
public class TitleScene : MonoBehaviour
{
    // コンポーネントを参照
    Animator animator;
    AudioSource audioSource;

    private int sum = 0;
    // 説明資料を指定
    [SerializeField]
    private GameObject[] howToPlay = null;
    // ボタン入力音を指定
    [SerializeField]
    private AudioClip onActionSound = null;

    // Start is called before the first frame update
    void Start()
    {
        // コンポーネントを参照
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // マウスカーソルを非表示にする
        Cursor.visible = false;
        // マウスカーソルを画面中央に固定する
        Cursor.lockState = CursorLockMode.Locked;
        // コルーチンを開始
        StartCoroutine(OnStart());
    }
    // 説明資料を1ページ進む。最大ページ数以上になった場合、シーンを移行する
    public void OnAction(InputAction.CallbackContext context)
    {
        if (context.performed && sum <= howToPlay.Length)
        {
            NextPage();
        }
    }
    // 説明資料を1ページ進む。最大ページ数以上になった場合、シーンを移行する
    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.performed && sum <= howToPlay.Length)
        {
            NextPage();
        }
    }
    // 説明資料のページを１つ進める
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
    // 説明資料の2ページ以上を表示している場合、1ページ戻る
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
        // ２秒間だけ入力を受け付けない
        yield return new WaitForSeconds(2);
        // 説明資料よりsumの値が大きくなった場合、シーンを移行する
        while (true)
        {
            if (sum > howToPlay.Length)
            {
                break;
            }
            yield return null;
        }
        // FadeOutアニメーションへ遷移
        animator.SetTrigger("FadeOut");
        // 1秒間待機
        yield return new WaitForSeconds(1.5f);
        // 次のシーンを読み込む
        SceneManager.LoadScene("Stage");
    }

}
