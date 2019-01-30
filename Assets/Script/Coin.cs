using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    private SpriteRenderer bl_SR;
    private SpriteRenderer coin_SR;
    private SpriteRenderer bloom_SR;
    private float a = 0, last_a = 0;
    private float b = 0;
    private Vector3 randomOffset;
    private Vector3 originScale_bl;
    private bool isEnable = true;
    private GameObject Effect;

	void Start () {
        bl_SR = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "bl").GetComponent<SpriteRenderer>();
        coin_SR = GetComponent<SpriteRenderer>();
        bloom_SR = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "bloom").GetComponent<SpriteRenderer>();
        Effect = GameFunction.GetGameObjectInChildrenByName(this.gameObject, "Effect");

        Effect.SetActive(false);
        randomOffset.z = -0.1f;
        originScale_bl = bl_SR.transform.localScale;
	}

    private void FixedUpdate()
    {
        //金光闪闪动画
        if (isEnable)
        {
            b += Time.deltaTime;
            a = Mathf.Sin(b * 5);

            if (a > 0)
            {
                bl_SR.transform.localScale = a * originScale_bl;
                bl_SR.color = GameFunction.getColor(bl_SR.color.r, bl_SR.color.g, bl_SR.color.b, a);
            }

            if (last_a > 0 && a < 0)
            {
                randomOffset.x = Random.Range(-coin_SR.bounds.extents.x, coin_SR.bounds.extents.x);
                randomOffset.y = Random.Range(-coin_SR.bounds.extents.y, coin_SR.bounds.extents.y);
                bl_SR.transform.position = coin_SR.bounds.center + randomOffset;
            }

            last_a = a;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.CompareTo("Player") == 0 && isEnable)
        {
            isEnable = false;
            bloom_SR.color = GameFunction.Transparent;
            coin_SR.color = GameFunction.Transparent;
            bl_SR.color = GameFunction.Transparent;
            CharacterAttribute.GetInstance().coinNum++; //加钱
            StartCoroutine(animation_getCoin());
        }
    }

    //拾取特效
    IEnumerator animation_getCoin()
    {
        const float duration = 0.2f;
        float Timer = 0;
        Vector3 originScale = Effect.transform.localScale;

        Effect.SetActive(true);
        while(Timer < duration)
        {
            Timer += Time.deltaTime;
            float a = Timer / duration;

            originScale.x = Mathf.Lerp(originScale.x, 0, a);
            Effect.transform.localScale = originScale;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
