using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Lantern : MonoBehaviour {

    public SpriteRenderer blankLantern;

    public SpriteRenderer filledLantern;

    public Text countdownText;
    public Text hintText;
    public Text launchText;
    float holdTimer = 5f;
    bool isLaunched = false;
    bool sendSignal = false;
    float lanternY = -4f;
    public GameObject finishCanvas;
    public GameObject bg;
    public GameObject bg2;
    // Use this for initialization
    void Start () {
        //GameObject.Find("System").GetComponent<Client>().Reconnect();

    }

    // Update is called once per frame
    void Update() {
        if(!isLaunched)
        {
            bool leftHold = false;
            bool rightHold = false;
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                rightHold = true;
                leftHold = true;
            }
#endif
            var tapCount = Input.touchCount;
            for (var i = 0; i < tapCount; i++)
            {
                var touch = Input.GetTouch(i);
                //if (touch.phase == TouchPhase.Began)
                //{
                if (touch.position.x > (Screen.width / 2))
                {
                    rightHold = true;
                }
                else
                {
                    leftHold = true;
                }
                //}
            }
            if (leftHold && rightHold)
            {
                hintText.gameObject.SetActive(false);
                countdownText.gameObject.SetActive(true);
                holdTimer -= Time.deltaTime;
                countdownText.text = "   " + (int)holdTimer;
                //print(holdTimer);
                if (holdTimer <= 0)
                    isLaunched = true;
            }
            else
            {
                hintText.gameObject.SetActive(true);
                countdownText.gameObject.SetActive(false);
                holdTimer = 5f;
            }
            filledLantern.color = new Color(1, 1, 1, (5f - holdTimer) / 5f);
            blankLantern.color = new Color(1, 1, 1, holdTimer / 5f);
        }
        else
        {
            if(!sendSignal)
            {
                GameObject.Find("System").GetComponent<Client>().AsyncSend("Show");
                sendSignal = true;
            }
            countdownText.gameObject.SetActive(false);
            launchText.gameObject.SetActive(true);
            lanternY += Time.deltaTime * 1.5f;
            filledLantern.transform.localPosition = new Vector3(0,lanternY, -2f);

            if(lanternY>=7f)
            {
                filledLantern.gameObject.SetActive(false);
                launchText.gameObject.SetActive(false);
                bg.SetActive(false);
                bg2.SetActive(true);
                finishCanvas.SetActive(true);
            }
        }

    }

    public void ToDrawView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("2_DrawViewV1");
    }
}
