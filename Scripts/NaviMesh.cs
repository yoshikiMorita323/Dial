using UnityEngine;
using UnityEngine.AI;

// NaviMesh���g�p�������[�g�ړ�����
public class NaviMesh : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    // �����̈ʒu���w��
    private Vector3 myPositon = Vector3.zero;
    // ���[�g�{�����w��
    [SerializeField]
    private PatrolRoot[] root; 
    // ����n�_���w��
    [System.Serializable]
    public class PatrolRoot
    {
        public Transform[] targetPosition;
    }
    // ���[�g�̔ԍ����w��
    private int rootNumber = 0;
    // ����n�_�̔ԍ����w��
    private int targetNumber = 0;
    // �������Ă��鏄��n�_���i�[
    private Vector3 targetPositionTmp = Vector3.zero;
    // ���񒆂��ۂ��𔻒�
    [SerializeField]
    private bool randomRoot = true;
    // �ړI�n�ƓG�̈ʒu���ǂ̂��炢�̋����܂ŋ߂Â��Ύ��̖ړI�n�ɍs�����������邩�w��
    private float minDistance = 1;
    // Start is called before the first frame update
    void Start()
    {
        // �R���|�[�l���g���擾
        navMeshAgent = GetComponent<NavMeshAgent>();
        // �ŏ��̏��񃋁[�g�������_���Ŏw��
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
    
    // ���񒆂̖ړI�n���X�V���܂��B
    private void MovePatrol()
    {
        // �|�C���g�ɒ�����������
        if (Mathf.Abs(myPositon.x - targetPositionTmp.x) 
            + Mathf.Abs(myPositon.z - targetPositionTmp.z) <= minDistance)
        {
            targetNumber++;
            // ���݂̃��[�g�̍ŏI�n�_�ɒ������ꍇ�A���̃��[�g�������_���Ō��߂�
            if(targetNumber >= root[rootNumber].targetPosition.Length)
            {
                targetNumber = 0;
                rootNumber = Random.Range(0, root.Length);
            }
            // ����󋵂ɂ��킹�Ď��̃|�C���g���w��
            navMeshAgent.SetDestination(root[rootNumber].targetPosition[targetNumber].transform.position);
            targetPositionTmp = root[rootNumber].targetPosition[targetNumber].transform.position;
        }
    }
    
    // �ړI�n���w��
    public void SetTargetPosition(Vector3 targetPosition)
    {
        navMeshAgent.SetDestination(targetPosition);
        randomRoot = false;
    }

    // ���񃋁[�g�ɖ߂�
    public void RootSetDestination()
    {
        randomRoot = true;
        // �O�񃉃��_���Ŏw�肵���|�C���g�Ɉړ�
        navMeshAgent.SetDestination(root[rootNumber].targetPosition[targetNumber].transform.position);
        targetPositionTmp = root[rootNumber].targetPosition[targetNumber].transform.position;
    }
    
    // �ړI�n�ɓ��������ۂɎ~�܂邩�ۂ���ύX  true = �~�܂�Ȃ�
    public void AutoBraking(bool move)
    {
        navMeshAgent.autoBraking = move;
    }

    // �X�s�[�h��ύX
    public void ChengeSpeed(float moveSpeed)
    {
        navMeshAgent.speed = moveSpeed;
    }
    // NaviMesh������
    public void NaviMeshLift()
    {
        navMeshAgent.enabled = false;
    }
}
