using UnityEngine;

// ���ɂ̈ʒu�������_���ɔz�u���鏈��
public class SafePointController : MonoBehaviour
{
    // safe���w��
    [SerializeField]
    private Transform[] safe = null;
    // safePoint���w��
    [SerializeField]
    private Transform[] safePoint = null;

    void Start()
    {
        int i,t;
        int[] randomNomber = new int[safe.Length];
        randomNomber[0] = Random.Range(0, safePoint.Length);

        // safePoint�̈ʒu�Ɗp�x��safe�ɓ������܂��B
        safe[0].transform.position = safePoint[randomNomber[0]].transform.position;
        safe[0].transform.rotation = safePoint[randomNomber[0]].transform.rotation;

        // safe�̈ʒu�Ɗp�x�������_����safePoint�Ɠ������܂��B
        for (i = 1;i < safe.Length;i++)
        {
            int nowNomber = i;
            randomNomber[i] = Random.Range(0, safePoint.Length);
            //�@randomNomber[i]�Ɋi�[����������randomNomber���̐����ɔ�肪�Ȃ�������
            for (t = 0;t < i;t++)
            {
                // �ԍ���������ꍇ�A��蒼���B
                if(randomNomber[t] == randomNomber[i])
                {
                    i--;
                    break;
                }
            }
            if(i == nowNomber)
            {
                // safePoint�̈ʒu�Ɗp�x��safe�ɓ������܂��B
                safe[i].transform.position = safePoint[randomNomber[i]].transform.position;
                safe[i].transform.rotation = safePoint[randomNomber[i]].transform.rotation;
            }
        }
    }

}
