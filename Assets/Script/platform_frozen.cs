using UnityEngine;
using System.Collections;

public class platform_frozen : MonoBehaviour {

    public PlatformMove script;

    private GameObject particleEffect;
    private Transform[] stone = new Transform[6];
    private int times = 3;

    private void Start()
    {
        stone = GetComponentsInChildren<Transform>();
        particleEffect = Resources.Load<GameObject>("stoneParticleEffect");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.transform.tag);
        if(collision.transform.tag.Substring(0,4) == "arms")
        {
            Instantiate(particleEffect, position: collision.contacts[0].point, rotation: Quaternion.Euler(0, 0, 0));
            stone[times * 2 - 1].gameObject.SetActive(false);
            stone[times * 2 - 2].gameObject.SetActive(false);
            times--;

            if(times <= 0)
            {
                script.enabled = true;
                Destroy(this.gameObject);
            }
        }
    }
}
