using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingView : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToDrawView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("2_DrawViewV1");
    }

    public void ToProjectorPowerView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("6_ProjectorPower");
    }

    public void ToRemoveContentView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("7_RemoveContent");
    }

    public void ToProjectorCalibrationView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("8_ProjectorCalibration");
    }

    public void ToIpSettingView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("9_IpSetting");
    }

}
