using UnityEngine;
using System.Collections;

public class hurtBack : MonoBehaviour {
    public int damage;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            CharacterControl.instance.hurt(damage);
        }
    }
}
