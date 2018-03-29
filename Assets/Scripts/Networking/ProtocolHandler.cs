using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProtocolDef
{
	GamePhase = 1,
	GameStats = 2,
	PlaceTower = 3,
	SellTower = 4,
	Upgrade = 5,
	Enemy = 6,
	PosSync = 7,
	Error = 101,
	ClientList = 102
}

public class ProtocolHandler : MonoBehaviour
{
	public static ProtocolHandler instance;

	void Awake()
	{
		instance = this;
	}

	public void OnPlayerMesssage(Connection con, ProtocolDef pr, string msg)
	{
		switch (pr)
		{
		case ProtocolDef.GamePhase:
			switch (msg)
			{
			// End Game
			case "2":
				if (Connection.Player != null) OnEndGame();
				break;

			// Next Round
			case "4":
				if (!GamePlay.roundActive && GamePlay.gameActive)
				{
					bool enemiesAlive = ButtonScript.CheckEnemies();
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
				break;
			}
			break;

		case ProtocolDef.PlaceTower:
			string[] data = msg.Split(new char[]{ ';' });

			// Nur für Linux Build!
			#if !UNITY_EDITOR
			data[1] = data[1].Replace('.', ',');
			data[2] = data[2].Replace('.', ',');
			#endif

			float posX, posZ;
			if (!float.TryParse(data[1], out posX) || !float.TryParse(data[2], out posZ))
			{
				throw new UnityException("Error while parsing Float. X: " + data[1] + "; Y: " + data[2]);
			}
			int clientMsg = BuildingManager.PlaceBuilding(int.Parse(data[0]), new Vector3(posX, 0f, posZ));
			if (clientMsg != 1)
			{
				Connection.SendToAll((int)ProtocolDef.PlaceTower + "|" + clientMsg);
			}
			break;

		case ProtocolDef.SellTower:
			BuildingManager.SellBuilding (int.Parse(msg));
			break;

		case ProtocolDef.Upgrade:
			string[] data3 = msg.Split (new char[]{ ';' });
			int error = 3;
			for(int i = 0; i < BuildingManager.towersInScene.Count; i++)
			{
				if (BuildingManager.towersInScene[i] != null)
				{
					//Debug.Log (BuildingManager.towersInScene [i].GetComponent<Building> ().id + " ;;; " + BuildingManager.towersInScene [i].GetComponent<Building> ().prefabID);
					if (BuildingManager.towersInScene[i].GetComponent<Building> ().id == int.Parse (data3 [0])) {
						if (int.Parse (data3 [1]) == 1) {
							if (BuildingManager.towersInScene [i].GetComponent<Building> ().possibleUpgrades [0]) {
								error = BuildingManager.towersInScene [i].GetComponent<Building> ().IncreaseDamage ();
							} else {
								error = 3;
							}
						} else if (int.Parse (data3 [1]) == 2) {
							if (BuildingManager.towersInScene [i].GetComponent<Building> ().possibleUpgrades [1]) {
								error = BuildingManager.towersInScene [i].GetComponent<Building> ().IncreaseFireRate ();
							} else {
								error = 3;
							}
						} else if (int.Parse (data3 [1]) == 3) {
							if (BuildingManager.towersInScene [i].GetComponent<Building> ().possibleUpgrades [2]) {
								error = BuildingManager.towersInScene [i].GetComponent<Building> ().IncreaseRadius ();
							} else {
								error = 3;
							}
						} else if (int.Parse (data3 [1]) == 4) {
							if (BuildingManager.towersInScene [i].GetComponent<Building> ().possibleUpgrades [3]) {
								error = BuildingManager.towersInScene [i].GetComponent<Building> ().IncreaseSlow ();
							} else {
								error = 3;
							}
						} else {
							error = 3;
						}
					}
				}
			}
			if (error == 1) {
				Connection.SendToAll ((int)ProtocolDef.Upgrade + "|" + error + ";" + data3 [0] + ";" + data3 [1]);
				//Debug.Log ((int)ProtocolDef.Upgrade + "|" + error + ";" + data3 [0] + ";" + data3 [1]);
			}
			else {
				Connection.SendToAll ((int)ProtocolDef.Upgrade + "|" + error);
				//Debug.Log ((int)ProtocolDef.Upgrade + "|" + error);
			}
			break;
		}
	}

	public void OnClientMesssage(Connection con, ProtocolDef pr, string msg)
	{
		switch (pr)
		{
		case ProtocolDef.GamePhase:
			switch (msg)
			{
			// Start Game
			case "1":
				if (Connection.Player == null) OnStartGame(con);
				else con.SendBack((int)ProtocolDef.GamePhase + "|3");
				break;
			}
			break;
		}
	}

	public void OnStartGame(Connection player)
	{
		Connection.Player = player;
		if (!GamePlay.gameActive) {
			GamePlay.gameActive = true;
			GamePlay.status = 0;
			Debug.Log ("start");
			GameStatsScript.UpdateStats ();
		}
	}

	public void OnEndGame()
	{
		Connection.SendToAll((int)ProtocolDef.GamePhase + "|2");
		Connection.Player = null;
		GamePlay.status = -2;
	}
}
