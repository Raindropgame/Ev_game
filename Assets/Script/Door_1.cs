using UnityEngine;
using System.Collections;

public class Door_1 : MonoBehaviour {

    public Transform Stone_trans;
    public SpriteRenderer SR_Stone_crack;
    [HideInInspector]
    public bool isLock = true;

    private bool isStone = true;
    private Vector3 originPos_Stone;
    private float times = 0;

	void Start () {
        originPos_Stone = Stone_trans.position;
	}
	
	void Update () {
        if (isLock)
        {
            if (isStone)
            {
                times += Time.deltaTime;
                Stone_trans.position = originPos_Stone + Mathf.Sin(times) * Vector3.up * 0.4f;
            }
            SR_Stone_crack.color = Color.Lerp(Color.black, Color.white, 0.5f * Mathf.Sin(Time.time * 1.4f) + 0.5f);
        }
	}

    public void Unlock()
    {
        isLock = false;
        StartCoroutine(IE_unlock());
    }

    IEnumerator IE_unlock()
    {
        Transform trans_shadow = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "Shadow").transform;
        Vector3 originScale_shadow = trans_shadow.localScale;

        const float duration = 1.8f;
        float timer = 0;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            trans_shadow.localScale = Vector3.Lerp(originScale_shadow, GameFunction.getVector3(1,0,1), t);
            SR_Stone_crack.color = Color.Lerp(SR_Stone_crack.color, Color.black, t);

            yield return null;
        }

        Stone_trans.GetComponent<Rigidbody2D>().isKinematic = false;
        this.enabled = false;
        this.GetComponent<BoxCollider2D>().enabled = false;

        //Destroy(this.gameObject);
    }
}
