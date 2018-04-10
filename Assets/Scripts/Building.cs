using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Building : MonoBehaviour
{
	//Saves how much will be added with every upgrade / buff level.
	//Goes for upgrading AND buffing, both methods use the same stats!
	public int[] damageLevel;
	public int maxDamageBuff;

	public float[] fireRateLevel;
	public float maxFireRateBuff;

	public float[] radiusLevel;
	public float maxRadiusBuff;

	public float[] slowMultiplierLevel;
	public float maxSlowBuff;

	public float[] maxBuff = new float[4];

	//Basic stats, which will be manipulated due to upgrading and buffing.
	public float damage;
	public float fireRate;
	public float slowMultiplier;

	//More basic stats.
	public string buildingName;
	public int prefabID;
	public string description;
	// price will be increased when upgrading a tower (for calculating how much money the player will get back when selling this tower)
	public int price;

	//Sometimes you shouldn't be able to upgrade something at a certain tower. For example upgrading the fire rate at the tower which deals constant damage.
	//So I put bool values in this array which states if a stat can be upgraded or not (true = yes, false = no)
	//0 = damage
	//1 = fire rate
	//2 = radius
	//3 = slow multiplier
	public bool[] possibleUpgrades = new bool[4];

	//Same as above, but only important while buffing a tower. I didn't want an array again.
	public bool damageCanBeBuffed;
	public bool fireRateCanBeBuffed;

	//Costs for upgrading this tower in the mentioned stats:
	public int[] upgradeCostsDmg = new int[2];
	public int[] upgradeCostsFire = new int[2];
	public int[] upgradeCostsRad = new int[2];
	public int[] upgradeCostsSlow = new int[2];

	//Saves at which upgrade level the tower is. It can't exceed the maxLvl.
	public int maxLvl = 2;
	public int damageLvl = 0;
	public int fireRateLvl = 0;
	public int radiusLvl = 0;
	public int slowLvl = 0;

	//Stuff for the algorithm of the buff tower
	public bool inBuffRange = false;
	//public bool dontBuffAgain = true;
	public bool[] dontBuffAgain = new bool[3];

	//If a tower is buffed for example in damage, buff[0] will be set to 1.
	//Same goes for fire rate and radius.
	//Only for checking if a tower was already buffed in a certain stat, because buffs shouldn't stack.
	//type 0 = damage;
	//type 1 = fire rate;
	//type 2 = radius;
	public byte[] buff = new byte[3];

	public static byte idIncrement;
	public byte id;

	//Event
	public delegate void TowerRemoved(Building tower);
	public event TowerRemoved TowerRemovedEvent = delegate(Building tower){};

	//buffAmount stores amount of total buffs for one stat: necessary when removing buff
	//0 = damage
	//1 = fire rate
	//3 = radius
	public float[] buffAmount = new float[3];

	void Awake() {
		//Calculates the maximum amount per stat of how much can be buffed at this tower.
		for (int i = 0; i < maxLvl; i++) {
			maxDamageBuff += damageLevel[i];
			maxFireRateBuff += fireRateLevel[i];
			maxRadiusBuff += radiusLevel[i];
			maxSlowBuff += slowMultiplierLevel[i];
		}

		//And I also need an array for this :D
		maxBuff [0] = maxDamageBuff;
		maxBuff [1] = maxFireRateBuff;
		maxBuff [2] = maxRadiusBuff;
		maxBuff [3] = maxSlowBuff;
	}

	// Use this for initialization
	public void Init ()
	{
		idIncrement++;
		id = idIncrement;
		Connection.SendToAll("103|[Server] Building ID " + id);

		Connection.SendToAll ((int)ProtocolDef.PlaceTower + "|1;" + prefabID + ";" + id + ";" + transform.position.x + ";0;" + transform.position.z);
		//Debug.Log ((int)ProtocolDef.PlaceTower + "|1;" + prefabID + ";" + id + ";" + transform.position.x + ";0;" + transform.position.z);

		Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));

		//BuildingManager.towersInScene.Add(gameObject);
	}

	//The following 4 methods are for upgrading a tower.
	//It will be checked if a tower already reached the maximum level of upgrades.
	//If it doesn't, stats, level and price will be increased and the player spends money.
	public int IncreaseDamage() {
		if ((damageLvl < maxLvl)) {
			if (GameStatsScript.money >= upgradeCostsDmg [damageLvl]) {
				UpgradeOrBuff.Buff (id, 0, damageLvl, false);
				//Debug.Log (price + " 2/// " + (price / 2));
				price += upgradeCostsDmg [damageLvl];
				//Debug.Log (price + " 3/// " + (price / 2));
				GameStatsScript.money -= upgradeCostsDmg [damageLvl];
				GameStatsScript.UpdateStats ();
				damageLvl++;

					if (upgradeCostsDmg.Length <= damageLvl) {
						Connection.SendToAll ("8|2;" + id + ";1;-1");
						Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));
						//Debug.Log ("8|2;" + id + ";1;-1");
					} else {
						Connection.SendToAll ("8|2;" + id + ";1;" + upgradeCostsDmg [damageLvl]);
						Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));
						//Debug.Log (price + " 4/// " + (price / 2));
						//Debug.Log ("8|2;" + id + ";1;" + upgradeCostsDmg [damageLvl]);
						//Debug.Log ("8|2;" + id + ";0;" + (price / 2));
					}
					return 1;
			}
			else return 2;
		}
		else return 3;
	}

	public int IncreaseFireRate() {
		if ((fireRateLvl < maxLvl)) {
			if (GameStatsScript.money >= upgradeCostsFire [fireRateLvl]) {
				UpgradeOrBuff.Buff (id, 1, fireRateLvl, false);
				price += upgradeCostsFire [fireRateLvl];
				GameStatsScript.money -= upgradeCostsFire [fireRateLvl];
				GameStatsScript.UpdateStats ();
				fireRateLvl++;
				if (upgradeCostsFire.Length <= fireRateLvl) {
					Connection.SendToAll ("8|2;" + id + ";2;-1");
					Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));
					//Debug.Log ("8|2;" + id + ";2;-1");
				} else {
					Connection.SendToAll ("8|2;" + id + ";2;" + upgradeCostsFire [fireRateLvl]);
					Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));
					//Debug.Log ("8|2;" + id + ";2;" + upgradeCostsFire [fireRateLvl]);
					//Debug.Log ("8|2;" + id + ";0;" + (price / 2));
				}
				return 1;
			}
			else return 2;
		}
		else return 3;
	}

	public int IncreaseRadius() {
		if ((radiusLvl < maxLvl)) {
			if (GameStatsScript.money >= upgradeCostsRad [radiusLvl]) {
				UpgradeOrBuff.Buff (id, 2, radiusLvl, false);
				price += upgradeCostsRad [radiusLvl];
				GameStatsScript.money -= upgradeCostsRad [radiusLvl];
				GameStatsScript.UpdateStats ();
				radiusLvl++;
				if (upgradeCostsRad.Length <= radiusLvl) {
					Connection.SendToAll ("8|2;" + id + ";3;-1");
					Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));
					//Debug.Log ("8|2;" + id + ";3;-1");
				}
				else{
					Connection.SendToAll ("8|2;" + id + ";3;" +  upgradeCostsRad [radiusLvl]);
					Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));
					//Debug.Log ("8|2;" + id + ";3;" +  upgradeCostsRad [radiusLvl]);
					//Debug.Log ("8|2;" + id + ";0;" + (price / 2));
				}
				return 1;
			}
			else return 2;
		}
		else return 3;
	}

	public int IncreaseSlow() {
		if ((slowLvl < maxLvl)) {
			if (GameStatsScript.money >= upgradeCostsSlow [slowLvl]) {
				UpgradeOrBuff.Buff (id, 3, slowLvl, false);
				price += upgradeCostsSlow [slowLvl];
				GameStatsScript.money -= upgradeCostsSlow [slowLvl];
				GameStatsScript.UpdateStats ();
				slowLvl++;
				if (upgradeCostsSlow.Length <= slowLvl) {
					Connection.SendToAll ("8|2;" + id + ";4;-1");
					Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));
					//Debug.Log ("8|2;" + id + ";4;-1");
				} else {
					Connection.SendToAll ("8|2;" + id + ";4;" + upgradeCostsSlow [slowLvl]);
					Connection.SendToAll ("8|2;" + id + ";0;" + (price / 2));
					//Debug.Log ("8|2;" + id + ";4;" + upgradeCostsSlow [slowLvl]);
					//Debug.Log ("8|2;" + id + ";0;" + (price / 2));
				}
				return 1;
			}
			else return 2;
		}
		else return 3;
	}

	//Event
	void OnDestroy()
	{
		TowerRemovedEvent(this);
		if (Connection.connectionList.Count > 0) Connection.SendToAll((int)ProtocolDef.SellTower + "|" + id);
	}
}