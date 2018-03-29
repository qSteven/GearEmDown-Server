using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
	//If a tower is placed, it will be added to this list.
	public static List<GameObject> towersInScene = new List<GameObject>();
	public static int[] prices;

	//bools for checking if a tower can be placed.
	public static bool inZone = false;
	public static bool enoughSpace = true;
	public static bool enoughMoney = false;

	public static int clientMsg;

	void Start()
	{
		prices = new int[ButtonScript.towers.Count];
		for (int i = 0; i < ButtonScript.towers.Count; i++) {
			prices [i] = ButtonScript.towers [i].GetComponent<Building> ().price;
		}
	}

	//MS: V.10.0 API for placing a tower.
	public static int PlaceBuilding(int prefabId, Vector3 position)
	{
		clientMsg = 2;
		//Checks if position is at the place, where a tower can be placed.
		foreach (GameObject item in GameObject.FindGameObjectsWithTag("TowerPlace"))
		{
			if (item.transform.GetComponent<Collider>().bounds.Contains(position))
			{
				inZone = true;
				//Debug.Log ("inZone true");
				break;
			}
		}

		//Checks if no other tower is in range of the position.
		if (towersInScene != null) {
			for (int i = 0; i < towersInScene.Count; i++) {
				if (towersInScene [i] != null) {
					if (Vector3.Distance (position, towersInScene [i].transform.position) < 1.5f) {
						enoughSpace = false;
						//Debug.Log ("enoughSpace false");
					}
				}
			}
		}

		//Checks if the player has enough money to buy this tower.
		if (!(GameStatsScript.money < prices[prefabId-1])) {
			enoughMoney = true;
			//Debug.Log ("enoughMoney true");
		}

		//If every criterion applies, tower will be placed at the position.
		//The new tower will be added to the towersInScene list and the price will be subtracted from the player's money.
		if (inZone && enoughSpace && enoughMoney) {
			//Debug.Log ("placed");
			inZone = false;
			enoughSpace = true;
			enoughMoney = false;
			GameObject tower = Instantiate(ButtonScript.towers [prefabId-1], position, Quaternion.identity) as GameObject;
			tower.GetComponent<Building> ().Init ();
			towersInScene.Add (tower);
			//Debug.Log (position.x + ":::" + position.y + ":::" + position.z);
			clientMsg = 1;
			GameStatsScript.money -= prices [prefabId-1];
			GameStatsScript.UpdateStats ();
			if (!tower.CompareTag ("TowerBuff")) {
				for (int i = 0; i < towersInScene.Count; i++) {
					if (towersInScene [i] != null) {
						if (towersInScene [i].CompareTag ("TowerBuff")) {
							towersInScene [i].GetComponent<TowerBuffScript> ().UpdateTowerList ();
						}
					}
				}
			}
			return clientMsg;
		}
		//If not every criterion applies, return false.
		else {
			Debug.Log("not placed");
			Debug.Log("in zone: " + inZone + " ; enough space: " + enoughSpace + " ; enough money: " + enoughMoney);
			Debug.Log("Position: " + position.x + ":::" + position.y + ":::" + position.z);
			if (!inZone) clientMsg = 3;
			if (!enoughSpace) clientMsg = 4;
			if (!enoughMoney) clientMsg = 5;
			inZone = false;
			enoughSpace = true;
			enoughMoney = false;
			return clientMsg;
		}
	}

	//When a tower is being sold, he will be removed from the towersInScene list and 50% of his price will be added to the player's money.
	public static void SellBuilding(int propId){
		for(int i = 0; i < towersInScene.Count; i++){
			if (towersInScene [i] != null) {
				if (propId == towersInScene [i].GetComponent<Building> ().id) {
					Connection.SendToAll ((int)ProtocolDef.SellTower + "|" + towersInScene [i].GetComponent<Building> ().id);
					Debug.Log ((int)ProtocolDef.SellTower + "|" + towersInScene [i].GetComponent<Building> ().id);
					GameStatsScript.money += (int)(towersInScene [i].GetComponent<Building> ().price / 2);
					GameStatsScript.UpdateStats ();
					Destroy (towersInScene [i]);
					break;
				}
			}
		}
		//If a buff tower was sold, every other buff tower should update his list in case they are able buff a tower which was buffed by the sold buff tower.
		//It's not as confusing as it sounds m8, my english is just bad.
		for (int i = 0; i < towersInScene.Count; i++) {
			if (towersInScene [i] != null) {
				if (towersInScene [i].CompareTag ("TowerBuff")) {
					towersInScene [i].GetComponent<TowerBuffScript> ().UpdateTowerList ();
				}
			}
		}
	}

}
