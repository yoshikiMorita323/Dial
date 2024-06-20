using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 継承アイテムの親オブジェクト
public class ItemManager : MonoBehaviour
{
    // 自身の3Dモデルを指定
    [SerializeField]
    protected GameObject model = null;

    [SerializeField]
    private AudioClip pickUpOnSound = null;
    [SerializeField]
    private float pickUpSoundVolume = 1;

    protected AudioSource audioSource;
    protected new Collider collider;
    protected Animator animator;

    // ItemAnimatorのパラメーターID
    static readonly int idleId = Animator.StringToHash("Idle");
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    // プレイヤーのアイテム取得処理
    public void PickUp()
    {
        // 取得時に音を鳴らす
        audioSource.PlayOneShot(pickUpOnSound, pickUpSoundVolume);
        // Modelを非表示にする
        model.SetActive(false);
        // コライダーを非表示にする
        collider.enabled = false;
        // アニメーターを遷移
        animator.SetTrigger(idleId);
        // アイテム追加処理
        AddItemStock();
    }
    // アイテム追加処理
    protected virtual void AddItemStock()
    {

    }
}
