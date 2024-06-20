using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �����Ώۂ̈ʒu�ɉ������T�m����
public class SearchManager : MonoBehaviour
{
    // ray���΂��������w��
    [SerializeField]
    private float rayRange = 20;
    // �Ώۂ̃��C���[���w��
    [SerializeField]
    private LayerMask rayLayerMask = default;
    // �T�m�ł���p�x���擾
    [SerializeField]
    private float searchAngle = 90;
    // �����̖ڐ��̈ʒu���w��
    [SerializeField]
    private Vector3 rayPoint = Vector3.zero;
    public Vector3 LostPosition { get => lostPosition; private set => lostPosition = value; }

    // ���������O�̈ʒu���w�� 
    private Vector3 lostPosition = Vector3.zero;
    public bool Search { get => search; private set => search = value; }

    // �T�m�������ۂ��𔻒�
    private bool search = false;


    // �w��͈͂̌����A�y�юw��p�x���ɐN�������ۂ̔�������
    public void SearchAreaManager(Transform searchArea,LayerMask overlapLayerMask,int targetLayerNo)
    {
        // �G���A���ɑΏۂ����邩�������A�G���A�ɓ������ꍇ�A�R���C�_�[���i�[���܂��B
        Collider targetCollider = SearchCollider(searchArea,overlapLayerMask);

        if (targetCollider != null)
        {
            // �Ώۂ̈ʒu�����擾
            Vector3 partnerPosition = targetCollider.gameObject.transform.position;
            // Ray�̔��ˈʒu���w��
            var rayStartPos = transform.position + rayPoint;

            // �x�N�g���Ɗp�x���v�Z
            var direction = VectorManager(rayStartPos, partnerPosition);
            float angleY = AngleManager(direction.x, direction.z);
            // ���g���猩������̊p�x���i�[
            var angleQuartanion = Quaternion.Euler(0, angleY - transform.eulerAngles.y, 0);

            // �T�m�p�x���ɑΏۂ������Ă���ۂ̍X�V����
            if ((angleQuartanion.y >= -searchAngle / 180) && (angleQuartanion.y <= searchAngle / 180))
            {
                // rey�𔭎�
                Ray ray = new Ray(rayStartPos, direction);
                Debug.DrawRay(ray.origin, ray.direction * rayRange, Color.red);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, rayRange,rayLayerMask))
                {
                    // �ΏۂƎ����̊Ԃɉ����Ȃ����true��Ԃ�
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

    // �w�肵�����C���[���͈͓��ɓ������ۂɃR���C�_�[��Ԃ��܂��B
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

    // ����Ƃ̃x�N�g�����v�Z
    public Vector3 VectorManager(Vector3 myPosition,Vector3 partnerPosition)
    {
        Vector3 direction = (partnerPosition - myPosition);
        return direction.normalized;
    }

    // ����Ƃ̊p�x���v�Z
    public float AngleManager(float directionX,float directionY)
    {
        float angle = (Mathf.Atan2(directionX, directionY)) * Mathf.Rad2Deg;
        return angle;
    }

    
}
