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
    float lanternY = -1.8f;
    public GameObject finishCanvas;

    // Use this for initialization
    void Start () {
		
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
                countdownText.text = "倒數 " + (int)holdTimer + " 秒";
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
            countdownText.gameObject.SetActive(false);
            launchText.gameObject.SetActive(true);
            lanternY += Time.deltaTime;
            filledLantern.transform.localPosition = new Vector3(0,lanternY, 0);

            if(lanternY>=7f)
            {
                filledLantern.gameObject.SetActive(false);
                launchText.gameObject.SetActive(false);
                finishCanvas.SetActive(true);
            }
        }

    }

    public void ToDrawView()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("2_DrawViewV1");
    }
}
