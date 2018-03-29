using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round
{
	/// <summary>
	/// MS: V.10.0 This script generates rounds in a certain scheme.
	/// </summary>

	//Will be given to the Wave constructor.
	public static int maxEnemies = 15;

	public float roundDelay;
	public int wavePointer;
	public Wave[] roundWaves = new Wave[5];

	//Saves the current wave number for the GUI and updates the client with it.
	private static int currentWaveNumber;
	public static int CurrentWaveNumber
	{
		get { return currentWaveNumber; }
		set
		{
			currentWaveNumber = value;
			// TODO : Client informieren
		}
	}

	//Generated Round.
	//A round has 5 waves.
	//A generated round spawns a wave with a number of enemies of maxEnemies and increases it every wave by a certain amount.
	public Round()
	{
		roundWaves = new Wave[]
		{
			new Wave (maxEnemies),
			new Wave (maxEnemies+2),
			new Wave (maxEnemies+4),
			new Wave (maxEnemies+6),
			new Wave (maxEnemies+16)
		};
	}

	//Custom Round
	//Round 1 is custom for having a more balanced game.
	//This custom round is initialised in EnemySpawnScript.
	public Round(Wave[] waves)
	{
		roundWaves = waves;
	}

	//Asks the wave class for the next group and returns it.
	public Group NextGroup()
	{
		if (wavePointer >= roundWaves.Length) return null;
		Group ret = roundWaves[wavePointer].NextGroup();
		if (ret == null)
		{
			Debug.Log ("Wave done");
			wavePointer++;
			if(wavePointer != 5) currentWaveNumber++;
			GameStatsScript.UpdateStats();
			return NextGroup();
		}
		return ret;
	}
}