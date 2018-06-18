using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArmsGemsBar : MonoBehaviour {

    //武器镶嵌UI管理

    public struct ArmsInfo
    {
        public RectTransform rectTrans;
        public string name;
        public int pos;
        public Image image;
    }

    [Header("--------武器栏属性----------")]
    public float maxArmsScale = 1.2f;
    public RectTransform sword, arrow, spear;
    public float armsMoveTime;  //移动时间
    public Color backArmsColor;  //在后面的武器的颜色
    [Header("--------武器槽栏属性----------")]
    public GameObject[] GemGroove;
    public GameObject[] GemGroove_Disable, GemGroove_None, Gem;
    public Color GemGrooveDisableColor;
    [Header("--------徽章属性----------")]
    public float changeScale;
    public float playTime;
    public float headShakeScale;
    public int shakeRatio;
    public int shakeFrequency;
    [Header("--------结晶属性----------")]
    public Image[] GemItem;   //结晶格子

    private RectTransform[] point;
    private ArrayList ArmsList = new ArrayList();
    private ArmsInfo[] armsInfo;
    private int currentArms; // 当前选中的武器
    private Outline headOutline;
    private Vector3 HeadOriginPos;  //徽章的初始位置

    private void Start()
    {
        headOutline = GameObject.Find("Head").GetComponent<Outline>();
        HeadOriginPos = headOutline.gameObject.transform.position;

        //--初始化
        point = new RectTransform[3];
        point[2] = GameObject.Find("arms_point_left").GetComponent<RectTransform>();
        point[0] = GameObject.Find("arms_point_middle").GetComponent<RectTransform>();
        point[1] = GameObject.Find("arms_point_right").GetComponent<RectTransform>();

        //初始化各个武器的位置
        if(CharacterAttribute.GetInstance().isEnable[(int)state.attack1])  //检查当前有哪些武器
        {
            ArmsList.Add(sword);
        }
        else
        {
            sword.gameObject.SetActive(false);
        }
        if (CharacterAttribute.GetInstance().isEnable[(int)state.shoot])
        {
            ArmsList.Add(arrow);
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
        if (CharacterAttribute.GetInstance().isEnable[(int)state.jumpshoot])
        {
            ArmsList.Add(spear);
        }
        else
        {
            spear.gameObject.SetActive(false);
        }
        armsInfo = new ArmsInfo[ArmsList.Count];
        for(int i = 0;i<ArmsList.Count;i++)
        {
            ((RectTransform)ArmsList[i]).position = point[i].position;

            armsInfo[i].rectTrans = ((RectTransform)ArmsList[i]);
            armsInfo[i].name = ((RectTransform)ArmsList[i]).name;
            armsInfo[i].pos = i;
            armsInfo[i].image = ((RectTransform)ArmsList[i]).gameObject.GetComponent<Image>();

            if(i == 0)   //改变样式
            {
                ((RectTransform)ArmsList[i]).localScale = new Vector2(maxArmsScale, maxArmsScale);
                armsInfo[i].image.color = Color.white;

                currentArms = i;
            }
            else
            {
                ((RectTransform)ArmsList[i]).localScale = new Vector2(1, 1);
                armsInfo[i].image.color = backArmsColor;
            }
        }

        updateGemGroove();  //更新当前武器槽的显示
        updateGemItemShow(); //更新所携带的结晶
    }

    private void OnDisable()
    {
        ArmsList.Clear();
        //初始化各个武器的位置
        if (CharacterAttribute.GetInstance().isEnable[(int)state.attack1])  //检查当前有哪些武器
        {
            ArmsList.Add(sword);
        }
        else
        {
            sword.gameObject.SetActive(false);
        }
        if (CharacterAttribute.GetInstance().isEnable[(int)state.shoot])
        {
            ArmsList.Add(arrow);
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
        if (CharacterAttribute.GetInstance().isEnable[(int)state.jumpshoot])
        {
            ArmsList.Add(spear);
        }
        else
        {
            spear.gameObject.SetActive(false);
        }
        armsInfo = new ArmsInfo[ArmsList.Count];
        for (int i = 0; i < ArmsList.Count; i++)
        {
            ((RectTransform)ArmsList[i]).position = point[i].position;

            armsInfo[i].rectTrans = ((RectTransform)ArmsList[i]);
            armsInfo[i].name = ((RectTransform)ArmsList[i]).name;
            armsInfo[i].pos = i;
            armsInfo[i].image = ((RectTransform)ArmsList[i]).gameObject.GetComponent<Image>();

            if (i == 0)   //改变样式
            {
                ((RectTransform)ArmsList[i]).localScale = new Vector2(maxArmsScale, maxArmsScale);
                armsInfo[i].image.color = Color.white;

                currentArms = i;
            }
            else
            {
                ((RectTransform)ArmsList[i]).localScale = new Vector2(1, 1);
                armsInfo[i].image.color = backArmsColor;
            }
        }

        updateGemGroove();  //更新当前武器槽的显示
        updateGemItemShow(); //更新所携带的结晶

        headOutline.effectDistance = Vector2.zero;   //初始化徽章的状态
        isPlayAnimation_0 = false;
        headOutline.gameObject.transform.position = HeadOriginPos;
    }


    public void onClickArms(GameObject t)  //武器的点击事件
    {
        for(int i = 0;i<armsInfo.Length;i++)  //判断是否点击的为当前选中的武器
        {
            if(t.name == armsInfo[i].name)
            {
                if(armsInfo[i].pos == 0)
                {
                    return;   //点击的为中间的武器
                }
            }
        }
        if(t.transform.position.x > point[0].position.x)  //判断点击的位置
        {
            currentArms++;
            currentArms = currentArms > 2 ? 0 : currentArms;
            for(int i = 0;i<armsInfo.Length;i++)  //改变位置
            {
                armsInfo[i].pos--;
                if(armsInfo[i].pos < 0)
                {
                    armsInfo[i].pos = 2;
                }
            }
            StartCoroutine(moveArms(dir.left));
        }
        else
        {
            currentArms--;
            currentArms = currentArms < 0 ? 2 : currentArms;
            for (int i = 0; i < armsInfo.Length; i++)  //改变位置
            {
                armsInfo[i].pos++;
                if (armsInfo[i].pos > 2)
                {
                    armsInfo[i].pos = 0;
                }
            }
            StartCoroutine(moveArms(dir.right));
        }

        updateGemGroove();  //更新当前武器槽的显示
    }

    IEnumerator moveArms(dir Dir)  //移动武器到当前位置
    {
        float t_time = 0;
        while(true)
        {
            t_time += Time.deltaTime;
            for(int i = 0;i<armsInfo.Length;i++)
            {
                int lastPos; //计算之前的位置
                if(Dir == dir.left)
                {
                    lastPos = armsInfo[i].pos + 1;
                    if(lastPos > 2)
                    {
                        lastPos = 0;
                    }
                }
                else
                {
                    lastPos = armsInfo[i].pos - 1;
                    if(lastPos < 0)
                    {
                        lastPos = 2;
                    }
                }

                armsInfo[i].rectTrans.position = Vector3.Slerp(point[lastPos].position, point[armsInfo[i].pos].position, t_time / armsMoveTime);  //插值位置
                if(armsInfo[i].pos == 0)    
                {
                    armsInfo[i].rectTrans.localScale = Vector2.Lerp(new Vector2(1,1), new Vector2(maxArmsScale, maxArmsScale), t_time / armsMoveTime);   //插值缩放
                    armsInfo[i].image.color = Color.Lerp(backArmsColor, new Color(1, 1, 1, 1), t_time / armsMoveTime);   //插值颜色
                }
                else
                {
                    armsInfo[i].rectTrans.localScale = Vector2.Lerp(armsInfo[i].rectTrans.localScale, new Vector2(1, 1), t_time / armsMoveTime);
                    armsInfo[i].image.color = Color.Lerp(armsInfo[i].image.color, backArmsColor, t_time / armsMoveTime);   //插值颜色
                }
                
            }

            if(t_time > armsMoveTime)  //是否结束
            {
                break;
            }
            yield return null;
        }
    }

    void updateGemGroove()  //更新武器槽的显示
    {
        ArmsGemGroove currentGemGroove;
        switch(armsInfo[currentArms].name)  //获取当前武器槽的信息
        {
            case "swords":
                currentGemGroove = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords];
                break;
            case "arrow":
                currentGemGroove = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.arrow];
                break;
            case "spear":
                currentGemGroove = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.spear];
                break;
            default:
                currentGemGroove = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords];
                break;
        }

        for(int i = 0;i<GemGroove.Length;i++)
        {
            if(i < currentGemGroove.currentGemNum)
            {
                GemGroove_Disable[i].SetActive(false);  
                GemGroove[i].GetComponent<Image>().color = Color.white;
                GemGroove[i].GetComponent<Button>().enabled = false;
                if (currentGemGroove.GemItem[i] == null)  //有格子无物品
                {
                    GemGroove_None[i].SetActive(true);
                    Gem[i].SetActive(false);
                }
                else//  添加当前所镶嵌宝石的图片    有格子有物品
                {
                    GemGroove_None[i].SetActive(false);
                    Gem[i].SetActive(true);
                    switch (armsInfo[currentArms].name)
                    {
                        case "swords":
                            Gem[i].GetComponent<Image>().sprite = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords].GemItem[i].sprite;
                            break;
                        case "arrow":
                            Gem[i].GetComponent<Image>().sprite = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.arrow].GemItem[i].sprite;
                            break;
                        case "spear":
                            Gem[i].GetComponent<Image>().sprite = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.spear].GemItem[i].sprite;
                            break;
                    }
                }
            }
            else   //无格子
            {
                GemGroove_Disable[i].SetActive(true);
                GemGroove_None[i].SetActive(false);
                Gem[i].SetActive(false);
                GemGroove[i].GetComponent<Image>().color = GemGrooveDisableColor;
                GemGroove[i].GetComponent<Button>().enabled = true;
            }
        }
    }

    public void opneNewGemGroove(int i)   //开启一个新的武器槽
    {
        int currentNum = 0;
        i -= 1;

        switch(armsInfo[currentArms].name)   //更新人物数据
        {
            case "swords":
                CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords].currentGemNum++;
                currentNum = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords].currentGemNum;
                break;
            case "arrow":
                CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.arrow].currentGemNum++;
                currentNum = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.arrow].currentGemNum;
                break;
            case "spear":
                CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.spear].currentGemNum++;
                currentNum = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.spear].currentGemNum;
                break;
        }

        GemGroove[currentNum - 1].GetComponent<Image>().color = Color.white;  //更新显示
        GemGroove_Disable[currentNum - 1].SetActive(false);
        GemGroove_None[currentNum - 1].SetActive(true);

        GemGroove[currentNum - 1].GetComponent<Button>().enabled = false;   //禁用按钮
    }

    private bool isPlayAnimation_0 = false;
    public void clickHead()  //点头
    {
        if(!isPlayAnimation_0)
        {
            isPlayAnimation_0 = true;
            StartCoroutine(HeadAnimation());
            StartCoroutine(HeadShakeAnimation());
        }
    }

    IEnumerator HeadShakeAnimation()
    {
        float _time0 = 0;
        Vector3 originPos = headOutline.gameObject.transform.position;  //原始的位置
        while (true)
        {
            _time0 += Time.deltaTime;

            headOutline.gameObject.transform.position = originPos + (Vector3)Random.insideUnitCircle * headShakeScale;
            if(_time0 > playTime / shakeRatio)
            {
                break;
            }
            yield return new WaitForSeconds(playTime / shakeRatio / shakeFrequency);   //摇动次数为6
        }
        headOutline.gameObject.transform.position = originPos;
    }

    IEnumerator HeadAnimation()
    {
        float _time0 = 0;
        while(true)
        {
            _time0 += Time.deltaTime;

            if(_time0 < playTime / 2)
            {
                headOutline.effectDistance = Vector2.Lerp(Vector2.zero, new Vector2(changeScale, changeScale), _time0 / (playTime / 2));
            }
            else
            {
                headOutline.effectDistance = Vector2.Lerp(new Vector2(changeScale, changeScale), Vector2.zero, (_time0 - playTime / 2) / (playTime / 2));
            }

            if(_time0 > playTime)
            {
                break;
            }
            yield return null;
        }

        isPlayAnimation_0 = false;
    }

    void updateGemItemShow()   //更新结晶格子的显示
    {
        for(int i = 0;i<GemItem.Length;i++)
        {
            if(Bag.getInstance().GemItem[i] == null)  //如果没物品
            {
                GemItem[i].sprite = null;
                GemItem[i].color = new Color(1, 1, 1, 0);
            }
            else
            {
                GemItem[i].color = Color.white;
                GemItem[i].sprite = Bag.getInstance().GemItem[i].sprite;
            }
        }
    }

    public void putOnGem(int ItemIndex)
    {
        if(Bag.getInstance().GemItem[ItemIndex] != null)  //当前格子是否有物品
        {
            switch (armsInfo[currentArms].name)   //更新人物数据
            {
                case "swords":
                    if (Bag.getInstance().GemItem[ItemIndex].Take(Arms.swords))
                    {
                        CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords].takeOn(Bag.getInstance().GemItem[ItemIndex]);  //装备结晶
                        Bag.getInstance().GemItem[ItemIndex] = null;  //该格子变为空

                        updateGemGroove();  //更新显示
                        updateGemItemShow();
                    }
                    break;
                case "arrow":
                    if (Bag.getInstance().GemItem[ItemIndex].Take(Arms.arrow))
                    {
                        CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.arrow].takeOn(Bag.getInstance().GemItem[ItemIndex]);   //装备结晶
                        Bag.getInstance().GemItem[ItemIndex] = null;

                        updateGemGroove();  //更新显示
                        updateGemItemShow();
                    }
                    break;
                case "spear":
                    if (Bag.getInstance().GemItem[ItemIndex].Take(Arms.spear))
                    {
                        CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.spear].takeOn(Bag.getInstance().GemItem[ItemIndex]);    //装备结晶
                        Bag.getInstance().GemItem[ItemIndex] = null;

                        updateGemGroove();  //更新显示
                        updateGemItemShow();
                    }
                    break;
            }
        }
    }

    Arms name2type(string t)  //查询武器
    {
        Arms o = Arms.swords ;
        switch (t)   //更新人物数据
        {
            case "swords":
                o = Arms.swords;
                break;
            case "arrow":
                o = Arms.arrow;
                break;
            case "spear":
                o = Arms.spear;
                break;
        }
        return o;
    }

    public void TakeOffGem(int i)  //卸下结晶
    {
        //放入背包
        //从武器槽脱下
        //属性重设
        if(Bag.getInstance().getLeftItem() <= 0)
        {
            return;  //背包空间不足
        }
        Arms type = name2type(armsInfo[currentArms].name);
        CharacterAttribute.GetInstance().ArmsGemGroove[(int)type].GemItem[i].TakeOff();
        Bag.getInstance().putGemIntoBag(CharacterAttribute.GetInstance().ArmsGemGroove[(int)type].GemItem[i]);
        CharacterAttribute.GetInstance().ArmsGemGroove[(int)type].GemItem[i] = null;

        //更新显示
        updateGemGroove();
        updateGemItemShow();
        return;
    }

}
