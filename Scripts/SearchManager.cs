using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 検索対象の位置に応じた探知処理
public class SearchManager : MonoBehaviour
{
    // rayを飛ばす距離を指定
    [SerializeField]
    private float rayRange = 20;
    // 対象のレイヤーを指定
    [SerializeField]
    private LayerMask rayLayerMask = default;
    // 探知できる角度を取得
    [SerializeField]
    private float searchAngle = 90;
    // 自分の目線の位置を指定
    [SerializeField]
    private Vector3 rayPoint = Vector3.zero;
    public Vector3 LostPosition { get => lostPosition; private set => lostPosition = value; }

    // 見失う直前の位置を指定 
    private Vector3 lostPosition = Vector3.zero;
    public bool Search { get => search; private set => search = value; }

    // 探知したか否かを判定
    private bool search = false;


    // 指定範囲の検索、及び指定角度内に侵入した際の発見処理
    public void SearchAreaManager(Transform searchArea,LayerMask overlapLayerMask,int targetLayerNo)
    {
        // エリア内に対象がいるか検索し、エリアに入った場合、コライダーを格納します。
        Collider targetCollider = SearchCollider(searchArea,overlapLayerMask);

        if (targetCollider != null)
        {
            // 対象の位置情報を取得
            Vector3 partnerPosition = targetCollider.gameObject.transform.position;
            // Rayの発射位置を指定
            var rayStartPos = transform.position + rayPoint;

            // ベクトルと角度を計算
            var direction = VectorManager(rayStartPos, partnerPosition);
            float angleY = AngleManager(direction.x, direction.z);
            // 自身から見た相手の角度を格納
            var angleQuartanion = Quaternion.Euler(0, angleY - transform.eulerAngles.y, 0);

            // 探知角度内に対象が入っている際の更新処理
            if ((angleQuartanion.y >= -searchAngle / 180) && (angleQuartanion.y <= searchAngle / 180))
            {
                // reyを発射
                Ray ray = new Ray(rayStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * rayRange, Color.red);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayRange,rayLayerMask))
                {
                    // 対象と自分の間に何もなければtrueを返す
                    if (hit.collider.gameObject.layer == targetLayerNo)
                    {
                        Search = true;
                    }
                    else
                    {
                        LostPosition = partnerPosition;
                        Search = false;
                    }
                }
            }
        }
        else
        {
            Search = false;
        }
    }

    // 指定したレイヤーが範囲内に入った際にコライダーを返します。
    public Collider SearchCollider(Transform searchArea,LayerMask overlapLayerMask)
    {
        Collider[] collider = Physics.OverlapSphere(
                                searchArea.position,
                                searchArea.transform.localScale.x,
                                overlapLayerMask);
        if(collider.Length > 0)
        {
            return collider[0];
        }
        else
        {
            return null;
        }
    }
    
    public Collider SoundSearchCollider(Transform searchArea,LayerMask overlapLayerMask)
    {
        Collider[] collider = Physics.OverlapSphere(
                                searchArea.position,
                                searchArea.transform.localScale.x,
                                overlapLayerMask);
        
        if(collider.Length > 0)
        {
            return collider[0];
        }
        else
        {
            return null;
        }
    }

    // 相手とのベクトルを計算
    public Vector3 VectorManager(Vector3 myPosition,Vector3 partnerPosition)
    {
        Vector3 direction = (partnerPosition - myPosition);
        return direction.normalized;
    }

    // 相手との角度を計算
    public float AngleManager(float directionX,float directionY)
    {
        float angle = (Mathf.Atan2(directionX, directionY)) * Mathf.Rad2Deg;
        return angle;
    }

    
}
