using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScript : MonoBehaviour {

	public static int health;
	public int lifePoints;

	void Awake(){
		health = lifePoints;
	}

	//If enemy reaches base, base takes damage.
	//Meanwhile enemy gets destroyed.
	void OnTriggerEnter(Collider other){
		if (GamePlay.gameActive) {
			if (other.CompareTag ("Enemy")) {
				lifePoints -= other.gameObject.GetComponent<EnemyScript> ().damage;
				GameStatsScript.UpdateBaseHealth (lifePoints);
				Destroy (other.gameObject);

				if (lifePoints <= 0) {
					//UNCOMMENT IF YOU WANT THE GAME TO END AFTER THE BASE REACHED 0 HP
					GameOver();
				}
			}
		}
	}

	//Game stops: Player has no input anymore, he is only able to press the "okay"-button, which resets the game.
	void GameOver(){
		/*GamePlay.gameActive = false;
		GamePlay.roundActive = false;
		//show okay(reset)-button
		Debug.Log ("Base dead");
		GameStatsScript.UpdateStats ();*/
		ProtocolHandler.instance.OnEndGame ();
	}
}
