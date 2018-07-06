using UnityEngine;
using System.Collections;

public class platform_jump : MonoBehaviour {

    public ParticleSystem particle;
    public float restTime;
    public Color disableColor;
    public float scale;
    public float speed;

    private bool Rest = false;
    private SpriteRenderer SR;
    private Vector3 originPos;  //原点
    private float randomTime1,randomTime2;  //随机的预先时间

	// Use this for initialization
	void Start () {
        originPos = transform.position;
        SR = GetComponent<SpriteRenderer>();
        randomTime1 = Random.Range(1, 100);
        randomTime2 = Random.Range(1, 100);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(scale * Mathf.Sin(speed * Time.time + randomTime1) + originPos.x, scale * Mathf.Sin(speed * Time.time + randomTime2) + originPos.y, originPos.z);
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !Rest)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CharacterControl.instance._jumpTimes--;
                Rest = true;
                particle.gameObject.SetActive(true);
                Invoke("reset", restTime);
                SR.color = disableColor;
            }
        }
    }

    void reset()
    {
        particle.gameObject.SetActive(false);
        Rest = false;
        SR.color = Color.white;
    }
}
