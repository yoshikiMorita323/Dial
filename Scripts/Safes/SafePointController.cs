using UnityEngine;

// 金庫の位置をランダムに配置する処理
public class SafePointController : MonoBehaviour
{
    // safeを指定
    [SerializeField]
    private Transform[] safe = null;
    // safePointを指定
    [SerializeField]
    private Transform[] safePoint = null;

    void Start()
    {
        int i,t;
        int[] randomNomber = new int[safe.Length];
        randomNomber[0] = Random.Range(0, safePoint.Length);

        // safePointの位置と角度をsafeに同期します。
        safe[0].transform.position = safePoint[randomNomber[0]].transform.position;
        safe[0].transform.rotation = safePoint[randomNomber[0]].transform.rotation;

        // safeの位置と角度をランダムでsafePointと同期します。
        for (i = 1;i < safe.Length;i++)
        {
            int nowNomber = i;
            randomNomber[i] = Random.Range(0, safePoint.Length);
            //　randomNomber[i]に格納した数字とrandomNomber内の数字に被りがないか判定
            for (t = 0;t < i;t++)
            {
                // 番号が被った場合、やり直す。
                if(randomNomber[t] == randomNomber[i])
                {
                    i--;
                    break;
                }
            }
            if(i == nowNomber)
            {
                // safePointの位置と角度をsafeに同期します。
                safe[i].transform.position = safePoint[randomNomber[i]].transform.position;
                safe[i].transform.rotation = safePoint[randomNomber[i]].transform.rotation;
            }
        }
    }

}
