using UnityEngine;
using UnityEngine.AI;

// NaviMeshを使用したルート移動処理
public class NaviMesh : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    // 自分の位置を指定
    private Vector3 myPositon = Vector3.zero;
    // ルート本数を指定
    [SerializeField]
    private PatrolRoot[] root; 
    // 巡回地点を指定
    [System.Serializable]
    public class PatrolRoot
    {
        public Transform[] targetPosition;
    }
    // ルートの番号を指定
    private int rootNumber = 0;
    // 巡回地点の番号を指定
    private int targetNumber = 0;
    // 向かっている巡回地点を格納
    private Vector3 targetPositionTmp = Vector3.zero;
    // 巡回中か否かを判定
    [SerializeField]
    private bool randomRoot = true;
    // 目的地と敵の位置がどのくらいの距離まで近づけば次の目的地に行く処理をするか指定
    private float minDistance = 1;
    // Start is called before the first frame update
    void Start()
    {
        // コンポーネントを取得
        navMeshAgent = GetComponent<NavMeshAgent>();
        // 最初の巡回ルートをランダムで指定
        rootNumber = Random.Range(0, root.Length);
        navMeshAgent.SetDestination(root[rootNumber].targetPosition[targetNumber].transform.position);
        targetPositionTmp = root[rootNumber].targetPosition[targetNumber].transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        myPositon = transform.position;
        if (randomRoot)
        {
            MovePatrol();
        }
    }
    
    // 巡回中の目的地を更新します。
    private void MovePatrol()
    {
        // ポイントに着いたか判定
        if (Mathf.Abs(myPositon.x - targetPositionTmp.x) 
            + Mathf.Abs(myPositon.z - targetPositionTmp.z) <= minDistance)
        {
            targetNumber++;
            // 現在のルートの最終地点に着いた場合、次のルートをランダムで決める
            if(targetNumber >= root[rootNumber].targetPosition.Length)
            {
                targetNumber = 0;
                rootNumber = Random.Range(0, root.Length);
            }
            // 巡回状況にあわせて次のポイントを指定
            navMeshAgent.SetDestination(root[rootNumber].targetPosition[targetNumber].transform.position);
            targetPositionTmp = root[rootNumber].targetPosition[targetNumber].transform.position;
        }
    }
    
    // 目的地を指定
    public void SetTargetPosition(Vector3 targetPosition)
    {
        navMeshAgent.SetDestination(targetPosition);
        randomRoot = false;
    }

    // 巡回ルートに戻る
    public void RootSetDestination()
    {
        randomRoot = true;
        // 前回ランダムで指定したポイントに移動
        navMeshAgent.SetDestination(root[rootNumber].targetPosition[targetNumber].transform.position);
        targetPositionTmp = root[rootNumber].targetPosition[targetNumber].transform.position;
    }
    
    // 目的地に到着した際に止まるか否かを変更  true = 止まらない
    public void AutoBraking(bool move)
    {
        navMeshAgent.autoBraking = move;
    }

    // スピードを変更
    public void ChengeSpeed(float moveSpeed)
    {
        navMeshAgent.speed = moveSpeed;
    }
    // NaviMeshを解除
    public void NaviMeshLift()
    {
        navMeshAgent.enabled = false;
    }
}
