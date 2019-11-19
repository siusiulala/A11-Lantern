using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordView : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void CheckPassword(InputField pwInput)
    {
        if(pwInput.text=="pass")
        {
            ToSettingView();
        }
    }

    public void ToDrawView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("2_DrawViewV1");
    }

    public void ToSettingView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("5_SettingView");
    }

}
