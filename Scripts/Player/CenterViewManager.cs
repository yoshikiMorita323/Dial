using UnityEngine;

// 中央カーソルからRayを出してオブジェクトを判定する処理
public class CenterViewManager : MonoBehaviour
{
    [SerializeField]
    private CenterText centerText = null;

    //Rayの長さを指定
    [SerializeField]
    private float rayLength = 10;
    [SerializeField]
    private LayerMask layerMask = default;
    // Ray機能を操作できるか否か
    private bool onRay = true;

    public int objectLayer = 0;
    // 飛ばしたRayにあたったオブジェクトのCollider情報を格納
    public RaycastHit hitCrossHairRay;

    // Update is called once per frame
    void Update()
    {
        if(centerText != null && onRay)
        {
            // 画面の中心がオブジェクトに当たった際の処理
            // ViewportPointToRay( 画面のRay発射位置 ,Rayが最初にあたったColliderをhitに取得, Rayの距離）
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hitCrossHairRay, rayLength,layerMask))
            {
                // 最初の更新処理だけ有効にしたいため、
                // hitしたレイヤーとobjectLayerの番号があっていれば処理する
                if (objectLayer != hitCrossHairRay.collider.gameObject.layer)
                {
                    // 特定のオブジェクトにカーソルが当たった時のテキスト処理
                    centerText.CenterTextManager(hitCrossHairRay.collider.gameObject.layer);
                }
                //レイヤーの番号を取得
                objectLayer = hitCrossHairRay.collider.gameObject.layer;
            }
            else
            {
                centerText.Hide();
                objectLayer = 0;
            }
        }
    }
    // rayをとばす
    public void StartUpRay()
    {
        onRay = true;
        if(hitCrossHairRay.collider != null)
        {
            centerText.Show(hitCrossHairRay.collider.gameObject.layer);
        }
    }
    // rayを停止する
    public void StopRay()
    {
        onRay = false;
        centerText.Hide();
    }
}
