using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Collections.Specialized;

public class ArmDisarm : MonoBehaviour {

    string URL = "http://192.168.10.105/genericArgs";

	// Use this for initialization
	void Start () {
		
	}


	public void btn(){

        using (WebClient client = new WebClient())
        {

            byte[] response =
            client.UploadValues(URL, new NameValueCollection()
            {
                { "Turn","on"},
                { "ch7","2000"},
                { "ch8","1030"}
            });

            Debug.Log(response);

        //    string result = System.Text.Encoding.UTF8.GetString(response);

        }

	}
    
}
