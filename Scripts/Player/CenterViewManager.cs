using UnityEngine;

// �����J�[�\������Ray���o���ăI�u�W�F�N�g�𔻒肷�鏈��
public class CenterViewManager : MonoBehaviour
{
    [SerializeField]
    private CenterText centerText = null;

    //Ray�̒������w��
    [SerializeField]
    private float rayLength = 10;
    [SerializeField]
    private LayerMask layerMask = default;
    // Ray�@�\�𑀍�ł��邩�ۂ�
    private bool onRay = true;

    public int objectLayer = 0;
    // ��΂���Ray�ɂ��������I�u�W�F�N�g��Collider�����i�[
    public RaycastHit hitCrossHairRay;

    // Update is called once per frame
    void Update()
    {
        if(centerText != null && onRay)
        {
            // ��ʂ̒��S���I�u�W�F�N�g�ɓ��������ۂ̏���
            // ViewportPointToRay( ��ʂ�Ray���ˈʒu ,Ray���ŏ��ɂ�������Collider��hit�Ɏ擾, Ray�̋����j
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hitCrossHairRay, rayLength,layerMask))
            {
                // �ŏ��̍X�V���������L���ɂ��������߁A
                // hit�������C���[��objectLayer�̔ԍ��������Ă���Ώ�������
                if (objectLayer != hitCrossHairRay.collider.gameObject.layer)
                {
                    // ����̃I�u�W�F�N�g�ɃJ�[�\���������������̃e�L�X�g����
                    centerText.CenterTextManager(hitCrossHairRay.collider.gameObject.layer);
                }
                //���C���[�̔ԍ����擾
                objectLayer = hitCrossHairRay.collider.gameObject.layer;
            }
            else
            {
                centerText.Hide();
                objectLayer = 0;
            }
        }
    }
    // ray���Ƃ΂�
    public void StartUpRay()
    {
        onRay = true;
        if(hitCrossHairRay.collider != null)
        {
            centerText.Show(hitCrossHairRay.collider.gameObject.layer);
        }
    }
    // ray���~����
    public void StopRay()
    {
        onRay = false;
        centerText.Hide();
    }
}
