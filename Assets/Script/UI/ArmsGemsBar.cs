using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArmsGemsBar : MonoBehaviour
{
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
    public Image[] GemGroove_Disable, GemGroove_None, Gem;
    public Color GemGrooveDisableColor;
    public GameObject[] Dirty;  //穿上粒子特效
    [Header("--------徽章属性----------")]
    public float changeScale;
    public float playTime;
    public float headShakeScale;
    public int shakeRatio;
    public int shakeFrequency;
    [Header("--------结晶格子属性----------")]
    public Image[] GemItem;   //结晶格子
    public Color outlineColor = Color.blue;
    public Material OutlineMaterial;
    [Header("--------消息框属性----------")]
    public RectTransform InfoBoxRect;
    public Text[] InfoBox;  //0:名字  1：描述
    public float showTime;
    public Vector2 W_H;  //长和高
    [Header("--------武器碎片属性---------")]
    public Text armsFrag_Text;
    public int PerGrooveFragNum = 1;  //开一个槽所需的碎片数
    public GameObject[] Frags;
    public GameObject Frag_icon;
    public float openGroove_first_animation_duration;
    public float openGroove_second_animation_duration;
    public GameObject OpenNewGroove_MessageBox;

    private RectTransform[] point;
    private ArrayList ArmsList = new ArrayList();
    private ArmsInfo[] armsInfo;
    private int currentArms; // 当前选中的武器
    private Outline headOutline;
    private Vector3 HeadOriginPos;  //徽章的初始位置
    private string[][] GemInfo;  //结晶的描述信息
    private bool isInt = false;
    private GameObject GrooveMessageBox_select = null;
    private int currentSelectGemGrooveID;

    private void Start()
    {
        headOutline = GameObject.Find("Head").GetComponent<Outline>();
        HeadOriginPos = headOutline.gameObject.transform.position;

        GemInfo = readInfo();  //读取结晶信息

        //--初始化
        point = new RectTransform[3];
        point[2] = GameObject.Find("arms_point_left").GetComponent<RectTransform>();
        point[0] = GameObject.Find("arms_point_middle").GetComponent<RectTransform>();
        point[1] = GameObject.Find("arms_point_right").GetComponent<RectTransform>();

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
        updateArmsFragNum();  //更新武器槽碎片数量
    }

    private void OnEnable()
    {
        if (isInt)
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


            headOutline.effectDistance = Vector2.zero;   //初始化徽章的状态
            isPlayAnimation_0 = false;
            headOutline.gameObject.transform.position = HeadOriginPos;

            InfoBoxRect.gameObject.SetActive(false);

            for (int i = 0; i < Dirty.Length; i++)
            {
                Dirty[i].SetActive(false);  //防止重复播放
            }

            //初始化开启武器槽动画
            for(int i = 0;i<Frags.Length;i++)
            {
                Frags[i].transform.position = Frag_icon.transform.position;
                Frags[i].SetActive(false);
            }
            Frags[0].GetComponent<TrailRenderer>().enabled = true;
            Frags[0].transform.GetChild(0).gameObject.SetActive(true);
            Frags[0].transform.localScale = Frags[1].transform.localScale;

            //提示框隐藏
            OpenNewGroove_MessageBox.SetActive(false);
            isAnimation_openGroove = false;

            updateGemGroove();  //更新当前武器槽的显示
            updateGemItemShow(); //更新所携带的结晶
            updateArmsFragNum();  //更新武器槽碎片数量
        }
        else
        {
            isInt = true;
        }
    }


    public void onClickArms(GameObject t)  //武器的点击事件
    {
        for (int i = 0; i < armsInfo.Length; i++)  //判断是否点击的为当前选中的武器
        {
            if (t.name == armsInfo[i].name)
            {
                if (armsInfo[i].pos == 0)
                {
                    return;   //点击的为中间的武器
                }
            }
        }
        if (t.transform.position.x > point[0].position.x)  //判断点击的位置
        {
            currentArms++;
            currentArms = currentArms > 2 ? 0 : currentArms;
            for (int i = 0; i < armsInfo.Length; i++)  //改变位置
            {
                armsInfo[i].pos--;
                if (armsInfo[i].pos < 0)
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
        while (true)
        {
            t_time += Time.deltaTime;
            for (int i = 0; i < armsInfo.Length; i++)
            {
                int lastPos; //计算之前的位置
                if (Dir == dir.left)
                {
                    lastPos = armsInfo[i].pos + 1;
                    if (lastPos > 2)
                    {
                        lastPos = 0;
                    }
                }
                else
                {
                    lastPos = armsInfo[i].pos - 1;
                    if (lastPos < 0)
                    {
                        lastPos = 2;
                    }
                }

                armsInfo[i].rectTrans.position = Vector3.Slerp(point[lastPos].position, point[armsInfo[i].pos].position, t_time / armsMoveTime);  //插值位置
                if (armsInfo[i].pos == 0)
                {
                    armsInfo[i].rectTrans.localScale = Vector2.Lerp(new Vector2(1, 1), new Vector2(maxArmsScale, maxArmsScale), t_time / armsMoveTime);   //插值缩放
                    armsInfo[i].image.color = Color.Lerp(backArmsColor, new Color(1, 1, 1, 1), t_time / armsMoveTime);   //插值颜色
                }
                else
                {
                    armsInfo[i].rectTrans.localScale = Vector2.Lerp(armsInfo[i].rectTrans.localScale, new Vector2(1, 1), t_time / armsMoveTime);
                    armsInfo[i].image.color = Color.Lerp(armsInfo[i].image.color, backArmsColor, t_time / armsMoveTime);   //插值颜色
                }

            }

            if (t_time > armsMoveTime)  //是否结束
            {
                break;
            }
            yield return null;
        }
    }

    void updateGemGroove()  //更新武器槽的显示
    {
        ArmsGemGroove currentGemGroove;

        for (int i = 0; i < Gem.Length; i++)  //初始化武器上结晶的大小
        {
            Gem[i].transform.localScale = new Vector3(1, 1, 1);
        }

        switch (armsInfo[currentArms].name)  //获取当前武器槽的信息
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

        for (int i = 0; i < GemGroove.Length; i++)
        {
            if (i < currentGemGroove.currentGemNum)
            {
                GemGroove_Disable[i].gameObject.SetActive(false);
                GemGroove[i].GetComponent<Image>().color = Color.white;
                GemGroove[i].GetComponent<Button>().enabled = false;
                if (currentGemGroove.GemItem[i] == null)  //有格子无物品
                {
                    GemGroove_None[i].gameObject.SetActive(true);
                    Gem[i].gameObject.SetActive(false);
                }
                else//  添加当前所镶嵌宝石的图片    有格子有物品
                {
                    Gem[i].color = Color.white;
                    GemGroove_None[i].gameObject.SetActive(false);
                    Gem[i].gameObject.SetActive(true);
                    switch (armsInfo[currentArms].name)
                    {
                        case "swords":
                            Gem[i].sprite = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords].GemItem[i].sprite;
                            break;
                        case "arrow":
                            Gem[i].sprite = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.arrow].GemItem[i].sprite;
                            break;
                        case "spear":
                            Gem[i].sprite = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.spear].GemItem[i].sprite;
                            break;
                    }
                }
            }
            else   //无格子
            {
                GemGroove_Disable[i].gameObject.SetActive(true);
                GemGroove_None[i].gameObject.SetActive(false);
                Gem[i].gameObject.SetActive(false);
                GemGroove[i].GetComponent<Image>().color = GemGrooveDisableColor;
                GemGroove[i].GetComponent<Button>().enabled = true;
            }
        }
    }

    private void opneNewGemGroove(int i)   //开启一个新的武器槽
    {
        int currentNum = 0;
        i -= 1;

        switch (armsInfo[currentArms].name)   //更新人物数据
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

        StartCoroutine(IE_openNewGroove(currentNum - 1));

        GemGroove[currentNum - 1].GetComponent<Button>().enabled = false;   //禁用按钮

        Bag.getInstance().ConsumeFragment(PerGrooveFragNum);  //消耗碎片
        
    }

    private bool isPlayAnimation_0 = false;
    public void clickHead()  //点头
    {
        if (!isPlayAnimation_0)
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
            if (_time0 > playTime / shakeRatio)
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
        while (true)
        {
            _time0 += Time.deltaTime;

            if (_time0 < playTime / 2)
            {
                headOutline.effectDistance = Vector2.Lerp(Vector2.zero, new Vector2(changeScale, changeScale), _time0 / (playTime / 2));
            }
            else
            {
                headOutline.effectDistance = Vector2.Lerp(new Vector2(changeScale, changeScale), Vector2.zero, (_time0 - playTime / 2) / (playTime / 2));
            }

            if (_time0 > playTime)
            {
                break;
            }
            yield return null;
        }

        isPlayAnimation_0 = false;
    }

    void updateGemItemShow()   //更新结晶格子的显示
    {
        for (int i = 0; i < GemItem.Length; i++)
        {
            GemItem[i].transform.localScale = new Vector3(1, 1, 1);  //初始化大小

            if (Bag.getInstance().GemItem[i] == null)  //如果没物品
            {
                GemItem[i].sprite = null;
                GemItem[i].color = new Color(1, 1, 1, 0);
                GemItem[i].material = null;
            }
            else
            {
                GemItem[i].color = Color.white;
                GemItem[i].sprite = Bag.getInstance().GemItem[i].sprite;
                GemItem[i].material = null;
            }
        }
    }

    public void putOnGem(int ItemIndex)
    {
        if (Bag.getInstance().GemItem[ItemIndex] != null)  //当前格子是否有物品
        {
            switch (armsInfo[currentArms].name)   //更新人物数据
            {
                case "swords":
                    if (Bag.getInstance().GemItem[ItemIndex].Take(Arms.swords))
                    {
                        int i = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.swords].takeOn(Bag.getInstance().GemItem[ItemIndex]);  //装备结晶
                        Bag.getInstance().GemItem[ItemIndex] = null;  //该格子变为空

                        updateGemGroove();  //更新显示
                        updateGemItemShow();
                        InfoBoxRect.gameObject.SetActive(false);

                        StartCoroutine(Animation_putonGem(i));
                        return;
                    }
                    break;
                case "arrow":
                    if (Bag.getInstance().GemItem[ItemIndex].Take(Arms.arrow))
                    {
                        int i = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.arrow].takeOn(Bag.getInstance().GemItem[ItemIndex]);   //装备结晶
                        Bag.getInstance().GemItem[ItemIndex] = null;

                        updateGemGroove();  //更新显示
                        updateGemItemShow();
                        InfoBoxRect.gameObject.SetActive(false);

                        StartCoroutine(Animation_putonGem(i));
                        return;
                    }
                    break;
                case "spear":
                    if (Bag.getInstance().GemItem[ItemIndex].Take(Arms.spear))
                    {
                        int i = CharacterAttribute.GetInstance().ArmsGemGroove[(int)Arms.spear].takeOn(Bag.getInstance().GemItem[ItemIndex]);    //装备结晶
                        Bag.getInstance().GemItem[ItemIndex] = null;

                        updateGemGroove();  //更新显示
                        updateGemItemShow();
                        InfoBoxRect.gameObject.SetActive(false);

                        StartCoroutine(Animation_putonGem(i));
                        return;
                    }
                    break;
            }
        }

        StartCoroutine(Animation_shake(GemItem[ItemIndex].transform));  //播放无法穿戴动画
    }

    Arms name2type(string t)  //查询武器
    {
        Arms o = Arms.swords;
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
        if (Bag.getInstance().getLeftItem() <= 0)
        {
            return;  //背包空间不足
        }
        Arms type = name2type(armsInfo[currentArms].name);
        CharacterAttribute.GetInstance().ArmsGemGroove[(int)type].GemItem[i].TakeOff();
        int t = Bag.getInstance().putGemIntoBag(CharacterAttribute.GetInstance().ArmsGemGroove[(int)type].GemItem[i]);
        CharacterAttribute.GetInstance().ArmsGemGroove[(int)type].GemItem[i] = null;

        //更新显示
        updateGemGroove();
        updateGemItemShow();

        StartCoroutine(Animation_putoffGem(t));  //动画
        return;
    }

    public void OnMouseEnterGen(int i)  //当鼠标进入当前结晶
    {
        if (Bag.getInstance().GemItem[i] != null)   //当这个格子不为空
        {
            GemItem[i].material = OutlineMaterial;
            showInfoBox(i);   //描述
        }
    }

    public void OnMouseExitGen(int i)  //当鼠标退出当前结晶
    {
        GemItem[i].material = null;
        InfoBoxRect.gameObject.SetActive(false);
    }

    IEnumerator Animation_shake(Transform trans)  //无法穿戴动画
    {
        Vector3 originPos = trans.position;
        int times = 8, _times = 0;  //摇动次数
        float shakeScale = 3;  //摇动幅度
        while (true)
        {
            _times++;
            trans.position = originPos + (Vector3)Random.insideUnitCircle * shakeScale;

            if (_times > times)
            {
                break;
            }
            yield return new WaitForSeconds(0.03f);
        }
        trans.position = originPos;
    }

    public void showInfoBox(int i)
    {
        //摆放到正确位置
        //显示该物品的描述
        InfoBoxRect.gameObject.SetActive(true); //激活
        InfoBoxRect.position = Input.mousePosition;

        if (InfoBoxRect.anchoredPosition.x + InfoBoxRect.sizeDelta.x > 400)  //摆放到正确位置 防止越界
        {
            InfoBoxRect.anchoredPosition = new Vector2(400 - InfoBoxRect.sizeDelta.x, InfoBoxRect.anchoredPosition.y);
        }
        if (InfoBoxRect.anchoredPosition.y - InfoBoxRect.sizeDelta.y < -225)
        {
            InfoBoxRect.anchoredPosition = new Vector2(InfoBoxRect.anchoredPosition.x, -225 + InfoBoxRect.sizeDelta.y);
        }

        StartCoroutine(showInfo(i, GemInfo[Bag.getInstance().GemItem[i].index]));
    }

    IEnumerator Animation_showInfoBox()   //消息框出现动画
    {
        float _time = 0;

        InfoBox[0].gameObject.SetActive(false);
        InfoBox[1].gameObject.SetActive(false);
        while (true)
        {
            _time += Time.deltaTime;

            InfoBoxRect.sizeDelta = Vector2.Lerp(Vector2.zero, W_H, _time / showTime);

            if (_time > showTime)
            {
                InfoBox[0].gameObject.SetActive(true);   //播放完动画才出现文字
                InfoBox[1].gameObject.SetActive(true);
                break;
            }
            yield return null;
        }
    }

    IEnumerator showInfo(int i, string[] info)  //等待动画完成 添加描述
    {
        yield return StartCoroutine(Animation_showInfoBox());
        InfoBox[0].text = info[1];
        InfoBox[1].text = info[2];
    }

    string[][] readInfo()  //从文件读取结晶的描述 0:序号  1：名字  2：描述
    {
        string[][] o;
        TextAsset text = Resources.Load("GemInfo") as TextAsset;
        string t_text = text.text;
        Resources.UnloadUnusedAssets();  //释放内存

        string[] t = t_text.Split("\n".ToCharArray());
        o = new string[t.Length][];
        for (int i = 0; i < t.Length; i++)
        {
            o[i] = t[i].Split(" ".ToCharArray());
        }
        return o;
    }

    IEnumerator Animation_putonGem(int i)   //穿戴上结晶的特效
    {
        float time1 = 0.3f, _time1 = 0;
        Vector2 _scale = new Vector2(2f, 2f);

        while (true)   //第一阶段
        {
            _time1 += Time.deltaTime;

            Gem[i].GetComponent<RectTransform>().localScale = Vector3.Lerp(_scale, Vector3.one, _time1 / time1);
            if (_time1 > time1)
            {
                break;
            }
            yield return null;
        }
        Dirty[i].transform.position = GemGroove[i].transform.position;   //尘土粒子特效
        Dirty[i].SetActive(true);
    }

    IEnumerator Animation_putoffGem(int i)   //卸下结晶的特效
    {
        float time1 = 0.25f, _time1 = 0;
        Vector2 _scale = new Vector2(1.6f, 1.6f);

        while (true)   //第一阶段
        {
            _time1 += Time.deltaTime;

            GemItem[i].GetComponent<RectTransform>().localScale = Vector3.Lerp(_scale, Vector3.one, _time1 / time1);
            if (_time1 > time1)
            {
                break;
            }
            yield return null;
        }

    }

    void updateArmsFragNum()
    {
        armsFrag_Text.text = Bag.getInstance().getArmsFragment().ToString();
    }

    IEnumerator IE_openNewGroove(int GrooveID)
    {
        //第一段动画
        {
            for(int i = 0;i<Frags.Length;i++)
            {
                Frags[i].SetActive(true);
            }

            float _time0 = 0;

            Vector3 originScale_FragIcon = Frag_icon.transform.localScale;
            Vector3[] originPos_Frags = new Vector3[Frags.Length];
            Vector3 _dir, _dir_2;
            Vector3 p1;
            Vector3[] p2 = new Vector3[Frags.Length];
            _dir = Frags[0].transform.position - Gem[GrooveID].transform.position;
            _dir_2 = Vector3.Cross(_dir, Vector3.back);
            p1 = Frags[0].transform.position + _dir * 0.5f;
            for(int i = 0;i<Frags.Length;i++)
            {
                p2[i] = Frags[0].transform.position + _dir_2 * (0.5f - i * 0.1f) * (i % 2 == 0 ? 1 : -1);
                originPos_Frags[i] = Frags[i].transform.position;
            }

            updateArmsFragNum();
            while (_time0 < openGroove_first_animation_duration)
            {
                _time0 += Time.deltaTime;
                float t = _time0 / openGroove_first_animation_duration;
                Frag_icon.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, originScale_FragIcon, t * 8);
                for(int i = 0;i<Frags.Length;i++)
                {
                    Frags[i].transform.position = GameFunction.BezierLine(originPos_Frags[i], Gem[GrooveID].transform.position, p1, p2[i], t);
                }

                yield return null;
            }
        }

        for(int i = 1;i<Frags.Length;i++)
        {
            Frags[i].SetActive(false);
            Frags[i].transform.position = Frag_icon.transform.position;
        }

        //第二段动画
        { 
            Frags[0].GetComponent<TrailRenderer>().enabled = false;
            Frags[0].transform.GetChild(0).gameObject.SetActive(false);
            float _time0 = 0;

            Image img_frag = Frags[0].GetComponent<Image>();
            Color originColor_frag = img_frag.color;
            Vector3 originScale_Frag = Frags[0].transform.localScale;
            bool isUpdate = false;
            while(_time0 < openGroove_second_animation_duration)
            {
                _time0 += Time.deltaTime;
                float t = _time0 / openGroove_second_animation_duration;
                Frags[0].transform.localScale = Vector3.Lerp(originScale_Frag, Vector3.one * 33f, t);
                img_frag.color = Color.Lerp(originColor_frag, originColor_frag * GameFunction.Transparent, t);

                if(!isUpdate && _time0 > 0.3f * openGroove_second_animation_duration)
                {
                    GemGroove[GrooveID].GetComponent<Image>().color = Color.white;  //更新显示
                    GemGroove_Disable[GrooveID].gameObject.SetActive(false);
                    GemGroove_None[GrooveID].gameObject.SetActive(true);
                }

                yield return null;
            }
            Frags[0].GetComponent<TrailRenderer>().enabled = true;
            Frags[0].transform.localScale = originScale_Frag;
            Frags[0].transform.position = Frag_icon.transform.position;
            img_frag.color = originColor_frag;
            Frags[0].SetActive(false);
            Frags[0].transform.GetChild(0).gameObject.SetActive(true);
        }

    }

    private bool isAnimation_openGroove = false;
    public void OpenNewGroove_MessageBox_show(int i)
    {
        if (Bag.getInstance().getArmsFragment() >= PerGrooveFragNum)
        {
            if(!isAnimation_openGroove)
            {
                StartCoroutine(IE_TipBox_Move(i));
            }
        }
    }  //打开提示消息框

    public void OpenGroove_Button_mouseEnter(Transform t)
    {
        if(GrooveMessageBox_select.activeSelf == false)
        {
            GrooveMessageBox_select.SetActive(true);
        }
        GrooveMessageBox_select.transform.position = t.position;
    }

    public void OpenGroove_Button_mouseExit()
    {
        if (GrooveMessageBox_select.activeSelf == false)
        {
            return;
        }
        GrooveMessageBox_select.SetActive(false);
    }

    public void getGrooveMessageBoxBotton(int i)
    {
        if(i == 1)
        {
            opneNewGemGroove(currentSelectGemGrooveID);
        }
        OpenNewGroove_MessageBox.SetActive(false);
    }

    IEnumerator IE_TipBox_Move(int i)
    {
        isAnimation_openGroove = true;

        currentSelectGemGrooveID = i;
        OpenNewGroove_MessageBox.SetActive(true);
        GameFunction.GetGameObjectInChildrenByName(OpenNewGroove_MessageBox, "Text").GetComponent<Text>().text = string.Format("是否消耗<color=red>{0}</color>个碎片开启新的宝石槽？", PerGrooveFragNum);

        if (GrooveMessageBox_select == null)
        {
            GrooveMessageBox_select = GameFunction.GetGameObjectInChildrenByName(OpenNewGroove_MessageBox, "Select");
        }

        GrooveMessageBox_select.SetActive(false);

        const float first_animation_duration = 0.2f;
        RectTransform t_rect = OpenNewGroove_MessageBox.GetComponent<RectTransform>();
        {
            float _time0 = 0;           
            while(_time0 < first_animation_duration)
            {
                _time0 += Time.deltaTime;
                float t = _time0 / first_animation_duration;
                t_rect.anchoredPosition = Vector3.Lerp(Vector3.left * 710, Vector3.zero, t);

                yield return null;
            }
        }

        const float second_animation_duration = 0.3f;
        {
            float _time0 = 0;
            while(_time0 < second_animation_duration)
            {
                _time0 += Time.deltaTime;
                float t = _time0 / second_animation_duration;
                t_rect.anchoredPosition = Vector3.zero + Vector3.right * Mathf.Sin(Mathf.Lerp(0, 360, t) * Mathf.Deg2Rad) * Mathf.Lerp(50,30,t);

                yield return null;
            }
        }

        isAnimation_openGroove = false;
    }
}
