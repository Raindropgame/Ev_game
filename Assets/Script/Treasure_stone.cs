using UnityEngine;
using System.Collections;

public class Treasure_stone : MonoBehaviour {

    private bool isUsed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isUsed)
        {
            try
            {
                if (collision.tag.Substring(0, 4).CompareTo("arms") == 0)
                {
                    isUsed = true;
                    Broken();
                }
            }
            catch
            {

            }
        }
    }

    void Broken()
    {
        Destroy(this.gameObject);
        //粒子
        GameObject stone_broken = Resources.Load<GameObject>("Stone_Broken");
        Instantiate(stone_broken, position: transform.position, rotation: Quaternion.Euler(0, 0, 0));
        stone_broken = null;

        //奖励
        float t = Random.value;
        if (t < 0.5f)
        {
            GameObject Soul = Resources.Load<GameObject>("Soul");
            Instantiate(Soul, position: transform.position, rotation: Quaternion.Euler(0, 0, 0));
            Soul = null;
        }
        else
        {
            int num = Random.Range(4, 7);
            GameObject Coin = Resources.Load<GameObject>("Coin");
            for (int i = 0; i < num; i++)
            {
                ((GameObject)Instantiate(Coin, position: transform.position, rotation: Quaternion.Euler(0, 0, 0))).GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * 200);
            }
            Coin = null;
        }

        Resources.UnloadUnusedAssets();
    }

}
