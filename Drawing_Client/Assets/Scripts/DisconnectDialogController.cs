using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectDialogController : MonoBehaviour {

    public GameObject disconnectDialog;

    public void Show()
    {
        disconnectDialog.SetActive(true);
    }

    public void Hide()
    {
        disconnectDialog.SetActive(false);
    }
}
