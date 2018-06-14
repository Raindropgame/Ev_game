using UnityEngine;
using System.Collections;

public class UIManagement : MonoBehaviour
{

    //UI管理

    public GameObject ArmsGemsBar;   //武器镶嵌

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ArmsGemsBar.activeSelf)
            {
                ArmsGemsBar.SetActive(false);
            }
            else
            {
                ArmsGemsBar.SetActive(true);
            }
        }


    }
}
