using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class Connection : WebSocketBehavior
{
	// Static Fields
	public static Dictionary<string, Connection> connectionList = new Dictionary<string, Connection>();
	public static WebSocketServiceHost service;
	private static Connection player;
	public static Connection Player
	{
		get { return player; }
		set
		{
			player = value;
			if (value == null) Debug.Log("No one is player right now.");
			else
			{
				Debug.Log("ID \"" + player.clientID + "\" is now player.");
				SendToAll((int)ProtocolDef.GamePhase + "|1;" + player.clientID);
			}
		}
	}

	// Properties
	public bool IsPlayer
	{ get { return object.ReferenceEquals(this, player); } }
	public string clientNickName;
	public int clientID;

	// Initialize client data and add to client list.
	private void InitializeClient()
	{
		// Find free Client ID.
		int potentialClientID = 1;
		int chaos = 100;
		bool found = false;
		while (chaos > 0)
		{
			chaos--;
			found = false;
			foreach (var con in connectionList)
			{
				if (con.Value.clientID == potentialClientID)
				{
					found = true;
					potentialClientID++;
					break;
				}
			}
			if (!found)
			{
				clientID = potentialClientID;
				break;
			}
		}
		if (chaos <= 0)
		{
			SendToAll((int)ProtocolDef.Error + "| CHAOS HAPPEND");
			throw new UnityException("CHAOS HAPPEND");
		}

		clientNickName = "Player_" + clientID;
		connectionList.Add(this.ID, this);
		Debug.Log("Client connected.\n" + ToString());
	}

	// Message Events
	protected override void OnOpen()
	{
		UnityMainThreadDispatcher.Instance().Enqueue (() =>
			{
				// Initialize
				InitializeClient();

				// Update client list
				SendClientListToAll();

				// Update player
				if (player != null) Send((int)ProtocolDef.GamePhase + "|1;" + player.clientID);

				// Tower positions
				if (BuildingManager.towersInScene != null) {
					for (int i = 0; i < BuildingManager.towersInScene.Count; i++) {
						if (BuildingManager.towersInScene [i] != null) {
							Send ("3|1;" + BuildingManager.towersInScene [i].GetComponent<Building> ().prefabID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().id + ";" + BuildingManager.towersInScene [i].transform.position.x + ";" + BuildingManager.towersInScene [i].transform.position.y + ";" + BuildingManager.towersInScene [i].transform.position.z);
							Debug.Log ("3|1;" + BuildingManager.towersInScene [i].GetComponent<Building> ().prefabID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().id + ";" + BuildingManager.towersInScene [i].transform.position.x + ";" + BuildingManager.towersInScene [i].transform.position.y + ";" + BuildingManager.towersInScene [i].transform.position.z);
						}
					}
				}

				// Enemy positions
				if (EnemyScript.EnemiesActive != null) {
					for (int i = 0; i < EnemyScript.EnemiesActive.Length; i++) {
						if (EnemyScript.EnemiesActive [i] != null) {
							Send ("6|1;" + EnemyScript.EnemiesActive [i].GetComponent<EnemyScript> ().prefabID + ";" + EnemyScript.EnemiesActive [i].GetComponent<EnemyScript> ().id);
							Debug.Log ("6|1;" + EnemyScript.EnemiesActive [i].GetComponent<EnemyScript> ().prefabID + ";" + EnemyScript.EnemiesActive [i].GetComponent<EnemyScript> ().id);
						}
					}
				}

				// Game Stats
				Send("2|1;" + Round.CurrentWaveNumber);
				Send("2|2;" + GameStatsScript.money);
				Send("2|3;" + GameStatsScript.kills);
				Send("2|4;" + GameStatsScript.baseHealth);
				Send("2|5;" + GameStatsScript.kills);

				// Tower and upgrade prices
				foreach (GameObject tower in ButtonScript.towers)
				{
					if(tower != null)
					{
						Building towerBuilding = tower.GetComponent<Building>();
						Send("8|1;" + towerBuilding.prefabID + ";" + towerBuilding.price);
						if(towerBuilding.possibleUpgrades[0]) Send("8|7;" + towerBuilding.prefabID + ";1;" + towerBuilding.upgradeCostsDmg[0]);
						if(towerBuilding.possibleUpgrades[1]) Send("8|7;" + towerBuilding.prefabID + ";2;" + towerBuilding.upgradeCostsFire[0]);
						if(towerBuilding.possibleUpgrades[2]) Send("8|7;" + towerBuilding.prefabID + ";3;" + towerBuilding.upgradeCostsRad[0]);
						if(towerBuilding.possibleUpgrades[3]) Send("8|7;" + towerBuilding.prefabID + ";4;" + towerBuilding.upgradeCostsSlow[0]);

						if(towerBuilding.damage != 0) Send("8|8;" + towerBuilding.prefabID + ";" + towerBuilding.damage);
						if(towerBuilding.fireRate != 0) Send("8|9;" + towerBuilding.prefabID + ";" + towerBuilding.fireRate);
						Send("8|10;" + towerBuilding.prefabID + ";" + tower.GetComponent<SphereCollider>().radius);
						if(towerBuilding.slowMultiplier != 0) Send("8|11;" + towerBuilding.prefabID + ";" + towerBuilding.slowMultiplier);
					}
				}
			});
	}
	protected override void OnClose(CloseEventArgs e)
	{
		connectionList.Remove(this.ID);
		Debug.Log("Client disconnected.\n" + this.ToString());
		SendClientListToAll();
		if (object.ReferenceEquals(this, player)) ProtocolHandler.instance.OnEndGame();
	}
	protected override void OnMessage(MessageEventArgs e)
	{
		try
		{
			UnityMainThreadDispatcher.Instance().Enqueue(() =>
			{
				string[] data = e.Data.Split(new char[]{'|'}, 2);
				ProtocolDef pr = (ProtocolDef)Convert.ToInt32(data[0]);
				if (IsPlayer) ProtocolHandler.instance.OnPlayerMesssage(this, pr, data[1]);
				else ProtocolHandler.instance.OnClientMesssage(this, pr, data[1]);
			});
		}
		catch(UnityException err)
		{
			Send((int)ProtocolDef.Error + "|ERROR: " + err.Message);
		}
		catch(Exception err)
		{
			Send((int)ProtocolDef.Error + "|Protocol ERROR: " + err.Message);
		}
	}

	// Client List
	public static void SendClientListToAll()
	{
		foreach (var connectionClient in connectionList.Values)
		{
			string list = connectionClient.clientID + "," + connectionClient.clientNickName;
			foreach (var connectionOther in connectionList.Values)
			{
				if (object.ReferenceEquals(connectionOther, connectionClient)) continue;
				list += ";" + connectionOther.clientID + "," + connectionOther.clientNickName;
			}
			connectionClient.Send((int)ProtocolDef.ClientList + "|" + list);
		}
	}
	public void SendClientList()
	{
		string list = this.clientID + "," + clientNickName;
		foreach (var connectionOther in connectionList.Values)
		{
			if (object.ReferenceEquals(connectionOther, this)) continue;
			list += ";" + connectionOther.clientID + "," + connectionOther.clientNickName;
		}
		Send((int)ProtocolDef.ClientList + "|" + list);
	}

	// Send Messages
	public void SendBack(string data)
	{
		Send(data);
	}
	public static void SendToPlayer(string data)
	{
		player.Send(data);
	}
	public static void SendToAll(string data)
	{
		// TODO Sessions ist leer
		service.Sessions.Broadcast(data);
	}

	public override string ToString()
	{
		return string.Format("Websocket ID: {0}\nClient ID: {1}\nNick name: {2}", this.ID, clientID, clientNickName);
	}
}