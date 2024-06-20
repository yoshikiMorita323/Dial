using UnityEngine;
using UnityEngine.UI;
using System;

// Playerの中央カーソルが指しているオブジェクトに応じたテキスト表示処理
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

    // オブジェクトのレイヤーに応じてテキストを表示
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

    // テキストを読み込む
    public void Show(int layerNo)
    {
        CenterTextManager(layerNo);
    }

    // テキスト機能を停止する
    public void Hide()
    {
        centerText.text = ("");
    }
}
