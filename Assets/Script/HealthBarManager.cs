using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour {

    //管理血量槽

    static public HealthBarManager instance;  //单例脚本

    public float soul_space_width;
    public float soul_space_height;
    public GameObject soulIcon;

    private Transform startPoint;  //血槽起始点
    private int current_HP,MaxHP;
    private ArrayList Icon;

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        startPoint = GameObject.Find("startPoint").transform;

        current_HP = CharacterAttribute.GetInstance().HP;
        MaxHP = CharacterAttribute.GetInstance().MaxHP;
        Icon = new ArrayList();
        for(int i = 0;i<MaxHP;i++)
        {
            GameObject t =  Instantiate(soulIcon, position: (Vector2)startPoint.position + new Vector2(soul_space_width * i,soul_space_height * (i%2 == 0?-1:1)), rotation: new Quaternion(0, 0, 0, 0)) as GameObject;
            t.transform.SetParent(this.transform, true);
            t.transform.localScale = new Vector3(1, 1, 1);  //改回原来的比例
            Icon.Add(t);
            if(i >= current_HP)  //     对空灵魂槽的操作
            {
                t.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
            }
        }
	}
	

	void Update () {
	    if(CharacterAttribute.GetInstance().HP > current_HP)  //HP已增加
        {
            int num = CharacterAttribute.GetInstance().HP - current_HP;
            current_HP = CharacterAttribute.GetInstance().HP;
            add_soul(num);
        }
        if(CharacterAttribute.GetInstance().HP < current_HP)  //HP已减少
        {
            int num = current_HP - CharacterAttribute.GetInstance().HP;
            current_HP = CharacterAttribute.GetInstance().HP;
            reduce_soul(num);
        }
        if(CharacterAttribute.GetInstance().MaxHP > MaxHP)  //最大HP已增加
        {
            int num = CharacterAttribute.GetInstance().MaxHP - MaxHP;
            MaxHP = CharacterAttribute.GetInstance().MaxHP;
            add_MaxSoul(num);
        }
	}

    void reduce_soul(int num)
    {
        for(int i = current_HP;i < current_HP + num;i++)
        {
            if (i > MaxHP - 1)  //防止超出数组范围
            {
                break;
            }
            ((GameObject)Icon[i]).SendMessage("changeReduce");
        }
    }

    void add_soul(int num)
    {
        for(int i = current_HP - 1;i>current_HP - num - 1;i--)
        {
            if(i > MaxHP - 1)  //防止超出数组范围
            {
                break;
            }
            ((GameObject)Icon[i]).SendMessage("changeAdd");
        }
    }

    void add_MaxSoul(int num)  //增加最大灵魂值 
    {
        for(int i = MaxHP - num;i<MaxHP;i++)
        {
            GameObject t = Instantiate(soulIcon, position: (Vector2)startPoint.position + new Vector2(soul_space_width * i, soul_space_height * (i % 2 == 0 ? -1 : 1)), rotation: new Quaternion(0, 0, 0, 0)) as GameObject;
            t.transform.SetParent(this.transform, true);
            t.transform.localScale = new Vector3(1, 1, 1);
            Icon.Add(t);
            t.SendMessage("changeBorn");  //播放动画
        }
    }
}
