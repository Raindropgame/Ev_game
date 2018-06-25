using UnityEngine;
using System.Collections;

public class CloseDirty : MonoBehaviour {

	public void close()
    {
        this.gameObject.SetActive(false);
    }
}
