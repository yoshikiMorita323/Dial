using UnityEngine;
using UnityEngine.UI;
using System;

// Player�̒����J�[�\�����w���Ă���I�u�W�F�N�g�ɉ������e�L�X�g�\������
public class CenterText : MonoBehaviour
{
    [SerializeField]
    private Text centerText = null;

    [Serializable]
    class CenterTextClass
    {
        public TextAsset name;

        public int layerNo;
    }

    [SerializeField]
    private CenterTextClass[] centerTextClass;

    // �I�u�W�F�N�g�̃��C���[�ɉ����ăe�L�X�g��\��
    public void CenterTextManager(int layerNomber)
    {
       for(int i = 0;i < centerTextClass.Length;i++)
       {
            if (centerTextClass[i].layerNo == layerNomber)
            {
                centerText.text = centerTextClass[i].name.text;
                return;
            }
       }
       centerText.text = centerTextClass[0].name.text;
     
    }

    // �e�L�X�g��ǂݍ���
    public void Show(int layerNo)
    {
        CenterTextManager(layerNo);
    }

    // �e�L�X�g�@�\���~����
    public void Hide()
    {
        centerText.text = ("");
    }
}
