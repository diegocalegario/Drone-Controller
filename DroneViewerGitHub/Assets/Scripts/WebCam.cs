using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WebCam : MonoBehaviour {


    int currentCamIndex = 0;
    WebCamTexture tex;

    public RawImage display;

    public Text startstopText;

    public  void SwapCam_Clicked()
    {
        if(WebCamTexture.devices.Length > 0)
        {
            currentCamIndex += 1;
            currentCamIndex %= WebCamTexture.devices.Length;

            //if tex is not null:
            // stop the webcam
            // start the webcam

            if(tex != null)
            {
                StopWebCam();
                StarStopCam_Clicked();
            }
        }
    }

    public  void StarStopCam_Clicked()
    {
        if(tex != null) //Stop Camera
        {
            StopWebCam();
            startstopText.text = "Start Camera";
        }
        else //Start Camera
        {
            WebCamDevice device = WebCamTexture.devices[currentCamIndex];
            tex = new WebCamTexture(device.name);
            display.texture = tex;

			float	antiRotate = -(360 - tex.videoRotationAngle);
			Quaternion	quatRot = new	Quaternion ();
			quatRot.eulerAngles = new	Vector3 (0, 0, antiRotate);

				display.transform.rotation = quatRot;

            tex.Play();
            startstopText.text = "Stop Camera";
        }
    }

    private void StopWebCam()
    {
        display.texture = null;
        tex.Stop();
        tex = null;
    }
}
