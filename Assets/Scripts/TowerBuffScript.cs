using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuffScript : MonoBehaviour
{
	/// <summary>
	/// MS: V.10.0
	/// Mechanics of the buff tower:
	///
	/// When you place this tower it's doing nothing at first.
	/// It has to be initialised with a type first.
	/// The type says, which stat of other towers the buff tower is increasing.
	/// One buff tower can only have one type and can not be changed anymore.
	/// When you initialise a buff tower with a type, it will automatically upgrade himself to level 1 (which costs money).
	/// You can upgrade the buff tower to level 2, so it buffes more.
	///
	/// If another tower was added to the buff tower's range while the buff tower was already been initialised, the new tower is getting buffed automatically.
	/// The buff tower connects with one tower, as long as they both exist.
	/// When one of them is being sold, like for example the buff tower, the normal tower automatically connects with another buff tower, if he is in range.
	/// </summary>

	//Says if a buff tower has already been initialised with a type.
	public bool chosen = false;

	//Will be set true everytime a buff tower buffes another tower (again).
	public bool buffTower = false;

	//type 0 = damage;
	//type 1 = fire rate;
	//type 2 = radius;
	public int type = -1;
	public int level = -1;

	//How much upgrading the buff tower costs.
	public int[] levelCosts = new int[2];

	//Can't upgrade it more than the maxLevel.
	public int maxLevel = 2;

	//towerList saves other towers in the buff tower's range
	//towersBuffed saves other towers which are already buffed by this tower
	private List<GameObject> towerList = new List<GameObject>();
	public List<GameObject> towersBuffed = new List<GameObject>();

	// Update is called once per frame
	void Update ()
	{
		if (buffTower) {
			buffTower = false;
			BuffTower ();
		}
	}
		
	//API for choosing a type for the buff tower and / or leveling it up.
	//Call this method for initialising the buff tower with a type (which automatically upgrades the tower to level 1) or when you just upgrade it.
	public void InitBuff(int upgradeType) {
		if (level + 1 < maxLevel) {
			if (GameStatsScript.money >= levelCosts [level + 1]) {
				if (!chosen) {
					chosen = true;
					type = upgradeType;
					if (type == 0) {
						Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building> ().id + ";2;-1");
						Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building> ().id + ";3;-1");
					}
					else if (type == 1) {
						Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building> ().id + ";1;-1");
						Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building> ().id + ";3;-1");
					}
					else if (type == 2) {
						Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building> ().id + ";1;-1");
						Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building> ().id + ";2;-1");
					}
				}
				IncreaseLevel ();
			}
		}
	}

	//The following code is just for the GUI in Unity.
	//If you call this method here, you must have chosen the type of this buff tower in the inspector already.
	//The correct method is the API above.
	public void InitBuff() {
		if (level + 1 < maxLevel) {
			if (GameStatsScript.money >= levelCosts [level + 1]) {
				if (!chosen) {
					chosen = true;
				}
				IncreaseLevel ();
			}
		}
	}

	//Don't call this method in another method than the InitBuff method.
	void IncreaseLevel() {
		if (chosen && (level < maxLevel)) {
			level++;
			/*gameObject.GetComponent<Building>().price += levelCosts [level];
			GameStatsScript.money -= levelCosts [level];
			if (levelCosts.Length <= level) {
				Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building>().id + ";1;-1");
				Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building>().id + ";0;" + (gameObject.GetComponent<Building>().price / 2));
				Debug.Log ("8|2;" + gameObject.GetComponent<Building>().id + ";1;-1");
				Debug.Log ("8|2;" + gameObject.GetComponent<Building>().id + ";0;" + (gameObject.GetComponent<Building>().price / 2));
			} else {
				Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building>().id + ";" + type + ";" + (gameObject.GetComponent<Building>().price/2));
				Connection.SendToAll ("8|2;" + gameObject.GetComponent<Building>().id + ";0;" + (gameObject.GetComponent<Building>().price / 2));
				Debug.Log ("8|2;" + gameObject.GetComponent<Building>().id + ";" + type + ";" + (gameObject.GetComponent<Building>().price/2));
				Debug.Log ("8|2;" + gameObject.GetComponent<Building>().id + ";0;" + (gameObject.GetComponent<Building>().price / 2));
			}*/
			GameStatsScript.UpdateStats ();
			buffTower = true;
		}
	}

	public void BuffTower(){
		//Checks all towers in the scene
		foreach (GameObject tower in BuildingManager.towersInScene)
		{
			if (tower != null) {
				//Ignores buff towers
				if (!tower.CompareTag ("TowerBuff")) {
					//Only counts in towers in his range
					if (Vector3.Distance (transform.position, tower.transform.position) < 6.5f) {
						tower.gameObject.GetComponent<Building> ().inBuffRange = true;
						//If the tower isn't in towerList, tower is added to towerList.
						if (!towerList.Contains (tower)) {
							towerList.Add (tower);
							tower.GetComponent<Building> ().TowerRemovedEvent += OnTowerRemoved;
							for (int i = 0; i < towersBuffed.Count; i++) {
								if (towersBuffed [i] != null)
									towersBuffed [i].GetComponent<Building> ().dontBuffAgain [type] = true;
							}
							Debug.Log ("add");
						}
					}
				}
			}
		}

		//Buffing a tower for the first time:
		if(type != -1) for (int i = 0; i < towerList.Count; i++) {
				if (towerList [i] != null) {
					if ((towerList [i].gameObject.GetComponent<Building> ().buff [type] != 1) &&
						(towerList [i].GetComponent<Building> ().inBuffRange)) {
						towerList [i].gameObject.GetComponent<Building> ().buff [type] = 1;
						for (int k = 0; k < level + 1; k++) {
							Debug.Log ("buff first time");
							Debug.Log (towerList [i].GetComponent<Building> ().id);
							UpgradeOrBuff.Buff (towerList [i].GetComponent<Building> ().id, type, k, true);
						}
						towersBuffed.Add (towerList [i]);
						for (int j = 0; j < towersBuffed.Count; j++) {
							if (towersBuffed [j] != null) towersBuffed [j].GetComponent<Building> ().dontBuffAgain[type] = true;
						}
						Debug.DrawLine (new Vector3(transform.position.x, transform.position.y+3.09f, transform.position.z), new Vector3(towerList [i].transform.position.x, towerList [i].transform.position.y+1.8f, towerList [i].transform.position.z), Color.blue, 10000);
					}
				}
			}

		for (int j = 0; j < towersBuffed.Count; j++) {
			if (towersBuffed [j] != null) {
				//Buffing a tower again, which was already buffed by this buff tower:
				if ((towersBuffed [j].GetComponent<Building> ().maxBuff[type] > towersBuffed [j].GetComponent<Building> ().buffAmount [type])
					&& (towersBuffed [j].gameObject.GetComponent<Building> ().buff [type] == 1)
					&& !(towersBuffed [j].gameObject.GetComponent<Building> ().dontBuffAgain[type])) {
					UpgradeOrBuff.Buff (towersBuffed [j].GetComponent<Building> ().id, type, level, true);
					for (int k = 0; k < towersBuffed.Count; k++) {
						if (towersBuffed [k] != null) towersBuffed [k].GetComponent<Building> ().dontBuffAgain[type] = false;
					}
				}
			}
		}
		for (int j = 0; j < towersBuffed.Count; j++) towersBuffed [j].GetComponent<Building> ().dontBuffAgain[type] = false;
	}

	public void UpdateTowerList(){
		//Checks all towers in the scene
		foreach (GameObject tower in BuildingManager.towersInScene) {
			if (tower != null) {
				//Ignores buff towers
				if (!tower.CompareTag ("TowerBuff")) {
					//Only counts in towers in his range
					if (Vector3.Distance (transform.position, tower.transform.position) < 6.5f) {
						tower.gameObject.GetComponent<Building> ().inBuffRange = true;
						//If the tower isn't in towerList, tower is added to towerList.
						if (!towerList.Contains (tower)) {
							towerList.Add (tower);
							tower.GetComponent<Building> ().TowerRemovedEvent += OnTowerRemoved;
							for (int i = 0; i < towersBuffed.Count; i++) {
								if (towersBuffed [i] != null)
									towersBuffed [i].GetComponent<Building> ().dontBuffAgain [type] = true;
							}
							buffTower = true;
							//Debug.Log ("add");
						}
					}
				}
			}
		}
	}

	//Event: When a tower in the scene was removed, the buff tower checks if that tower was in range: If yes, he removes the tower from his list.
	void OnTowerRemoved(Building tower)
	{
		towerList.Remove (tower.gameObject);
		towersBuffed.Remove (tower.gameObject);
	}

	//When this tower is sold, he "releases" all his buffed towers, so that they can be buffed by another buff tower.
	void OnDestroy()
	{
		BuildingManager.towersInScene.Remove(gameObject);
		if (towerList != null) {
			for (int i = 0; i < towerList.Count; i++) {
				towerList [i].GetComponent<Building> ().TowerRemovedEvent -= OnTowerRemoved;
				if(type != -1) towerList [i].gameObject.GetComponent<Building> ().buff [type] = 0;
				towerList [i].GetComponent<Building> ().inBuffRange = false;
				UpgradeOrBuff.RemoveBuff (towerList [i].GetComponent<Building> ().id, type);
			}
		}
		//And then he tells other buff towers to update their buffs, so that a tower is buffed automatically by another buff tower when the old buff tower was removed.
		for (int i = 0; i < BuildingManager.towersInScene.Count; i++) {
			if (BuildingManager.towersInScene [i] != null) {
				if (BuildingManager.towersInScene [i].CompareTag ("TowerBuff")) {
					BuildingManager.towersInScene [i].GetComponent<TowerBuffScript> ().buffTower = true;
				}
			}
		}
	}
}
