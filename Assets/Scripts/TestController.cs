using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestController : MonoBehaviour
{
	public Text[] TestText = new Text[7];//text components of answer buttons
	public Text question;//text component of question
	public Renderer[] Glow = new Renderer[7];//Renderers for glowing models
	public Material RedMaterial;//Material to show a less than perfect score
	public Material GreenMaterial;//Material to show a perfect score
	public Material BlueMaterial;
	public Material DimRedMaterial;
	public Material DimGreenMaterial;
	public Material DimBlueMaterial;
	public int[] scores = new int[7];//Scores for each model
	public int selected;//Holds which answer has been tapped last
	public ButtonControl buttonControl;//Reference for ButtonControll methods
	public string[] repairAnswer;//Holds user's ordered steps in the repair section
	public int modelNum;//The current model that is being tested on
	public int qNumber;//The current question number
	public AudioSource audioplay; //{get {return GetComponent<AudioSource> (); }}  
	public AudioClip correct;
	public AudioClip wrong;  
	public AudioClip questions; 
	public UIController uiController;
	public TouchController track;
	public AudioClip pass;
	public AudioClip fail; 
	public AudioClip fireworks; 
	public bool allcorrect = true; // checks whether all of the rpelacement steps were ordered crrectly.  This is set to true by default and to false if the steps are not ordered correctly
	//the score holder variables get the score for that part at the end of the test and store it.  scoreholder is the temporary storage during the test
	public int scoreHolder = -1;
	public int hdScore = -1;
	public int cdScore = -1;
	public int ramScore = -1;
	public int batteryScore = -1;
	public int wifiScore = -1;
	public int fanScore = -1;

	public TouchController touchController; 
	public bool moved; // checks whether a step has been moved in the replacement part of the test
	public bool wrongFlag;//checks whether the sound for getting the repair steps incorrect has been played, keeps it from playing multiple times
	public bool correctFlag; //checks whether the sound for getting the repair steps correct has been played, keeps it from playing multiple times



	void Awake () {
		//sets all of the scroesto -1, which the sets the material to blue in undim() and dim()
		for (int i = 0; i < scores.Length; i++) {
			scores [i] = -1;
		}
		for (int i = 1; i < Glow.Length; i++) {
		}
	}

	public void TestOn (int UIMenuON) {//pass touchController.menuOn, called from UIController
		modelNum = UIMenuON;//reset the testing variables
		scores [modelNum] = 0;
		qNumber = 0; 

		repairAnswer = new string[StringPool.RFacts [modelNum].Length];//Initialize repairAnswers with the correct repair steps 
		for (int i = 0; i < StringPool.RFacts [modelNum].Length; i++) {
			repairAnswer [i] = StringPool.RFacts [modelNum] [i];
		}

		question.text = StringPool.TFacts [0, modelNum, 0];//Set the first question and answers
		question.color = Color.white;

		for (int i = 0;i < 4;i++) {
			TestText [i].color = Color.white;
			TestText [i].text = StringPool.TFacts [i+1, modelNum, 0];
		}


		StartCoroutine(buttonControl.testButtons ());//Display the test elements


	}

	public void up () {//called by up arrow button during repair section
		moved = true;
		string temp = "";

		temp = repairAnswer [selected-1];//swap the selected step for the one above it
		repairAnswer [selected-1] = repairAnswer [selected-2];
		repairAnswer [selected-2] = temp;

		for (int i = 0; i < repairAnswer.Length; i++) {//re-display the array
			TestText [i].text = repairAnswer [i].Substring (3);
			TestText [i].color = Color.white;
		}
		selected--;//move the selection up
		TestText [selected-1].color = Color.cyan;

		if (selected == 1) {//hide the up button if the first option is selected
			buttonControl.upOn (false);
			buttonControl.downOn (true);
		} else {
			buttonControl.upOn (true);
			buttonControl.downOn (true);
		}
	}

	public void down () {//It's like up() but backwards
		string temp = "";
		moved = true;

		temp = repairAnswer [selected-1];
		repairAnswer [selected-1] = repairAnswer [selected];
		repairAnswer [selected] = temp;

		for (int i = 0; i < repairAnswer.Length; i++) {
			TestText [i].text = repairAnswer [i].Substring (3);
			TestText [i].color = Color.white;
		}
		selected++;
		TestText [selected-1].color = Color.cyan;

		if (selected == repairAnswer.Length) {
			buttonControl.downOn (false);
			buttonControl.upOn (true);
		} else {
			buttonControl.upOn (true);
			buttonControl.downOn (true);
		}
	}

	public void choose(int s) {//called when any answer is tapped, passes it's number (1-7)
		selected = s;//change the selection and assosiated visuals

		audioplay=GameObject.Find("New Test Holder").GetComponent<AudioSource>();
		audioplay.clip= questions;
		audioplay.Play();

		for (int i = 0; i < 7; i++) {
			TestText [i].color = Color.white;
		}
		TestText [selected-1].color = Color.cyan;

		if (qNumber == 4) {//if the user is in the repair section
			if (selected == 1) {//hide or show the arrow buttons if slected is at the top or bottom
				buttonControl.upOn (false);
				buttonControl.downOn (true);
			} else if (selected == repairAnswer.Length) {
				buttonControl.downOn (false);
				buttonControl.upOn (true);
			} else {
				buttonControl.upOn (true);
				buttonControl.downOn (true);
			}
		}
	}


	public void Update(){
		if (Input.GetKeyDown ("m")) {
			hdScore = 7;
		}
		if (Input.GetKeyDown ("n")) {
			cdScore = 8;
		}
		if (Input.GetKeyDown ("b")) {
			ramScore = 9;
		}
		if (Input.GetKeyDown ("v")) {
			batteryScore = 8;
		}
		if (Input.GetKeyDown ("c")) {
			wifiScore = 9;
		}
		if (Input.GetKeyDown ("x")) {
			fanScore = 10;
		}
	}


	public void next() {//Called when the next button is tapped
		scoreHolder = scores[modelNum];

		if (selected != 12) {//Checks if an answer is being submitted, 12 means that no answer button is selected and a new question needs to be loaded, otherwise the selected answer is checked
			if (qNumber < 4) {//Checks if the test is in the About section, questions 0-3 are About, 4 is Repair
					buttonControl.showAnswers (7, false);//Hide and reset answer buttons
					for (int i = 0; i < 7; i++) {
						TestText [i].color = Color.white;
					}
					if (selected == StringPool.TAnswers [modelNum, qNumber]) {//checks if the selected answer is the same as the answer key array
						scores [modelNum]++;//Add to the score, and display a correct message in green
						question.color = Color.green;
						question.text = "Correct.";
					audioplay=GameObject.Find("New Test Holder").GetComponent<AudioSource>();
					audioplay.PlayOneShot(correct);

					} else {
						question.color = Color.red;//display a incorrrect message and the correct answer in red
						question.text = "Incorrect. The correct answer is:\n" + StringPool.TFacts [StringPool.TAnswers [modelNum, qNumber], modelNum, qNumber];
						audioplay = GameObject.Find ("New Test Holder").GetComponent<AudioSource> ();
						audioplay.PlayOneShot (wrong);
					}
					qNumber++;//Advance the test and indicate no answer is selected
					selected = 12;
			} else {//Check if the test is in the Repair section
				buttonControl.interactAnswers (false);
				for (int i = 0; i < repairAnswer.Length; i++) {//Check the first character of all the user's answers against the first character of the key, the step numbers should be the same
					Debug.Log (" Submitted answer: " + repairAnswer [i].Substring (0, 1) + " Answer key: " + StringPool.RFacts [modelNum] [i].Substring (0, 1));
					if (repairAnswer [i].Substring (0, 1).Equals (StringPool.RFacts [modelNum] [i].Substring (0, 1))) {
						scores [modelNum]++;
						TestText [i].color = Color.green;
						Debug.Log ("You rock");

					} else {
						TestText [i].color = Color.red;
						allcorrect = false; 
						if (wrongFlag == false) {
							wrongFlag = true;
							audioplay = GameObject.Find ("New Test Holder").GetComponent<AudioSource> ();
							audioplay.PlayOneShot (wrong);
						}

						


					}
					if (allcorrect == true) {
						if (correctFlag == false) {
							correctFlag = true;
							audioplay = GameObject.Find ("New Test Holder").GetComponent<AudioSource> ();
							audioplay.PlayOneShot (pass);
							Debug.Log ("play correct 2");
						}
					}
				}
				qNumber++;
				selected = 12;

			}

		} else {//Is a new question or results screen being loaded
			
			if (qNumber < 4) {//Is it an About question
				question.color = Color.white;
				question.text = StringPool.TFacts [0, modelNum, qNumber];
				for (int i = 0; i < 4; i++) {
					TestText [i].text = StringPool.TFacts [i + 1, modelNum, qNumber];
				}
				if (qNumber == 3) {
					buttonControl.showAnswers (2, true);
				} else {
					buttonControl.showAnswers (4, true);
				}

			} else if (qNumber < 5) {//Is it a Repair question

				question.color = Color.white;
				question.text = "Select and use arrows to reorder the steps.";

				if (moved == false) {
					StepOrder ();
				}


				buttonControl.showAnswers (repairAnswer.Length, true);
				buttonControl.downOn (true);
				buttonControl.upOn (true);

			} else {//Is it a results screen
				moved = false;
				buttonControl.showAnswers (7, false);
				buttonControl.interactAnswers (true);
				buttonControl.nextOn (false);
				buttonControl.downOn (false);
				buttonControl.upOn (false);

				if (scores[modelNum] == 4 + repairAnswer.Length) {
					question.color = Color.green;
					question.text = "You Passed!\nYour score is " + scores[modelNum] + "/" + (4 + repairAnswer.Length) + ".";
					Glow [modelNum].material = DimGreenMaterial;
					uiController.particlesystem.Play ();
					audioplay=GameObject.Find("New Test Holder").GetComponent<AudioSource>();
					audioplay.PlayOneShot(fireworks);

					switch (modelNum){
					case 1:
						batteryScore = scoreHolder;
						break;
					case 2:
						cdScore = scoreHolder;
						break;
					case 3:
						fanScore = scoreHolder;
						break;
					case 4:
						hdScore = scoreHolder;
						break;
					case 5:
						ramScore = scoreHolder;
						break;
					case 6:
						wifiScore = scoreHolder;
						break;

					}
					if ((hdScore == 7) && (cdScore == 8) && (ramScore == 9) && (batteryScore == 8) && (wifiScore == 9) && (fanScore == 10)) {
						uiController.fireworksBool = true;
					}

				} else {
					question.color = Color.red;
					question.text = "You Failed!\nYour score is " + scores[modelNum] + "/" + (4 + repairAnswer.Length) + ".\nYou need to retake the test.";
					Glow [modelNum].material = DimRedMaterial;
					audioplay=GameObject.Find("New Test Holder").GetComponent<AudioSource>();
					//audioplay.PlayOneShot(wrong);
					switch (modelNum){
					case 1:
						batteryScore = scoreHolder;
						break;
					case 2:
						cdScore = scoreHolder;
						break;
					case 3:
						fanScore = scoreHolder;
						break;
					case 4:
						hdScore = scoreHolder;
						break;
					case 5:
						ramScore = scoreHolder;
						break;
					case 6:
						wifiScore = scoreHolder;
						break;

					}
				}
			}
		}
	}

	public void StepOrder(){
		string temp;
		int rand;
		int rand2;
		moved = true;
			for (int i = 0; i < repairAnswer.Length; i++) {
				rand = Random.Range (i, repairAnswer.Length);
				rand2 = Random.Range (2, repairAnswer.Length);

				Debug.Log (rand2);
				temp = repairAnswer [i];
				repairAnswer [i] = repairAnswer [rand2];
				repairAnswer [rand2] = temp;


		}
		for (int i = 0; i < repairAnswer.Length; i++) {
			TestText [i].text = repairAnswer [i].Substring (3);
		}


}


	public void dim(int TCMenu) {
		for (int i = 1; i < Glow.Length; i++) {

			if (i == TCMenu){
					switch (i) {
					case 1:
						if (batteryScore == 4 + StringPool.RFacts [i].Length) {
							Glow [i].material = GreenMaterial;
						} else if (batteryScore == -1) {
							Glow [i].material = BlueMaterial;
						} else {
							Glow [i].material = RedMaterial;

						}
						break;
					case 2:
						if (cdScore == 4 + StringPool.RFacts [i].Length) {
							Glow [i].material = GreenMaterial;
						} else if (cdScore == -1) {
							Glow [i].material = BlueMaterial;
						} else {
							Glow [i].material = RedMaterial;

						}
						break;
					case 3:
						if (fanScore == 4 + StringPool.RFacts [i].Length) {
							Glow [i].material = GreenMaterial;
						} else if (fanScore == -1) {
							Glow [i].material = BlueMaterial;
						} else {
							Glow [i].material = RedMaterial;

						}
						break;
					case 4:
						if (hdScore == 4 + StringPool.RFacts [i].Length) {
							Glow [i].material = GreenMaterial;
						} else if (hdScore == -1) {
							Glow [i].material = BlueMaterial;
						} else {
							Glow [i].material = RedMaterial;

						}
						break;
					case 5:
						if (ramScore == 4 + StringPool.RFacts [i].Length) {
							Glow [i].material = GreenMaterial;
						} else if (ramScore == -1) {
							Glow [i].material = BlueMaterial;
						} else {
							Glow [i].material = RedMaterial;

						}
						break;
					case 6:
						if (wifiScore == 4 + StringPool.RFacts [i].Length) {
							Glow [i].material = GreenMaterial;
						} else if (wifiScore == -1) {
							Glow [i].material = BlueMaterial;
						} else {
							Glow [i].material = RedMaterial;

						}
						break;
					}
				}
			if (i != TCMenu){
			switch (i){
			case 1:
				if (batteryScore == 4 + StringPool.RFacts [i].Length) {
					Glow [i].material = DimGreenMaterial;
				} else if (batteryScore == -1) {
					Glow [i].material = DimBlueMaterial;
				} else {
					Glow [i].material = DimRedMaterial;

				}
						break;
			case 2:
				if (cdScore == 4 + StringPool.RFacts [i].Length) {
					Glow [i].material = DimGreenMaterial;
				} else if (cdScore == -1) {
					Glow [i].material = DimBlueMaterial;
				} else {
					Glow [i].material = DimRedMaterial;

				}
				break;
			case 3:
				if (fanScore == 4 + StringPool.RFacts [i].Length) {
					Glow [i].material = DimGreenMaterial;
				} else if (fanScore == -1) {
					Glow [i].material = DimBlueMaterial;
				} else {
					Glow [i].material = DimRedMaterial;

				}
				break;
			case 4:
				if (hdScore == 4 + StringPool.RFacts [i].Length) {
					Glow [i].material = DimGreenMaterial;
				} else if (hdScore == -1) {
					Glow [i].material = DimBlueMaterial;
				} else {
					Glow [i].material = DimRedMaterial;

				}
				break;
			case 5:
				if (ramScore == 4 + StringPool.RFacts [i].Length) {
					Glow [i].material = DimGreenMaterial;
				} else if (ramScore == -1) {
					Glow [i].material = DimBlueMaterial;
				} else {
					Glow [i].material = DimRedMaterial;

				}
				break;
			case 6:
				if (wifiScore == 4 + StringPool.RFacts [i].Length) {
					Glow [i].material = DimGreenMaterial;
				} else if (wifiScore == -1) {
					Glow [i].material = DimBlueMaterial;
				} else {
					Glow [i].material = DimRedMaterial;

				}
				break;
			}
					}

		}
	}

	public void undim() {//would go under back button

		for (int i = 1; i < Glow.Length; i++) {

				switch (i) {
				case 1:
					if (batteryScore == 4 + StringPool.RFacts [i].Length) {
						Glow [i].material = DimGreenMaterial;
					} else if (batteryScore == -1) {
						Glow [i].material = DimBlueMaterial;
					} else {
						Glow [i].material = DimRedMaterial;
					Debug.Log ("dim red on battery");
					}
					break;
				case 2:
					if (cdScore == 4 + StringPool.RFacts [i].Length) {
						Glow [i].material = DimGreenMaterial;
					} else if (cdScore == -1) {
						Glow [i].material = DimBlueMaterial;
					} else {
						Glow [i].material = DimRedMaterial;

					}
					break;
				case 3:
					if (fanScore == 4 + StringPool.RFacts [i].Length) {
						Glow [i].material = DimGreenMaterial;
					} else if (fanScore == -1) {
						Glow [i].material = DimBlueMaterial;
					} else {
						Glow [i].material = DimRedMaterial;

					}
					break;
				case 4:
					if (hdScore == 4 + StringPool.RFacts [i].Length) {
						Glow [i].material = DimGreenMaterial;
					} else if (hdScore == -1) {
						Glow [i].material = DimBlueMaterial;
					} else {
						Glow [i].material = DimRedMaterial;

					}
					break;
				case 5:
					if (ramScore == 4 + StringPool.RFacts [i].Length) {
						Glow [i].material = DimGreenMaterial;
					} else if (ramScore == -1) {
						Glow [i].material = DimBlueMaterial;
					} else {
						Glow [i].material = DimRedMaterial;

					}
					break;
				case 6:
					if (wifiScore == 4 + StringPool.RFacts [i].Length) {
						Glow [i].material = DimGreenMaterial;
					} else if (wifiScore == -1) {
						Glow [i].material = DimBlueMaterial;
					} else {
						Glow [i].material = DimRedMaterial;

					}
					break;
				}



			}
		}

		}

