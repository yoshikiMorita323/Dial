using UnityEngine;

// 金庫開錠時のアニメーション処理
public class SafeAnimation : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void NextAnimator(int stepCount,int openStepCount)
    {
        if(openStepCount == 3) // WightSafe
        {
            switch (stepCount)
            {
                case 1:
                    animator.SetTrigger("GearFirst");
                    break;
                case 2:
                    animator.SetTrigger("GearSecond");
                    break;
                case 3:
                    animator.SetTrigger("Open");
                    break;
            }
        }

        if (openStepCount == 2) // SilverSafe
        {
            switch (stepCount)
            {
                case 1:
                    animator.SetTrigger("GearFirst");
                    break;
                case 2:
                    animator.SetTrigger("Open");
                    break;
            }
        }

        if (openStepCount == 1) // GreenSafe
        {
            switch (stepCount)
            {
                case 1:
                    animator.SetTrigger("Open");
                    break;
            }
        }
    }
}
