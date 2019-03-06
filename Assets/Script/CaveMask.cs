using UnityEngine;
using System.Collections;

public class CaveMask : MonoBehaviour {

    //幕布的显示与隐藏

    public SpriteRenderer[] SR;
    public float hideTime;

    private bool isStay = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (!isStay)
            {
                isStay = true;
                StartCoroutine(hide());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (isStay)
            {
                isStay = false;
                StartCoroutine(show());
            }
        }
    }

    IEnumerator hide()
    {
        float _time = 0;
        Color t;
        float a = SR[0].color.a;
        while (_time < hideTime)
        {
            _time += Time.deltaTime;

            for(int i = 0;i<SR.Length;i++)
            {
                t = SR[i].color;
                t.a = Mathf.Lerp(a, 0, _time / hideTime);
                SR[i].color = t;
            }
            yield return null;
        }       
    }

    IEnumerator show()
    {
        float _time = 0;
        Color t;
        float a = SR[0].color.a;
        while (_time < hideTime)
        {
            _time += Time.deltaTime;

            for (int i = 0; i < SR.Length; i++)
            {
                t = SR[i].color;
                t.a = Mathf.Lerp(a, 1, _time / hideTime);
                SR[i].color = t;
            }
            yield return null;
        }
    }
}
