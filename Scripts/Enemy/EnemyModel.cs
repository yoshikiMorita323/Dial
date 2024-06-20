using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �G�̃A�j���[�V��������
public class EnemyModel : MonoBehaviour
{
    Animator animator;
    AudioSource audioSource;

    [SerializeField]
    private AudioClip soundDetectedOnSound = null;
    [SerializeField]
    private AudioClip releaseWarningOnSound = null;
    [SerializeField]
    private AudioSource defultSound = null;
    [SerializeField]
    private AudioSource attackSound = null;

    // EnemyAnimator�̃p�����[�^ID
    static readonly int walkId = Animator.StringToHash("Walk");
    static readonly int attackId = Animator.StringToHash("Attack");
    static readonly int killId = Animator.StringToHash("Kill");
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // �v���C���[�𔭌�
    public void SetModelDiscover()
    {
        animator.SetTrigger(attackId);
        defultSound.Stop();
        attackSound.Play();
    }
    // �v���C���[��������
    public void SetModelLoseSight()
    {
        animator.SetTrigger(walkId);
        defultSound.Play();
        attackSound.Stop();
    }
    // �x��
    public void SetModelWarning()
    {
        audioSource.PlayOneShot(soundDetectedOnSound, AudioListener.volume);
    }
    // �x������
    public void SetModelReleaseWarning()
    {
        audioSource.PlayOneShot(releaseWarningOnSound, AudioListener.volume);
    }
    // �v���C���[��߂܂���
    public void SetModelCatch()
    {
        animator.SetTrigger(killId);
        attackSound.Stop();
    }
}
