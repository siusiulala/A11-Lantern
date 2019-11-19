using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IpSetting : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("System").GetComponent<Client>().ClientDisconnect();
        
        if (PlayerPrefs.HasKey("HostIp"))
        {
            var serverIP = PlayerPrefs.GetString("HostIp");
            GameObject.Find("ServerIpInput").GetComponent<InputField>().text = serverIP;
        }

        if (PlayerPrefs.HasKey("ProjectorIp"))
        {
            var projectorIP = PlayerPrefs.GetString("ProjectorIp");
            GameObject.Find("ProjectorIpInput").GetComponent<InputField>().text = projectorIP;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ConnectToServer(InputField ipInputField)
    {
        GameObject.Find("System").GetComponent<Client>().ConnectToServer(ipInputField);
    }
}
