using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	public class WebPrint : MonoBehaviour
	{
		/// <summary>
		/// Whether process is running or not.
		/// </summary>
		public static bool isRunning;

		/// <summary>
		/// The flash effect fade.
		/// </summary>
		public Animator flashEffect;

		/// <summary>
		/// The flash sound effect.
		/// </summary>
		public AudioClip flashSFX;

		/// <summary>
		/// The objects bet hide/show on screen capturing.
		/// </summary>
		public Transform[] objects;

		/// <summary>
		/// The logo on the bottom of the page.
		/// </summary>
		public Transform bottomLogo;


		void Start(){
			isRunning = false;
		}

		/// <summary>
		/// Print the screen.
		/// </summary>
		public void PrintScreen ()
		{
			//#if(UNITY_WEBPLAYER || UNITY_WEBGL || UNITY_EDITOR)
				//Debug.LogWarning("Print feature works only in the Web platform, check out the Manual.pdf to implement print feature...");
				StartCoroutine ("PrintScreenCoroutine");
			//#endif
		}

        Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
        {
            Color32[] original = originalTexture.GetPixels32();
            Color32[] rotated = new Color32[original.Length];
            int w = originalTexture.width;
            int h = originalTexture.height;

            int iRotated, iOriginal;

            for (int j = 0; j < h; ++j)
            {
                for (int i = 0; i < w; ++i)
                {
                    iRotated = (i + 1) * h - j - 1;
                    iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                    rotated[iRotated] = original[iOriginal];
                }
            }

            Texture2D rotatedTexture = new Texture2D(h, w);
            rotatedTexture.SetPixels32(rotated);
            rotatedTexture.Apply();
            return rotatedTexture;
        }

		public IEnumerator PrintScreenCoroutine ()
		{
			isRunning = true;

			HideObjects ();
			if(bottomLogo!=null)
				bottomLogo.gameObject.SetActive (true);
			string imageName = "DrawingAndColoring-"+System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss");

			//Capture screen shot
			yield return new WaitForEndOfFrame();
            //Texture2D texture = new Texture2D(Screen.width / 2, Screen.height / 2);
            //texture.ReadPixels(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), 0, 0);
            Texture2D texture = new Texture2D(1200 * Screen.width / 1200, 1540 * Screen.height / 1920);
            print(texture.width +","+texture.height);
            texture.ReadPixels(new Rect(0, 0, 1200 * Screen.width / 1200, 1540 * Screen.height / 1920), 0, 0);
			texture.Apply();
            //Texture2D textureR = RotateTexture(texture, false);
            /*
            Texture2D textureBig = new Texture2D(2464, 1540);
            var fillColorArray = textureBig.GetPixels();
            for (var i = 0; i < fillColorArray.Length; ++i)
            {
                fillColorArray[i] = Color.white;
            }

            textureBig.SetPixels(fillColorArray);

            textureBig.Apply();


            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color color = texture.GetPixel(x, y);
                    textureBig.SetPixel(x+632, y, color);
                }
            }
            textureBig.Apply();*/

            flashEffect.SetTrigger ("Run");
			//if(flashSFX !=null)
			//	AudioSource.PlayClipAtPoint (flashSFX, Vector3.zero, 1);
			yield return new WaitForSeconds (1);
			ShowObjects ();
			if(bottomLogo!=null)
				bottomLogo.gameObject.SetActive (false);
#if UNITY_EDITOR
            File.WriteAllBytes(Application.persistentDataPath + "/" + imageName + ".png", texture.EncodeToPNG());
#endif
            //Application.ExternalCall("PrintImage", System.Convert.ToBase64String(texture.EncodeToPNG()),imageName);
            isRunning = false;
            //GameObject.FindObjectOfType<Client>().AsyncSend(System.Convert.ToBase64String(texture.EncodeToPNG()));
            GameObject.FindObjectOfType<Client>().SendFile(texture.EncodeToPNG());
		}

		/// <summary>
		/// Hide the objects.
		/// </summary>
		private void HideObjects ()
		{
			if (objects == null) {
				return;
			}

			foreach (Transform obj in objects) {
				if(obj!=null)
					obj.gameObject.SetActive (false);
			}
		}

		/// <summary>
		/// Show the objects.
		/// </summary>
		private void ShowObjects ()
		{
			if (objects == null) {
				return;
			}
			
			foreach (Transform obj in objects) {
				obj.gameObject.SetActive (true);
			}
		}
	}
}