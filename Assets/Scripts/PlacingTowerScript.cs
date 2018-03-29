using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingTowerScript : MonoBehaviour {

	/// <summary>
	/// Handles the raycast for placing towers. (That floating red placeholder tower)
	/// It also checks when player presses right click (cancel tower placement) or left click (BuildingManager.Placebuilding will be called).
	/// </summary>

	//Raycast stuff
	private Vector3 pos;
	private Ray ray;
	public static RaycastHit hit;
	private bool cast = false;
	private Transform trans;

	//More raycast stuff
	void FixedUpdate () {
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		cast = Physics.Raycast (ray, out hit);
	}

	void Update () {
		//If player presses right mouse button, placeholder tower will be deleted.
		if(Input.GetMouseButtonDown(1))
		{
			Destroy (gameObject);
			ButtonScript.placed = false;
			ButtonScript.showTower = false;
			ButtonScript.clicked = false;
		}

		//If player presses left mouse button, BuildingManager.PlaceBuilding will be called.
		if (Input.GetMouseButtonDown (0) && GamePlay.gameActive)
			BuildingManager.PlaceBuilding (ButtonScript.selectedID+1, new Vector3 (hit.point.x, 0, hit.point.z));
		
		if(cast) {
			pos = new Vector3(hit.point.x, 0, hit.point.z);
		} else {
			pos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 0, Camera.main.ScreenToWorldPoint(Input.mousePosition).z);
		}
		gameObject.transform.position = pos;
	}


}
