using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebootWindow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void RebootWindows()
    {
        GameObject.FindObjectOfType<Client>().RebootWindows();
        ToSettingView();
    }

    public void ToSettingView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("5_SettingView");
    }

}
