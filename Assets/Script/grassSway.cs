using UnityEngine;
using System.Collections;

public class grassSway : MonoBehaviour {

    public Element elementTrigger;

    private bool isSway = false;
    private Animator animator;
    private bool isBurn = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isBurn)
        {
            if (elementTrigger.isContainElement(Attribute.fire))
            {
                GameFunction.t_Vector3 = transform.position;
                GameFunction.t_Vector3.z = 1;
                Instantiate(ResourcesManagement.getInstance().getResources("Fire"), position:GameFunction.t_Vector3,rotation:Quaternion.Euler(Vector3.zero));
                isBurn = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isSway)
        {
            dir _dir;
            if (collision.transform.position.x > transform.position.x)
                _dir = dir.left;
            else
                _dir = dir.right;

            animator.SetTrigger("sway");
            isSway = true;
            animator.SetInteger("dir", _dir == dir.left ? 1 : -1);
        }
    }



    public void Finish()
    {
        isSway = false;
    }

}
