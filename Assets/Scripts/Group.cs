using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group
{
	/// <summary>
	/// MS: V.10.0 Creating groups, which will be added to waves.
	/// </summary>

	//Delay between groups.
	public float groupDelay = 5f;

	//How many enemies of which kind are in a group.
	//enemyAmount = nArrow
	//enemyAmount2 = Steamtank
	//enemyAmount3 = AirShip
	public int enemyAmount;
	public int enemyAmount2;
	public int enemyAmount3;

	//Normal constructor.
	public Group(int e1Amount, int e2Amount, int e3Amount)
	{
		enemyAmount = e1Amount;
		enemyAmount2 = e2Amount;
		enemyAmount3 = e3Amount;
	}

	//Will be called when delay between groups has to be adjusted
	//(like when the group is the last group in the wave, the group delay will be overwritten with the wave delay).
	public Group(int e1Amount, int e2Amount, int e3Amount, float delay)
	{
		enemyAmount = e1Amount;
		enemyAmount2 = e2Amount;
		enemyAmount3 = e3Amount;
		groupDelay = delay;
	}
}
