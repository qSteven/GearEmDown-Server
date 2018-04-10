using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

	/// <summary>
	/// This script is only for the Unity GUI and testing stuff.
	/// Just checks if player presses for example "sell" and then presses on a tower and so on.
	/// You can also upgrade towers or initialise a buff tower.
	/// </summary>

	void Update ()
	{
		if (ButtonScript.playerInput && GamePlay.gameActive) {
			if (Input.GetMouseButtonDown (0)) {
				Ray ray2 = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit2;
				if (Physics.Raycast (ray2, out hit2)) {
					if (hit2.transform.gameObject.tag.Contains ("Tower")) {
						Debug.Log ("treffer");
						if (ButtonScript.sell) {
							Debug.Log ("sell");
							BuildingManager.SellBuilding (hit2.transform.gameObject.GetComponent<Building> ().id);

							ButtonScript.count = 0;
							ButtonScript.ResetRaycast ();
							ButtonScript.HideTools ();

							ButtonScript.playerInput = false;
							ButtonScript.sell = false;
							ButtonScript.init = false;
						}
						else if (ButtonScript.init) {
							Debug.Log ("init");
							if (hit2.transform.gameObject.CompareTag ("TowerBuff")) {
								hit2.transform.gameObject.GetComponent<TowerBuffScript> ().InitBuff ();
							}

							ButtonScript.count = 0;
							ButtonScript.ResetRaycast ();
							ButtonScript.HideTools ();

							ButtonScript.playerInput = false;
							ButtonScript.init = false;
							ButtonScript.sell = false;
						}
					}
				}
			}
		}
				

		if (ButtonScript.upgrade) {
			if (ButtonScript.playerInput && GamePlay.gameActive) {
				if (ButtonScript.addDamage) {
					if (Input.GetMouseButtonDown (0)) {
						Ray ray3 = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit hit3;
						if (Physics.Raycast (ray3, out hit3)) {
							Debug.Log ("raycast");
							ButtonScript.addDamage = false;
							if (hit3.transform.gameObject.tag.Contains ("Tower")) {
								Debug.Log ("treffer");
								if (hit3.transform.gameObject.GetComponent<Building> ().possibleUpgrades [0]) {
									hit3.transform.gameObject.GetComponent<Building> ().IncreaseDamage ();
									Debug.Log ("dmg+");
								}
							}
						}
					}
				}
				else if (ButtonScript.addFireRate) {
					if (Input.GetMouseButtonDown (0)) {
						Ray ray3 = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit hit3;
						if (Physics.Raycast (ray3, out hit3)) {
							Debug.Log ("raycast");
							ButtonScript.addFireRate = false;
							if (hit3.transform.gameObject.tag.Contains ("Tower")) {
								Debug.Log ("treffer");
								if (hit3.transform.gameObject.GetComponent<Building> ().possibleUpgrades [1]) {
									hit3.transform.gameObject.GetComponent<Building> ().IncreaseFireRate ();
									Debug.Log ("fire+");
								}
							}
						}
					}
				}
				else if (ButtonScript.addRadius) {
					if (Input.GetMouseButtonDown (0)) {
						Ray ray3 = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit hit3;
						if (Physics.Raycast (ray3, out hit3)) {
							Debug.Log ("raycast");
							ButtonScript.addRadius = false;
							if (hit3.transform.gameObject.tag.Contains ("Tower")) {
								Debug.Log ("treffer");
								if (hit3.transform.gameObject.GetComponent<Building> ().possibleUpgrades [2]) {
									hit3.transform.gameObject.GetComponent<Building> ().IncreaseRadius ();
									Debug.Log ("rad+");
								}
							}
						}
					}
				}
				else if (ButtonScript.addSlow) {
					if (Input.GetMouseButtonDown (0)) {
						Ray ray3 = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit hit3;
						if (Physics.Raycast (ray3, out hit3)) {
							Debug.Log ("raycast");
							ButtonScript.addSlow = false;
							if (hit3.transform.gameObject.tag.Contains ("Tower")) {
								Debug.Log ("treffer");
								if (hit3.transform.gameObject.GetComponent<Building> ().possibleUpgrades [3]) {
									hit3.transform.gameObject.GetComponent<Building> ().IncreaseSlow ();
									Debug.Log ("slow+");
								}
							}
						}
					}
				}
						
			}
		}

	}
}