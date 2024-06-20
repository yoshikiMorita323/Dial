using System.Collections;
using UnityEngine;

// ステージクリアシーン処理
public class StageClear : MonoBehaviour
{
    Animator animator;

    public bool action = false;
    // フェードアウトするまでの時間を指定
    [SerializeField]
    private float fadeOutTime = 1;

    // このクラスのインスタンスを取得します。
    public static StageClear Instance { get; private set; }
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

        StageScene.Instance.GiveUp();
    }
}
