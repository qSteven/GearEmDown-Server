using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

	/// <summary>
	/// "OH GOD!!!"
	/// Yeah
	/// So much unnecessary stuff
	/// It's only for my GUI so you can ignore this shit.
	/// But better don't delete this script, it has too many references to other scripts, because otherwise I wouldn't be able to test this game.
	/// </summary>

	public GameObject tower1Button;
	public GameObject tower2Button;
	public GameObject tower3Button;
	public GameObject tower4Button;
	public GameObject tower5Button;

	public GameObject towerTarget;
	public GameObject towerSplash;
	public GameObject towerSlow;
	public GameObject towerBuff;
	public GameObject towerTarget2;

	//THIS LIST IS NEEDED IN ANOTHER SCRIPT AT THE MOMENT, SORRY, TOO LAZY TO CHANGE THIS
	//DO NOT DELETE
	public static List<GameObject> towers;
	//DO NOT DELETE
	public List<GameObject> types;

	public GameObject openManagerButton;
	public static GameObject upgradeButtonStatic;
	public static GameObject sellButtonStatic;
	public static GameObject initButtonStatic;

	public static GameObject increaseDamageStatic;
	public static GameObject increaseFireRateStatic;
	public static GameObject increaseRadiusStatic;
	public static GameObject increaseSlowStatic;

	public GameObject increaseDamage;
	public GameObject increaseFireRate;
	public GameObject increaseRadius;
	public GameObject increaseSlow;

	public GameObject upgradeButton;
	public GameObject sellButton;
	public GameObject initButton;

	public static bool clicked = false;
	public static bool showTower = false;
	public static bool placed = false;
	private bool tower1 = false;
	private bool tower2 = false;
	private bool tower3 = false;
	private bool tower4 = false;
	private bool tower5 = false;
	public static int selectedID;
	Vector3 pos;

	public static int count = 0;
	public static int count2 = 0;

	public static bool playerInput = false;
	public static bool upgrade = false;
	public static bool sell = false;
	public static bool init = false;

	public static bool addDamage = false;
	public static bool addFireRate = false;
	public static bool addRadius = false;
	public static bool addSlow = false;

	public GameObject startGameButton;
	public GameObject startRoundButton;
	public GameObject gameOverButton;

	public static GameObject startGameButtonStatic;
	public static GameObject startRoundButtonStatic;
	public static GameObject gameOverButtonStatic;

	void Awake() {
		towers = types;
	}

	// Use this for initialization
	void Start () {
		Button startGameBtn = startGameButton.GetComponent<Button> ();
		startGameBtn.onClick.AddListener (StartGame);

		Button startRoundBtn = startRoundButton.GetComponent<Button> ();
		startRoundBtn.onClick.AddListener (StartRound);

		Button gameOverBtn = gameOverButton.GetComponent<Button> ();
		gameOverBtn.onClick.AddListener (GameOver);

		upgradeButtonStatic = upgradeButton;
		sellButtonStatic = sellButton;
		initButtonStatic = initButton;

		increaseDamageStatic = increaseDamage;
		increaseFireRateStatic = increaseFireRate;
		increaseRadiusStatic = increaseRadius;
		increaseSlowStatic = increaseSlow;

		startGameButtonStatic = startGameButton;
		startRoundButtonStatic = startRoundButton;
		gameOverButtonStatic = gameOverButton;

		Button btn1 = tower1Button.GetComponent<Button> ();
		btn1.onClick.AddListener (Tower1Clicked);

		Button btn2 = tower2Button.GetComponent<Button> ();
		btn2.onClick.AddListener (Tower2Clicked);

		Button btn3 = tower3Button.GetComponent<Button> ();
		btn3.onClick.AddListener (Tower3Clicked);

		Button btn4 = tower4Button.GetComponent<Button> ();
		btn4.onClick.AddListener (Tower4Clicked);

		Button btn5 = tower5Button.GetComponent<Button> ();
		btn5.onClick.AddListener (Tower5Clicked);

		Button openManagerBtn = openManagerButton.GetComponent<Button> ();
		openManagerBtn.onClick.AddListener (Manager);

		Button upgradeBtn = upgradeButtonStatic.GetComponent<Button> ();
		upgradeBtn.onClick.AddListener (Upgrade);

		Button sellBtn = sellButtonStatic.GetComponent<Button> ();
		sellBtn.onClick.AddListener (Sell);

		Button initBtn = initButtonStatic.GetComponent<Button> ();
		initBtn.onClick.AddListener (Init);

		Button increaseDamageBtn = increaseDamage.GetComponent<Button>();
		increaseDamageBtn.onClick.AddListener (IncreaseDamage);

		Button increaseFireRateBtn = increaseFireRate.GetComponent<Button>();
		increaseFireRateBtn.onClick.AddListener (IncreaseFireRate);

		Button increaseRadiusBtn = increaseRadius.GetComponent<Button>();
		increaseRadiusBtn.onClick.AddListener (IncreaseRadius);

		Button increaseSlowBtn = increaseSlow.GetComponent<Button>();
		increaseSlowBtn.onClick.AddListener (IncreaseSlow);

		HideTools ();
	}

	// Update is called once per frame
	void Update () {
		//If the player is about to place a tower, adjust raycast at the objects in the scene.
		if (playerInput) {
			GameObject.Find ("Ground").layer = 2;
			for (int i = 0; i < BuildingManager.towersInScene.Count; i++) {
				if(BuildingManager.towersInScene[i] != null) BuildingManager.towersInScene [i].layer = 0;
			}
		}
		if(!playerInput) ResetRaycast ();
	}

	void FixedUpdate () {
		if (GamePlay.gameActive && clicked && !showTower) {
			showTower = true;

			Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 5000f)) {
				pos=hit.point;
			} else {
				pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
			}
			if (!placed) {
				placed = true;
				if (tower1) {
					Instantiate (towerTarget, pos, Quaternion.identity);
					tower1 = false;
				}
				else if(tower2) {
					Instantiate (towerSplash, pos, Quaternion.identity);
					tower2 = false;
				}
				else if(tower3) {
					Instantiate (towerSlow, pos, Quaternion.identity);
					tower3 = false;
				}
				else if(tower4) {
					Instantiate (towerBuff, pos, Quaternion.identity);
					tower4 = false;
				}
				else if(tower5) {
					Instantiate (towerTarget2, pos, Quaternion.identity);
					tower5 = false;
				}

			}
		}
	}

	void Tower1Clicked(){
		if(!playerInput) {
			clicked = true;
			tower1 = true;
			selectedID = 0;
		}
	}

	void Tower2Clicked(){
		if (!playerInput) {
			clicked = true;
			tower2 = true;
			selectedID = 1;
		}
	}

	void Tower3Clicked(){
		if (!playerInput) {
			clicked = true;
			tower3 = true;
			selectedID = 2;
		}
	}

	void Tower4Clicked(){
		if (!playerInput) {
			clicked = true;
			tower4 = true;
			selectedID = 3;
		}
	}

	void Tower5Clicked(){
		if (!playerInput) {
			clicked = true;
			tower5 = true;
			selectedID = 4;
		}
	}

	void Manager(){
		if (count == 0) {
			count++;
			ShowTools ();
		} else if (count == 1) {
			count = 0;
			count2 = 0;
			playerInput = false;
			sell = false;
			HideTools ();
		}
	}

	void Upgrade() {
		if (count2 == 0) {
			count2++;
			upgrade = true;
			sell = false;
			ShowUpgrades ();
		} else if (count2 == 1) {
			count2 = 0;
			upgrade = false;
			HideUpgrades ();
		}
	}

	void Sell() {
		playerInput = true;
		sell = true;
		upgrade = false;
		init = false;
	}

	void Init() {
		playerInput = true;
		init = true;
		sell = false;
		upgrade = false;
	}

	void IncreaseDamage() {
		addDamage = true;
		playerInput = true;
	}

	void IncreaseFireRate() {
		playerInput = true;
		addFireRate = true;
	}

	void IncreaseRadius() {
		playerInput = true;
		addRadius = true;
	}

	void IncreaseSlow() {
		playerInput = true;
		addSlow = true;
	}

	public static void ShowUpgrades() {
		increaseDamageStatic.SetActive (true);
		increaseFireRateStatic.SetActive (true);
		increaseRadiusStatic.SetActive (true);
		increaseSlowStatic.SetActive (true);
	}

	public static void HideUpgrades() {
		increaseDamageStatic.SetActive (false);
		increaseFireRateStatic.SetActive (false);
		increaseRadiusStatic.SetActive (false);
		increaseSlowStatic.SetActive (false);
	}

	public static void ShowTools() {
		upgradeButtonStatic.SetActive (true);
		sellButtonStatic.SetActive (true);
		initButtonStatic.SetActive (true);
	} 

	public static void HideTools() {
		upgradeButtonStatic.SetActive (false);
		sellButtonStatic.SetActive (false);
		initButtonStatic.SetActive (false);

		increaseDamageStatic.SetActive (false);
		increaseFireRateStatic.SetActive (false);
		increaseRadiusStatic.SetActive (false);
		increaseSlowStatic.SetActive (false);
	}

	public static void ResetRaycast() {
		GameObject.Find ("Ground").layer = 0;
		for (int i = 0; i < BuildingManager.towersInScene.Count; i++) {
			if(BuildingManager.towersInScene[i] != null) BuildingManager.towersInScene [i].layer = 2;
		}
	}

	//START GAME
	public void StartGame(){
		if (!GamePlay.gameActive) {
			GamePlay.gameActive = true;
			GamePlay.status = 0;
			Debug.Log ("start");
			GameStatsScript.UpdateStats ();
		}
	}

	//START ROUND
	public void StartRound()
	{
		if (!GamePlay.roundActive && GamePlay.gameActive)
		{
			bool enemiesAlive = CheckEnemies();
			if (!enemiesAlive)
			{
				GamePlay.showNextRound = false;
				GamePlay.roundActive = true;
				GamePlay.round++;
				Round.CurrentWaveNumber++;
				EnemySpawnScript.instance.StartRound ();
				GameStatsScript.UpdateStats();
			}
		}
	}

	//RESET GAME
	public void GameOver(){
		GamePlay.status = -2;
	}

	//Checks, if all enemies are dead. Important for starting new rounds, because you can only start one, when there are no enemies left.
	public static bool CheckEnemies(){
		GameObject o = GameObject.FindWithTag("Enemy");
		if(o == null)
			return false;
		else
			return true;
	}
}
