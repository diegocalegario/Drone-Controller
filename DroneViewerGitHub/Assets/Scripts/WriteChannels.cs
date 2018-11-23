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


public class WriteChannels : MonoBehaviour {

	int[] defaultValue = new int[4];

	private int flagch1 = 0, flagch2 = 0, flagch3 = 0,flagch4 = 0, flagArm = 0, Mov = 0;
	private int ch1, ch2, ch3 , ch4;
	private int ch5 = 1000, ch6, ch7, ch8;
	private int ch1Old, ch2Old, ch3Old, ch4Old, ch5Old, ch6Old, ch7Old, ch8Old;
	private int G1 = 50, G2 = 50, G3 = 50, G4 = 50;
	private int Tr1 = 0, Tr2 = 0, Tr3 = 0, Tr4 = 0;

	public Text G1T;
	public Text G2T;
	public Text G3T;
	public Text G4T;

	public Slider mainSlider1;
	public Slider mainSlider2;
	public Slider mainSlider3;
	public Slider mainSlider4;

	public Text ArmDisarmText;
	public Text Action;

	public Text Alarme1;
	public Text Alarme2;

	public Text g1t;
	public Text g2t;
	public Text g3t;
	public Text g4t;

	//string URL;

	string Message;
	string reply;
	string message;
	string[] Messages;

	WebSocket w = new WebSocket(new Uri("ws://espWebSock.local:81"));
	
	// Use this for initialization
	public IEnumerator Start () {

		try{
			Tr1 = PlayerPrefs.GetInt("Tr1");
			Tr2 = PlayerPrefs.GetInt("Tr2");
			Tr3 = PlayerPrefs.GetInt("Tr3");
			Tr4 = PlayerPrefs.GetInt("Tr4");
		}catch{}

		defaultValue[0] = 1500 + Tr1;
		defaultValue[1] = 1500 + Tr2;
		defaultValue[2] = 1000 + Tr3;
		defaultValue[3] = 1500 + Tr4;

		setDefault();

		//URL = PlayerPrefs.GetString("IP","");

		yield return StartCoroutine(w.Connect());
		reply = w.RecvString();
		//Debug.Log(reply);

		mainSlider1.onValueChanged.AddListener(delegate {ValueChangeCheckG1(); });
		mainSlider2.onValueChanged.AddListener(delegate {ValueChangeCheckG2(); });
		mainSlider3.onValueChanged.AddListener(delegate {ValueChangeCheckG3(); });
		mainSlider4.onValueChanged.AddListener(delegate {ValueChangeCheckG4(); });

		//Procedimento de calibração
		ch6 = 2000;
		Alarme1.text = "CALIBRATING";
		StartCoroutine(Delay(5));
		Alarme1.text = "";
		ch6 = 1500;

		// Button btn1 = TrustUp.GetComponent<Button>();
    	// Button btn2 = TrustDown.GetComponent<Button>();

		// btn1.onClick.AddListener(TrustUp_btn);
		// btn2.onClick.AddListener(TrustDown_btn);
        //Calls the TaskOnClick/TaskWithParameters method when you click the Button
        // btn1.onClick.AddListener(TaskOnClick);
        // btn2.onClick.AddListener(delegate {TaskWithParameters("Hello"); });
	}

	IEnumerator Delay(float t)
    {
        yield return new WaitForSeconds(t);
    }

	public void ValueChangeCheckG1()
    {
        G1 = (int)mainSlider1.value;
    }
	public void ValueChangeCheckG2()
    {
        G2 = (int)mainSlider2.value;
    }
	public void ValueChangeCheckG3()
    {
        G3 = (int)mainSlider3.value;
    }
	public void ValueChangeCheckG4()
    {
        G4 = (int)mainSlider4.value;
    }
	
