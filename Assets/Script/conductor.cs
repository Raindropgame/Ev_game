using UnityEngine;
using System.Collections;

public class conductor : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "enemy")
        {          
            CharacterObjectManager.instance.sendHurt_other(GameData.lightning_Damage, Attribute.lightning, collision.gameObject.GetInstanceID());
            return;
        }

    }
}
