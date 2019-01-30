using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinBar : MonoBehaviour {

    public Text coin_text;
	
	void Update () {
        coin_text.text = CharacterAttribute.GetInstance().coinNum.ToString();
	}
}