	// Update is called once per frame
	void Update () {
		//OldTime = Time.time;
		
		if(Input.GetAxis("SobeDesce") > 0 && flagch3 == 1){
			ch3 = ch3 + G3;
			if(ch3 >2000){
				ch3 = 2000;
			}
			flagch3 = 0;
		}else if(Input.GetAxis("SobeDesce") < 0 && flagch3 == 1){
			ch3 = ch3 - G3;
			if(ch3 < 1000){
				ch3 = 1000;
			}
			flagch3 = 0;
		}else if(Input.GetAxis("SobeDesce") == 0 && flagch3 == 0)
		{
			flagch3 = 1;
		}

		if(Input.GetAxis("Roll") > 0 && flagch1 == 1){
			ch1 = ch1 + G1;
			flagch1 = 0;
		}else if(Input.GetAxis("Roll") < 0 && flagch1 == 1){
			ch1 = ch1 - G1;
			flagch1 = 0;
		}else if(Input.GetAxis("Roll") == 0 && flagch1 == 0)
		{
			ch1 = defaultValue[0];
			flagch1 = 1;
		}

		if(Input.GetAxis("Pitch") > 0 && flagch2 == 1){
			ch2 = ch2 + G2;
			flagch2 = 0;
		}else if(Input.GetAxis("Pitch") < 0 && flagch2 == 1){
			ch2 = ch2 - G2;
			flagch2 = 0;
		}else if(Input.GetAxis("Pitch") == 0 && flagch2 == 0)
		{
			ch2 = defaultValue[1];
			flagch2 = 1;
		}

		if(Input.GetAxis("Yaw") > 0 && flagch4 == 1){
			ch4 = ch4 + G4;
			flagch4 = 0;
		}else if(Input.GetAxis("Yaw") < 0 && flagch4 == 1){
			ch4 = ch4 - G4;
			flagch4 = 0;
		}else if(Input.GetAxis("Yaw") == 0 && flagch4 == 0)
		{
			ch4 = defaultValue[3];
			flagch4 = 1;
		}

		if(Input.GetButtonDown("Arm")){
			if(ch5 < 1500 && flagArm == 1){
				ch3 = 1000;
				ch5 = 1500;
				ArmDisarmText.text = "Desarm";
				Alarme1.text = "";
				flagArm = 0;
			}else if(flagArm == 1 && ch3 < 1300){
				ch3 = 1000;
				ch5 = 1000;
				ArmDisarmText.text = "Arm";
				Alarme1.text = "";
				flagArm = 0;
			}else{
				Alarme1.text = "Can't Disarm  while Thrust is ON";
			}
		}else{
			flagArm = 1;
		}

		if(Input.GetButtonDown("EmergenciaB")){
			ArmDisarmText.text = "Arm";
			setDefault();
		}

		G1T.text = G1.ToString();
		G2T.text = G2.ToString();
		G3T.text = G3.ToString();
		G4T.text = G4.ToString();

		g1t.text = ch1.ToString();
		g2t.text = ch2.ToString();
		g3t.text = ch3.ToString();
		g4t.text = ch4.ToString();

		if(ch1 != ch1Old || ch2 != ch2Old || ch3 != ch3Old || ch4 != ch4Old || ch5 != ch5Old || ch6 != ch6Old || ch7 != ch7Old || ch8 != ch8Old){
			if(ch3 >= 1400){
				Mov = 1;
			}else{
				Mov = 0;
			}
			SocketCall();
			try{
				string[] stringSeparators = new string[] {","};
				Messages = reply.Split(stringSeparators, StringSplitOptions.None);
				//Debug.Log(Messages[0]);
				//Debug.Log(Messages[1]);
				//Debug.Log(Messages[2]);
				if(Convert.ToDouble(Messages[0]) < 30){
					Alarme2.text = "Left Ultrassonic detecting collision";
				}else if(Convert.ToDouble(Messages[1]) < 30){
					Alarme2.text = "Right Ultrassonic detecting collision";
				}else if(Convert.ToDouble(Messages[2]) < 30){
					Alarme2.text = "Front Ultrassonic detecting collision";
				}else{
					Alarme2.text = "";
				}
			}catch{};
		}
	}

	public void SocketCall(){
		
		message = ch1.ToString() + "," +  ch2.ToString() + "," +  ch3.ToString() +"," +  ch4.ToString() + "," + ch5.ToString() + "," +  ch6.ToString() + "," +  ch7.ToString() + "," +  ch8.ToString() + "," + Mov.ToString();
		w.SendString(message);
		reply = w.RecvString();
		//Debug.Log(Mov);
		ch1Old = ch1;
		ch2Old = ch2;
		ch3Old = ch3;
		ch4Old = ch4;
		ch5Old = ch5;
		ch6Old = ch6;
		ch7Old = ch7;
		ch8Old = ch8;

	}

	public void TrustUp_btn()
	{
		flagch3 = 1;
		ch3 = ch3 + G3;
		if(ch3>2000){
			ch3 = 2000;
		}
	}

	public void TrustUpSoltar_btn()
	{
		flagch3 = 0;
	}

	public void TrustDown_btn()
	{
		flagch3 = 1;
		ch3 = ch3 - G3;
		if(ch3 < 1000){
			ch3 = 1000;
		}
	}
		
	public void TrustDownSoltar_btn()
	{
		flagch3 = 0;
	}



	public void AileronRight_btn()
	{
		flagch1 = 1;
		ch1 = ch1 + G1;
	}

	public void AileronRigthSoltar_btn()
	{
		flagch1 = 0;
		ch1 = Tr1;
	}
	
	public void AileronLeft_btn()
	{
		flagch1 = 1;
		ch1 = ch1 - G1;
	}
		
	public void AileronLeftSoltar_btn()
	{
		flagch1 = 0;
		ch1 = Tr1;
	}



	public void LemeRight_btn()
	{
		flagch4 = 1;
		ch4 = ch4 + G4;
	}

	public void LemeRigthSoltar_btn()
	{
		flagch4 = 0;
		ch4 = Tr4;
	}

	public void LemeLeft_btn()
	{
		flagch4 = 1;
		ch4 = ch4 - G4;
	}
		
