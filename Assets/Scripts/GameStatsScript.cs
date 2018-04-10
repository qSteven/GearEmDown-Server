using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsScript : MonoBehaviour {

	/// <summary>
	/// This script displays the current stats like base health, money, kills and game status.
	/// </summary>

	public static Text stats;
	public static Text gamePlayStats;
	public static int baseHealth;
	public static int money;
	public static int kills = 0;

	public Text statsText;
	public Text gamePlayStatsText;
	public int startMoneyAmount;
	 
	void Awake() {
		stats = statsText;
		gamePlayStats = gamePlayStatsText;
		money = startMoneyAmount;
	}

	// Use this for initialization
	void Start () {
		baseHealth = BaseScript.health;
		UpdateStats ();
	}

	public static void UpdateBaseHealth(int currentHealth){
		baseHealth = currentHealth;
		UpdateStats ();
	}

	public static void UpdateMoney(){
		UpdateStats ();
	}

	public static void UpdateKills(){
		kills++;
		UpdateStats ();
	}

	public static void UpdateStats(){
		stats.text = ("Base Health: " + baseHealth + "\nMoney: " + money + "\nKills: " + kills);
		gamePlayStats.text = ("Game active: " + GamePlay.gameActive + "\nRound active: " + GamePlay.roundActive + "\nRound: " + GamePlay.round + "\nWave: " + Round.CurrentWaveNumber);
		Connection.SendToAll ("2|1;" + Round.CurrentWaveNumber);
		Connection.SendToAll ("2|2;" + money);
		Connection.SendToAll ("2|3;" + kills);
		Connection.SendToAll ("2|4;" + baseHealth);
		Connection.SendToAll ("2|5;" + (kills*1337));
	}
}
