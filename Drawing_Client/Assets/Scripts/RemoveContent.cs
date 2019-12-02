using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveContent : MonoBehaviour {


    void Start()
    {
        GameObject.Find("System").GetComponent<Client>().Reconnect();
    }

    public void DoRemoveContent()
    {
        GameObject.Find("System").GetComponent<Client>().AsyncSend("Clear");
        ToSettingView();
    }


    public void ToSettingView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("5_SettingView");
    }


}