	public void LemeLeftSoltar_btn()
	{
		flagch4 = 0;
		ch4 = Tr4;
	}



	public void PitchUp_btn()
	{
		flagch2 = 1;
		ch2 = ch2 + G2;
	}

	public void PitchUpSoltar_btn()
	{
		flagch2 = 0;
		ch2 = Tr2;
	}

	public void PitchDown_btn()
	{
		flagch2 = 1;
		ch2 = ch2 - G2;
	}
		
	public void PitchDownSoltar_btn()
	{
		flagch2 = 0;
		ch2 = Tr2;
	}


	public void TrimCh1Up_btn()
	{
		Tr1 = Tr1 + 1;
		PlayerPrefs.SetInt("Tr1", Tr1);
		defaultValue[0] = 1500 + Tr1;
		ch1 = defaultValue[0];
	}

	public void TrimCh2Up_btn()
	{
		Tr2 = Tr2 + 1;
		PlayerPrefs.SetInt("Tr2", Tr2);
		defaultValue[1] = 1500 + Tr2;
		ch2 = defaultValue[1];
	}

	public void TrimCh3Up_btn()
	{
		Tr3 = Tr3 + 1;
		PlayerPrefs.SetInt("Tr3", Tr3);
		defaultValue[2] = 1000 + Tr3;
		ch3 = defaultValue[2];
	}

	public void TrimCh4Up_btn()
	{
		Tr4 = Tr4 + 1;
		PlayerPrefs.SetInt("Tr4", Tr4);
		defaultValue[3] = 1500 + Tr4;
		ch4 = defaultValue[3];
	}
	public void TrimCh1Down_btn()
	{
		Tr1 = Tr1 - 1;
		PlayerPrefs.SetInt("Tr1", Tr1);
		defaultValue[0] = 1500 + Tr1;
		ch1 = defaultValue[0];
	}

	public void TrimCh2Down_btn()
	{
		Tr2 = Tr2 - 1;
		PlayerPrefs.SetInt("Tr2", Tr2);
		defaultValue[1] = 1500 + Tr2;
		ch2 = defaultValue[1];
	}

	public void TrimCh3Down_btn()
	{
		Tr3 = Tr3 - 1;
		PlayerPrefs.SetInt("Tr3", Tr3);
		defaultValue[2] = 1000 + Tr3;
		ch3 = defaultValue[2];
	}

	public void TrimCh4Down_btn()
	{
		Tr4 = Tr4 - 1;
		PlayerPrefs.SetInt("Tr4", Tr4);
		defaultValue[3] = 1500 + Tr4;
		ch4 = defaultValue[3];
	}

	public void Back(){
		if(ArmDisarmText.text == "Disarm"){
			Alarme1.text = "Can't Back while Armed";
		}else{
			ArmDisarmText.text = "Arm";
			setDefault();
			SocketCall();
			SceneManager.LoadScene("Principal");
		}
	}

	public void Emergency(){
		ArmDisarmText.text = "Arm";
		setDefault();
		SocketCall();
		Alarme1.text = "";
	}

	public void Acao(){
		if(Action.text == "Take-Off"){
			Action.text = "Land";
			Decolar();
		}else{
			Action.text = "Take-Off";
			Pousar();
		}
	}

	public void Decolar(){
		for(int i=0; i<=400; i=i+80){
			ch3 = 1000 + i;
			SocketCall();
			StartCoroutine(Delay(0.5F));
		}
		Mov = 1;
		SocketCall();
	}

	public void Pousar(){
		if(ch3>=1400){
			int ch3Aux = ch3;
			for(int i=0; i<=(ch3Aux-1000); i=i+10){
				ch3 = ch3Aux - i;
				if(ch3<1400){
					Mov = 0;
				}
				SocketCall();
				StartCoroutine(Delay(0.1F));
			}
		}else{
			while(ch3!=1000){
				ch3--;
				StartCoroutine(Delay(0.1F));
			}

		}
	}

	public void setDefault(){
		ch1 = defaultValue[0];
		ch2 = defaultValue[1];
		ch3 = defaultValue[2];
		ch4 = defaultValue[3];
		ch5 = 1000;
		ch6 = 1500;
		ch7 = 1500;
		ch8 = 1500;
		ch1Old = defaultValue[0];
		ch2Old = defaultValue[1];
		ch3Old = defaultValue[2];
		ch4Old = defaultValue[3];
		ch5Old = 1000;
		ch6Old = 1500;
		ch7Old = 1500;
		ch8Old = 1500;
	}

	public void Arm(){
		if(ch5 < 1500){
			setDefault();
			ch5 = 1500;
			ArmDisarmText.text = "Disarm";
			Alarme1.text = "";
			SocketCall();
		}else if(ch3 < 1300){
			setDefault();
			SocketCall();
			ArmDisarmText.text = "Arm";
			Alarme1.text = "";
		}else{
			Alarme1.text = "Can't Disarm  while Thrust is ON";
		}
	}
}