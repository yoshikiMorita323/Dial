using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵のアニメーション処理
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

    // EnemyAnimatorのパラメータID
    static readonly int walkId = Animator.StringToHash("Walk");
    static readonly int attackId = Animator.StringToHash("Attack");
    static readonly int killId = Animator.StringToHash("Kill");
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // プレイヤーを発見
    public void SetModelDiscover()
    {
        animator.SetTrigger(attackId);
        defultSound.Stop();
        attackSound.Play();
    }
    // プレイヤーを見失う
    public void SetModelLoseSight()
    {
        animator.SetTrigger(walkId);
        defultSound.Play();
        attackSound.Stop();
    }
    // 警戒
    public void SetModelWarning()
    {
        audioSource.PlayOneShot(soundDetectedOnSound, AudioListener.volume);
    }
    // 警戒解除
    public void SetModelReleaseWarning()
    {
        audioSource.PlayOneShot(releaseWarningOnSound, AudioListener.volume);
    }
    // プレイヤーを捕まえる
    public void SetModelCatch()
    {
        animator.SetTrigger(killId);
        attackSound.Stop();
    }
}
