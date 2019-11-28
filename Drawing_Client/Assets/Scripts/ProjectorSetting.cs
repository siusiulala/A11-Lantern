using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectorSetting : MonoBehaviour {


    string settingCurrentCorner = "RTop";

    Client client;

	// Use this for initialization
	void Start () {
        client = GameObject.Find("System").GetComponent<Client>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSettingLeftBtn()
    {
        client.AsyncSend("P" + client.clientId + settingCurrentCorner + "Left");
    }
    public void OnSettingRightBtn()
    {
        client.AsyncSend("P" + client.clientId + settingCurrentCorner + "Right");
    }
    public void OnSettingUpBtn()
    {
        client.AsyncSend("P" + client.clientId + settingCurrentCorner + "Up");
    }
    public void OnSettingDownBtn()
    {
        client.AsyncSend("P" + client.clientId + settingCurrentCorner + "Down");
    }

    public void OnSettingRTopCorner()
    {
        settingCurrentCorner = "RTop";

        GameObject.Find("CornerButton0").GetComponent<Image>().color = new Color(1f, 0.3f, 0.24f);
        GameObject.Find("CornerButton1").GetComponent<Image>().color = Color.white;
        GameObject.Find("CornerButton2").GetComponent<Image>().color = Color.white;
        GameObject.Find("CornerButton3").GetComponent<Image>().color = Color.white;
    }

    public void OnSettingLTopCorner()
    {
        settingCurrentCorner = "LTop";
        GameObject.Find("CornerButton1").GetComponent<Image>().color = new Color(1f, 0.3f, 0.24f);
        GameObject.Find("CornerButton0").GetComponent<Image>().color = Color.white;
        GameObject.Find("CornerButton2").GetComponent<Image>().color = Color.white;
        GameObject.Find("CornerButton3").GetComponent<Image>().color = Color.white;
    }

    public void OnSettingRBottomCorner()
    {
        settingCurrentCorner = "RBottom";
        GameObject.Find("CornerButton3").GetComponent<Image>().color = new Color(1f, 0.3f, 0.24f);
        GameObject.Find("CornerButton0").GetComponent<Image>().color = Color.white;
        GameObject.Find("CornerButton1").GetComponent<Image>().color = Color.white;
        GameObject.Find("CornerButton2").GetComponent<Image>().color = Color.white;
    }

    public void OnSettingLBottomCorner()
    {
        settingCurrentCorner = "LBottom";
        GameObject.Find("CornerButton2").GetComponent<Image>().color = new Color(1f, 0.3f, 0.24f);
        GameObject.Find("CornerButton0").GetComponent<Image>().color = Color.white;
        GameObject.Find("CornerButton1").GetComponent<Image>().color = Color.white;
        GameObject.Find("CornerButton3").GetComponent<Image>().color = Color.white;
    }

    public void OnChangePlaneId(Dropdown dropdown)
    {
        if (dropdown.value == 0)
            client.clientId = 1;
        if (dropdown.value == 1)
            client.clientId = 2;
    }

    public void ToDrawView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DrawViewV1");
    }

    public void ToSettingView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("5_SettingView");
    }
}
