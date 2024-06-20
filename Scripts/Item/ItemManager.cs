using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �p���A�C�e���̐e�I�u�W�F�N�g
public class ItemManager : MonoBehaviour
{
    // ���g��3D���f�����w��
    [SerializeField]
    protected GameObject model = null;

    [SerializeField]
    private AudioClip pickUpOnSound = null;
    [SerializeField]
    private float pickUpSoundVolume = 1;

    protected AudioSource audioSource;
    protected new Collider collider;
    protected Animator animator;

    // ItemAnimator�̃p�����[�^�[ID
    static readonly int idleId = Animator.StringToHash("Idle");
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    // �v���C���[�̃A�C�e���擾����
    public void PickUp()
    {
        // �擾���ɉ���炷
        audioSource.PlayOneShot(pickUpOnSound, pickUpSoundVolume);
        // Model���\���ɂ���
        model.SetActive(false);
        // �R���C�_�[���\���ɂ���
        collider.enabled = false;
        // �A�j���[�^�[��J��
        animator.SetTrigger(idleId);
        // �A�C�e���ǉ�����
        AddItemStock();
    }
    // �A�C�e���ǉ�����
    protected virtual void AddItemStock()
    {

    }
}
