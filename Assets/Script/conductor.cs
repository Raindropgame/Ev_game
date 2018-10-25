using UnityEngine;
using System.Collections;

public class conductor : MonoBehaviour {

    private Collider2D colliderComp;

    private void Start()
    {
        colliderComp = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "enemy")
        {
            try
            {
                dir Dir = collision.GetComponent<Monster_base>().Dir;
                CharacterObjectManager.instance.sendHurt_other(GameData.lightning_Damage, Attribute.lightning, collision.gameObject.GetInstanceID(), Dir == dir.left ? (Vector2)colliderComp.bounds.center - Vector2.one : (Vector2)colliderComp.bounds.center + Vector2.one);
                return;
            }
            catch
            {
                CharacterObjectManager.instance.sendHurt_other(GameData.lightning_Damage, Attribute.lightning, collision.gameObject.GetInstanceID(), Vector2.zero);
                return;
            }
        }

    }
}
