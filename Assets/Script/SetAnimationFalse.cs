using UnityEngine;
using System.Collections;

public class SetAnimationFalse : MonoBehaviour {

    private Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9)
        {
            this.gameObject.SetActive(false);
        }
    }
}
