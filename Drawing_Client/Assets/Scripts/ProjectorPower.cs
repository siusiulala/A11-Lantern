using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectorPower : MonoBehaviour {

    
    // Use this for initialization
    void Start () {
		
	}


    static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }

    public byte[] CommandBytes(string cmdString)
    {
        var command = System.Text.RegularExpressions.Regex.Replace(cmdString, @"\s+", "");
        command = System.Text.RegularExpressions.Regex.Replace(command, @"0x|0X|,", "");
        return StringToByteArray(command);

    }

    public void SendPowerOn()
    {
        GameObject.Find("System").GetComponent<Client>().SendCmdToProjector(CommandBytes(GetPowerOnCommand("PJLink")), GetPort("PJLink"));
        ToSettingView();
    }

    public void SendPowerOff()
    {
        GameObject.Find("System").GetComponent<Client>().SendCmdToProjector(CommandBytes(GetPowerOffCommand("PJLink")), GetPort("PJLink"));
        ToSettingView();
    }

    public void ToSettingView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("5_SettingView");
    }


    public static int GetPort(string type)
    {
        int port = 4352;
        switch (type)
        {
            case "Z15WST":
                port = 23;
                break;
            default:
                port = 4352;
                break;
        }
        return port;
    }

    public static string GetPowerOnCommand(string type)
    {
        switch (type)
        {
            // z28 rj45 use PJLink
            //case "Z28H":
            //    return "0x23, 0x30, 0x30, 0x30, 0x30, 0x20, 0x31, 0x0D";
            case "Z15WST":
                return "0x7E, 0x30, 0x30, 0x30, 0x30, 0x20, 0x31, 0x0D";
            default:
                return "0x25, 0x31, 0x50, 0x4F, 0x57, 0x52, 0x20,  0x31, 0x0D";
        }
    }

    public static string GetPowerOffCommand(string type)
    {
        switch (type)
        {
            //case "Z28H":
            //    return "0x23, 0x30, 0x30, 0x30, 0x30, 0x20, 0x30, 0x0D";
            case "Z15WST":
                return "0x7E, 0x30, 0x30, 0x30, 0x30, 0x20, 0x30, 0x0D";
            default:
                return "0x25, 0x31, 0x50, 0x4F, 0x57, 0x52, 0x20,  0x30, 0x0D";
        }
    }

    public static string GetPowerStateCommand(string type)
    {
        switch (type)
        {
            //case "Z28H":
            //    return "0x23, 0x30, 0x30, 0x31, 0x36, 0x32, 0x20, 0x31, 0x0D";
            case "Z15WST":
                return "0x7E, 0x30, 0x30, 0x31, 0x32, 0x34, 0x20, 0x31, 0x0D";
            default:
                return "0x25, 0x31, 0x50, 0x4F, 0x57, 0x52, 0x20,  0x3F, 0x0D";
        }
    }
}
