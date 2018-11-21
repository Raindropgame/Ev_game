using UnityEngine;
using System.Collections;

public class DeadWood : MonoBehaviour {

    public Element ElementTrigger;

    private bool isBurning = false;
    private bool isBurningOver = false;

	void Update () {
	    if(ElementTrigger.isContainElement(Attribute.fire))
        {
            StartCoroutine(IE_Burning());
        }
	}

    IEnumerator IE_Burning()
    {
        isBurning = true;

        {
            const float first_animation_duration = 2;
            float _time0 = 0;
            while(_time0 < first_animation_duration)
            {
                _time0 += Time.deltaTime;

                yield return null;
            }
        }

        Destroy(this.gameObject);
        isBurning = false;
        isBurningOver = true;

    }
}
