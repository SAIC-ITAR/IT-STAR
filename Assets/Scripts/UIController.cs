using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UIController : MonoBehaviour {

	public GameObject smallMenuOcc;
	public GameObject aboutRepairOcc;
	public GameObject largeMenuOcc;
	public GameObject congrats;
	public GameObject close;
	public GameObject laptopBorder;
	public GameObject mainMarkerCanvas;
	public GameObject ARCamera;
	public GameObject[] Models = new GameObject[7];
	public Canvas[] NamePlates = new Canvas[7];
	public Material BlueMaterial;
	public Material DimBlue;
	public AudioSource soundPlayer;
	public AudioListener sound;
	public Animator anim1;
	public Animator anim2;
	public TouchController touchController;
	public ButtonControl buttonControl;
	public TestController testController;
	public string[][] Facts;
	public int factIndex = 0;
	public int lastMenuOn = 0;
	public Text MuteText;
	public Text infoText;
	public int repairCounter;
	public AudioClip abouton;
	public AudioClip repair;
	public AudioClip assessment; 
	public AudioClip click;
	public ParticleSystem particlesystem;
	public ParticleSystem particlesystem1;
	public ParticleSystem particlesystem2;
	public ParticleSystem particlesystem3;
	public ParticleSystem particlesystem4;
	public GameObject preparationSteps;
	public GameObject frameText;
	public GameObject instructionsText;
	public GameObject marker;
	public Collider hdRaycast;
	public Collider cdRaycast;
	public Collider ramRaycast;
	public Collider wifiRaycast;
	public Collider fanRaycast;
	public Collider batteryRaycast; 
	public GameObject fullOccluder;
	public bool occluderOn = false;
	public AudioClip fireworks; 
	public bool fireworksBool;
	public bool congratsCheck;
	public int occludeCounter;
	public GameObject exitCheck;
	public GameObject menuOccluder;
	public GameObject fullOccluderBegin;
	public bool prepStepsBool;

	// Use this for initialization
	void Start () {
		//preparationText = GameObject.Find("Preparation Steps").GetComponent<Text>();
		infoText = GameObject.Find("InfoText").GetComponent<Text>();
		//preparationHeading = GameObject.Find("Preparation Heading").GetComponent<Text>();

		anim1.SetBool ("panelLeft", false);
		anim2.SetBool ("instructionsUp", false);
		//mainMarkerCanvas.GetComponent<Canvas> ().enabled = false;
		//infoText.enabled = false;
		hdRaycast = GameObject.Find("HD_Raycast target").GetComponent<BoxCollider>();
		cdRaycast = GameObject.Find("CD_Raycast target").GetComponent<BoxCollider>();
		ramRaycast = GameObject.Find("RAM_Raycast target").GetComponent<BoxCollider>();
		wifiRaycast = GameObject.Find("WiFi_Raycast target").GetComponent<BoxCollider>();
		fanRaycast = GameObject.Find("Fan_Raycast target").GetComponent<BoxCollider>();
		batteryRaycast = GameObject.Find("Battery_Raycast target").GetComponent<BoxCollider>(); 

	}



	//------------------------------------------Button fuctions--------------------------------------------------------------------------

	public void Fireworks(){
		touchController.title.SetActive (false);
		fullOccluder.SetActive (true);

		anim1.SetBool ("panelRight", true);
		anim2.SetBool ("instructionsUp", false);
		testController.Glow [touchController.menuOn].enabled = true;
		testController.undim ();
		touchController.menuOn = 0;
		smallMenuOcc.SetActive (false);
		aboutRepairOcc.SetActive (false);

		particlesystem.Play ();
		particlesystem1.Play ();
		particlesystem2.Play ();
		particlesystem3.Play ();
		particlesystem4.Play ();
		congrats.SetActive (true);
		close.SetActive (true);
		anim1.SetBool ("panelRight", true);
		anim2.SetBool ("instructionsUp", false);
		testController.undim ();
		touchController.menuOn = 0;
		smallMenuOcc.SetActive (false);
		soundPlayer.PlayOneShot(fireworks);
		fireworksBool = true;
		congratsCheck = true;
		StartCoroutine(FireworksSounds());
	}


	public void QuitQuestionOn(){
		if (buttonControl.userGuidePanel.activeSelf) {
			buttonControl.userGuideOn (false);
		} 
		else if (prepStepsBool){
			screwOn ();
		}
		else {
			exitCheck.SetActive (true);
			fullOccluder.SetActive (true);
			menuOccluder.SetActive (true);
		}
	}

	public void QuitQuestionOff(){
		exitCheck.SetActive(false);
		fullOccluder.SetActive (false);
		menuOccluder.SetActive (false);

	}


	public void Quit(){
		
			Application.Quit ();
		
	}
		
	public void AboutOn(){
		aboutRepairOcc.SetActive (true);
		smallMenuOcc.SetActive (false);

		if (touchController.menuOn != 0){
			anim1.SetBool ("panelSmall", true);

		mainMarkerCanvas.GetComponent<Canvas> ().enabled = true;
		infoText.enabled = true;
		//preparationText.enabled = false;
		//preparationHeading.enabled = false;
		touchController.FirstMenu = false;
		Facts = StringPool.AFacts;
		mainMarkerCanvas.GetComponentInChildren<Text> ().text = Facts [touchController.menuOn] [0];
		factIndex = 0;
		soundPlayer.clip = abouton;
		soundPlayer.Play();
		buttonControl.removeFirstMenu ();
		}
	}  

	//tutorial instructions
	public void RepairOn(){
		aboutRepairOcc.SetActive (true);
		smallMenuOcc.SetActive (false);
		if (touchController.menuOn != 0){
			anim1.SetBool ("panelSmall", true);
		repairCounter = 1;
		mainMarkerCanvas.GetComponent<Canvas> ().enabled = true;
		infoText.enabled = true;
		//preparationText.enabled = false;
		//preparationHeading.enabled = false;
		touchController.FirstMenu = false;
		Facts = StringPool.RFacts;
		mainMarkerCanvas.GetComponentInChildren<Text> ().text = Facts [touchController.menuOn][0];
		factIndex = 0;
		soundPlayer.clip = abouton;
		soundPlayer.Play ();
		buttonControl.removeFirstMenu ();
		testController.Glow [touchController.menuOn].enabled = false;

		for (int i = 1;i < testController.Glow.Length;i++) {
			NamePlates[i].enabled = false;
		}
		}
		}

	public void left(){


		if (repairCounter > 0) {
			repairCounter -= 1;
		}


		if (factIndex > 0) {
			factIndex--;
			mainMarkerCanvas.GetComponentInChildren<Text> ().text = Facts [touchController.menuOn][factIndex];
		}
		if (factIndex == 0) {
				buttonControl.LeftOn (false);
		}
		if (factIndex == Facts[touchController.menuOn].Length - 2) {
				buttonControl.rightOn (true);
		}
	}

	public void right(){

		if (repairCounter > 0) {
			repairCounter++;
		}


		if (factIndex <  Facts[touchController.menuOn].Length - 1) {
			factIndex++;
			mainMarkerCanvas.GetComponentInChildren<Text> ().text = Facts [touchController.menuOn][factIndex];
		}
		if (factIndex == Facts [touchController.menuOn].Length - 1) {
				buttonControl.rightOn (false);
		}
		if (factIndex == 1) {
				buttonControl.LeftOn (true);
		}
	}







	public void TestOn () {
		aboutRepairOcc.SetActive (true);
		testController.scoreHolder = 0;
		testController.allcorrect = true;
		if (touchController.menuOn != 0){
		soundPlayer.clip = abouton;
		soundPlayer.Play ();
		anim1.SetBool ("testOn", true);
		touchController.FirstMenu = false;
		testController.TestOn (touchController.menuOn);
		largeMenuOcc.SetActive (true);
		}
	}

	public void Back(){
		aboutRepairOcc.SetActive (false);

		testController.wrongFlag = false;
		testController.correctFlag = false;
		if (fireworksBool == true) {
		Fireworks ();
		}

		congratsCheck = false;
		anim1.SetBool ("panelSmall", false);
		testController.moved = false;



		if (touchController.FirstMenu) {

			touchController.title.SetActive (false);

			anim1.SetBool ("panelRight", true);
			anim2.SetBool ("instructionsUp", false);
			testController.Glow [touchController.menuOn].enabled = true;


			testController.undim ();
			if (testController.qNumber < 5) {



				testController.scores [testController.modelNum] = -1;


			}
			
			touchController.menuOn = 0;
			smallMenuOcc.SetActive (false);
			aboutRepairOcc.SetActive (false);

		} else {
			touchController.FirstMenu = true;

			repairCounter = 0;

		/*	if (anim1.GetBool ("testOn") && testController.qNumber < 4) {
				testController.scores [touchController.menuOn] = -1;
			}
			*/
			anim1.SetBool ("testOn", false);
			mainMarkerCanvas.GetComponent<Canvas> ().enabled = false;
			buttonControl.bringFirstMenu ();
			testController.Glow [touchController.menuOn].enabled = true;
			//testController.Glow [touchController.menuOn].material = BlueMaterial;
			for (int i = 1;i < testController.Glow.Length;i++) {
				NamePlates[i].enabled = true;
			}
			largeMenuOcc.SetActive (false);



		


			//makes sure that the correct model is active, if turned off in the repair section
			switch (touchController.menuOn) {
			case 1: //Battery
				touchController.batteryRenderer.enabled = true;
				break;
			case 2: //CD Drive
				touchController.cdRenderer.enabled = true;
				break;
			case 3: //Fan Assembly
				touchController.fanRenderer.enabled = true;
				break;
			case 4: //Hard Drive
				touchController.hdRenderer.enabled = true;
				break;
			case 5: //RAM
				touchController.ramRenderer.enabled = true;
				break;
			case 6: //WiFi
				touchController.wifiRenderer.enabled = true;
				break;
			}
		}
		StartCoroutine (touchController.TitleActive ());

	}


	public IEnumerator FireworksSounds(){
		Debug.Log ("fireworks sounds");
		yield return new WaitForSeconds (.3f);
		soundPlayer.PlayOneShot(fireworks);
		yield return new WaitForSeconds (.4f);
		soundPlayer.PlayOneShot(fireworks);
		yield return new WaitForSeconds (.3f);
		soundPlayer.PlayOneShot(fireworks);
		yield return new WaitForSeconds (.5f);
		soundPlayer.PlayOneShot(fireworks);
	}

	public void AudioClick1(){
		soundPlayer.clip = abouton;

		soundPlayer.Play ();
	}

	public void mute() {
		if (AudioListener.pause){
			AudioListener.pause = false;
			MuteText.text = "Mute";

		}
		else{
			AudioListener.pause = true;
			MuteText.text = "Unmute";
		}
	}

	public void ScoreReset(){
		closeCongrats ();
		testController.hdScore = -1;
		testController.cdScore = -1;
		testController.batteryScore = -1;
		testController.wifiScore = -1;
		testController.fanScore = -1;
		testController.ramScore = -1;
		testController.dim (touchController.menuOn);


		}


	public void screwOn () {
		prepStepsBool = !prepStepsBool;
		occluderOn = !occluderOn;
		fullOccluder.SetActive (occluderOn);
		if (touchController.menuOn == 0){
		mainMarkerCanvas.GetComponent<Canvas> ().enabled = !mainMarkerCanvas.GetComponent<Canvas> ().enabled;
		infoText.enabled = !infoText.enabled;
		preparationSteps.SetActive (!preparationSteps.activeSelf);
		}
	}

	public void closeCongrats () {
		fullOccluder.SetActive (false);

		congratsCheck = false;
		fireworksBool = false;
		congrats.SetActive (false);
		close.SetActive (false);
		for (int i = 1; i < 7; i++) {
			testController.Glow [i].material = testController.DimGreenMaterial;
		}
	}

	public void trackingVisualsOn (bool b) {
		
		if ((b == false) || (Input.GetKeyDown("j"))) {
			ARCamera.transform.position = new Vector3 (-3220, -576, 520);
			ARCamera.transform.LookAt (marker.transform);
			//47, 180, 5
			hdRaycast.enabled = true;
			cdRaycast.enabled = true;
			fanRaycast.enabled = true;
			batteryRaycast.enabled = true;
			ramRaycast.enabled = true;
			wifiRaycast.enabled = true; 

		}
		laptopBorder.SetActive (!b);
		frameText.SetActive (!b);
		instructionsText.SetActive (b);

	}



	void Update () {
		if (occludeCounter < 60) {
			occludeCounter++;
		} else if (occludeCounter == 60) {
			fullOccluderBegin.SetActive (false);

		}


		if (Input.GetKeyDown("f")){
			Fireworks ();
	}
		if (Input.GetKeyDown ("g")) {
				testController.hdScore = 7;
				testController.cdScore = 8;
				testController.ramScore = 9;
				//testController.batteryScore = 8;
				testController.wifiScore = 9;
				testController.fanScore = 10;
		}
			


		if ((touchController.menuOn != lastMenuOn) && (lastMenuOn  != 0) && (!anim1.GetBool ("testOn"))) {
			mainMarkerCanvas.GetComponent<Canvas> ().enabled = false;
			anim1.SetBool ("panelSmall", false);
			if (!touchController.FirstMenu) {
				buttonControl.bringFirstMenu ();
			}
		}
		if (touchController.menuOn != lastMenuOn) {
			testController.Glow [lastMenuOn].enabled = true;
			touchController.FirstMenu = true;
			lastMenuOn = touchController.menuOn;
			repairCounter = 0;
		}
	}
}
	