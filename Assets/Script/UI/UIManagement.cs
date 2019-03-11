using UnityEngine;
using System.Collections;

public class UIManagement : MonoBehaviour
{

    //UI管理

    public GameObject ArmsGemsBar;   //武器镶嵌

    private void Update()
    {
        if (MyInput.instance.isGetBag())
        {
            if (ArmsGemsBar.activeSelf)
            {
                ArmsGemsBar.SetActive(false);
                CharacterControl.instance.getInput();
            }
            else
            {
                ArmsGemsBar.SetActive(true);
                CharacterControl.instance.setInputNone();
            }
        }


    }
}
