using UnityEngine;
using System.Collections;

public class ModifySortOrder : MonoBehaviour {

    public string SortingLayerName = "Default";
    public int SortingOrder = 0;

    private Renderer Render;

	void Start () {
        Render = GetComponent<TrailRenderer>();
        Render.sortingOrder = SortingOrder;
	}
}
