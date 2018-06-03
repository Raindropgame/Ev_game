using UnityEngine;
using System.Collections;

public class PlayAnimationDis : MonoBehaviour {

    //播放完动画销毁该物体

    public void destroy()
    {
        Destroy(this.gameObject);
    }
}
