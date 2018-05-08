using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoulIcon : MonoBehaviour {

    private bool isReduce = false, isAdd = false, isBorn = false;
    private Image image;
    private RectTransform trans;
    private Vector3 scale;   //原始缩放

    void Start()
    {
        image = this.GetComponent<Image>();
        trans = this.GetComponent<RectTransform>();
        scale = trans.localScale;
    }
	
	void Update () {
	    if(isAdd)
        {
            Add();
            return;
        }
        if(isBorn)
        {
            Born();
            return;
        }
        if(isReduce)
        {
            Reduce();
            return;
        }
	}

    private float _time1 = 0;
    void Add()
    {
        float time = 0.4f;  //动画时间
        float scaleNum = 1.5f;  //放大比例
        _time1 += Time.deltaTime;
        trans.localScale = Vector2.Lerp(scale, scaleNum * scale, _time1 / time);
        image.color = Color.Lerp(new Color(1, 1, 1, 0.2f), new Color(1, 1, 1, 1), _time1 / time);
        if(_time1 > time)
        {
            trans.localScale = scale;
            _time1 = 0;
            isAdd = false;
        }
    }

    private float _time3 = 0;
    void Born()
    {
        float time = 0.5f;
        float scaleNum_X = 1.5f,scaleNum_Y = 0.5f;
        _time3 += Time.deltaTime;
        trans.localScale = Vector2.Lerp(new Vector2(scale.x * scaleNum_X, scale.y * scaleNum_Y), scale * scaleNum_X, _time3 / time);
        image.color = Color.Lerp(new Color(1, 1, 1, 0.1f), new Color(1, 1, 1, 0.2f), _time3 / time);
        if(_time3 > time)
        {
            trans.localScale = scale;
            _time3 = 0;
            isBorn = false;
        }
    }

    private float _time2 = 0;
    void Reduce()
    {
        float time = 0.2f;  //动画时间
        float scaleNum = 1.5f;  //放大比例
        _time2 += Time.deltaTime;
        trans.localScale = Vector2.Lerp(scale, scaleNum * scale, _time2 / time);
        image.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0.2f), _time2 / time);
        if (_time2 > time)
        {
            trans.localScale = scale;
            _time2 = 0;
            isReduce = false;
        }
    }

    void changeAdd()
    {
        isAdd = true;
    }

    void changeReduce()
    {
        isReduce = true;
    }

    void changeBorn()
    {
        isBorn = true;
    }
}
