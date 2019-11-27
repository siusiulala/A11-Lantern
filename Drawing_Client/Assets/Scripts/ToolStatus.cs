using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolStatus : MonoBehaviour {

    public Sprite spriteClicked;
    public Sprite spriteUnclicked;

    // Use this for initialization
    void Start () {
	}
	
	public void Click()
    {
        GetComponent<Image>().sprite = spriteClicked;
    }

    public void Unclick()
    {
        GetComponent<Image>().sprite = spriteUnclicked;
    }
}
