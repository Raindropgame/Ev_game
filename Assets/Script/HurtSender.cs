using UnityEngine;
using System.Collections;

public class HurtSender : MonoBehaviour {

    public int damage;
    public Attribute attribute;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag.CompareTo("Player") == 0)
        {
            CharacterControl.instance.hurt(damage, attribute);
        }
    }
}
