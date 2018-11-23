using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using UnityEngine.Networking;
using System;
using System.Linq;
using UnityEngine.SceneManagement;


public class ChangeSceneScript : MonoBehaviour {
	public InputField TextInput;
	//public Text Battery;

	private String URL1, URL2,GOscene;

	private string bateria;
	private string Message;
	private string[] Messages; 
	private string message;

	public Text Fail;
	public Image Box;
	public bool boxOn;

/*
	public void ChangeScene(string sceneName){
		Application.LoadLevel(sceneName);
	}
 */

 	public void Start () {
		if(boxOn){
        	Box.enabled = true;
		}else{
			Box.enabled = false;
		}
    }

	public void update(){

	}

 	public void ChangeScenePrincipal(){
		PlayerPrefs.SetString("Scene", "Principal");
		SceneManager.LoadScene("Controles");
	}
	public void ChangeSceneWebCam(){
		PlayerPrefs.SetString("Scene", "WebCam");
		SceneManager.LoadScene("Controles");
	}

	public void ChangeSceneIf(string sceneName){
		StartCoroutine(TryChangeScene());

	}

	IEnumerator Delay()
    {
        //print(Time.time);
        yield return new WaitForSeconds(1);
        //print(Time.time);
    }

	public IEnumerator TryChangeScene(){
						
		URL1 = TextInput.text;		

		WebSocket w = new WebSocket(new Uri(string.Concat("ws://",string.Concat(URL1,":81"))));
		StartCoroutine(Delay());
		yield return StartCoroutine(w.Connect());
		w.SendString("1000");
		StartCoroutine(Delay());
				
		string reply = w.RecvString();
		//Debug.Log(reply);
		if (reply != null)
		{
			Fail.text = "OK";
			SceneManager.LoadScene("WebCam");
		}else{
			//
			reply = w.RecvString();
			if(reply != null){
				Fail.text = "OK";
				SceneManager.LoadScene("WebCam");
			}else{
				reply = w.RecvString();
				if(reply != null){
					Fail.text = "OK";
					SceneManager.LoadScene("WebCam");
				}else{
					Fail.text = "Fail To Connect To IP";
					Box.enabled = true;
				}
			}
		}
		if (w.error != null)
		{
			Fail.text = "Fail To Connect To IP";
			Box.enabled = true;
		}
		w.Close();
	}

	public void ChangeSceneBack(){

		if(PlayerPrefs.GetString("Scene") == "WebCam"){
			SceneManager.LoadScene("WebCam");
		}else{
			SceneManager.LoadScene("Principal");
		}
	}
}
