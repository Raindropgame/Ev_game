using UnityEngine;
using System.Collections;

public class Soul : MonoBehaviour {

    public int SoulAmout = 1;

    private GameObject SoulSprite,Halo;
    private bool isUsed = false;

    private void Start()
    {
        Halo = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "Halo");
        SoulSprite = GameFunction.GetGameObjectInChildrenByName(Halo, "SoulSprite");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isUsed)
        {
            if (collision.tag.CompareTo("Player") == 0)
            {
                isUsed = true;
                work();
            }
        }
    }

    void work()
    {
        //增加血量
        CharacterAttribute.GetInstance().add_HP(SoulAmout);

        StartCoroutine(IE_effect());
    }

    IEnumerator IE_effect()
    {
        const float duration = 0.3f;

        SoulSprite.SetActive(false);
        float Timer_0 = 0;
        Vector3 originScale = Halo.transform.localScale;
        SpriteRenderer SR_Halo = Halo.GetComponent<SpriteRenderer>();
        while(Timer_0 < duration)
        {
            Timer_0 += Time.deltaTime;
            float t = Timer_0 / duration;

            Halo.transform.localScale = Vector3.Lerp(originScale, originScale * 10, t);
            SR_Halo.color = Color.Lerp(Color.white, GameFunction.Transparent, t);

            yield return null;
        }

        Destroy(this.gameObject);
    }
}
