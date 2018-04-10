using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeOrBuff : MonoBehaviour {

	//MS: V.10.0 API for upgrading / buffing a tower.
	//propID says which tower is spoken to
	//type says which stat at the tower will be edited
	//lvl says the amount of how the stat above will change
	//isBuff says if it is a buff or an upgrade.
	//When isBuff is true it will store the buffed amount in "Building.buffAmount", because this amount is needed when removing the buff again.
	public static void Buff(int propID, int type, int lvl, bool isBuff)
	{
		for (int i = 0; i < BuildingManager.towersInScene.Count; i++) {
			if (BuildingManager.towersInScene [i] != null) {
				if (BuildingManager.towersInScene [i].GetComponent<Building> ().id == propID) {
					if (BuildingManager.towersInScene [i].CompareTag ("TowerBuff")) {
						if ((BuildingManager.towersInScene [i].GetComponent<TowerBuffScript> ().type == type) || (BuildingManager.towersInScene [i].GetComponent<TowerBuffScript> ().type == -1)) {
							BuildingManager.towersInScene [i].GetComponent<TowerBuffScript> ().InitBuff (type);
						}
					} else if (type == 0 && BuildingManager.towersInScene [i].GetComponent<Building> ().damageCanBeBuffed) {
						BuildingManager.towersInScene [i].GetComponent<Building> ().damage += BuildingManager.towersInScene [i].GetComponent<Building> ().damageLevel [lvl];
						if (isBuff)
							BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type] += BuildingManager.towersInScene [i].GetComponent<Building> ().damageLevel [lvl];
						Debug.Log (propID + " ;;; " + isBuff + " ;;; " + type + " ;;; " + lvl);
						Connection.SendToAll ("8|3;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().damage);
						Debug.Log ("8|3;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().damage);
					} else if (type == 1 && BuildingManager.towersInScene [i].GetComponent<Building> ().fireRateCanBeBuffed) {
						BuildingManager.towersInScene [i].GetComponent<Building> ().fireRate -= BuildingManager.towersInScene [i].GetComponent<Building> ().fireRateLevel [lvl];
						if (isBuff)
							BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type] += BuildingManager.towersInScene [i].GetComponent<Building> ().fireRateLevel [lvl];
						Connection.SendToAll ("8|4;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().fireRate);
						Debug.Log ("8|4;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().fireRate);
					} else if (type == 2) {
						BuildingManager.towersInScene [i].GetComponent<SphereCollider> ().radius += BuildingManager.towersInScene [i].GetComponent<Building> ().radiusLevel [lvl];
						if (isBuff)
							BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type] += BuildingManager.towersInScene [i].GetComponent<Building> ().radiusLevel [lvl];
						Connection.SendToAll ("8|5;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<SphereCollider> ().radius);
						Debug.Log ("8|5;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<SphereCollider> ().radius);
					} else if (type == 3) {
						BuildingManager.towersInScene [i].GetComponent<TowerSlowScript> ().UpdateSlow (BuildingManager.towersInScene [i].GetComponent<Building> ().slowMultiplierLevel [lvl]);
						Connection.SendToAll ("8|6;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().slowMultiplier);
						Debug.Log ("8|6;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().slowMultiplier);
					}
					break;
				}
			}
		}
	}

	//Removing a buff, when the buff tower was sold. Goes only for buffs, you can't remove upgrades.
	//But if you want to: You need another array like the buffAmount array which saves the upgrade amounts.
	public static void RemoveBuff(int propID, int type) {
		for (int i = 0; i < BuildingManager.towersInScene.Count; i++) {
			if (BuildingManager.towersInScene [i] != null) {
				if (BuildingManager.towersInScene [i].GetComponent<Building> ().id == propID) {
					if (type == 0) {
						BuildingManager.towersInScene [i].GetComponent<Building> ().damage -= (int)BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type];
						BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type] = 0;
						Connection.SendToAll ("8|3;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().damage);
						Debug.Log ("8|3;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().damage);
					} else if (type == 1) {
						BuildingManager.towersInScene [i].GetComponent<Building> ().fireRate += BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type];
						BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type] = 0;
						Connection.SendToAll ("8|4;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().fireRate);
						Debug.Log ("8|4;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<Building> ().fireRate);
					} else if (type == 2) {
						BuildingManager.towersInScene [i].GetComponent<SphereCollider> ().radius -= BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type];
						BuildingManager.towersInScene [i].GetComponent<Building> ().buffAmount [type] = 0;
						Connection.SendToAll ("8|5;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<SphereCollider> ().radius);
						Debug.Log ("8|5;" + propID + ";" + BuildingManager.towersInScene [i].GetComponent<SphereCollider> ().radius);
					}
					break;
				}
			}
		}
	}
}
