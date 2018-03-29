using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
	/// <summary>
	/// MS: V.10.0 This script is pretty cool, probably the best of what an useless and stupid trainee managed to build. :D
	/// 
	/// You give a maximum number of enemies for one wave to this constructor and it splits the number in groups, which have a random size.
	/// It also calculates how many enemies of which kind are in these groups.
	/// There are 40% normal, 30% tank and 30% air enemies of the maximum enemy amount in one wave and these will be added randomly to those groups.
	/// </summary>

	//Delay between waves.
	public float waveDelay = 20f;

	//List with groups in this wave.
	public List<Group> waveGroups = new List<Group>();

	//Which group is spoken to.
	public int groupPointer;

	public Wave(List<Group> groups)
	{
		waveGroups = groups;
	}

	public Wave(int maxEnemyAmount)
	{
		//How many of which enemy will appear in this wave will be calculated here:
		int e1Amount = (int)Mathf.Floor(maxEnemyAmount * 0.4f);
		int e2Amount = (int)Mathf.Floor(maxEnemyAmount * 0.3f);
		int e3Amount = (int)Mathf.Floor(maxEnemyAmount * 0.3f);
		e1Amount += maxEnemyAmount - (e1Amount + e2Amount + e3Amount);

		int e1InGroup = 0;
		int e2InGroup = 0;
		int e3InGroup = 0;

		//Split maxEnemyAmount in groups.
		//In every loop, one group will be filled.
		//Loop repeats as long until every enemy was assigned.
		while (maxEnemyAmount > 0)
		{
			//A group can have a size of 5 - 10 enemies (random).
			int groupLength = Random.Range(5, 11);

			if (maxEnemyAmount - groupLength >= 0) maxEnemyAmount -= groupLength;
			else
			{
				groupLength = maxEnemyAmount;
				maxEnemyAmount = 0;
			}

			//Fill this group with random enemies.
			//Enemies will be taken from the "Amount"-variables and then added to the "InGroup"-variables until one group is filled.
			for (int i = 0; i < groupLength; i++)
			{
				bool filled = false;
				while(!filled)
				{
					int rnd = Random.Range(1, 4);
					if (rnd == 1 && e1Amount > 0)
					{
						e1InGroup++;
						e1Amount--;
						filled = true;
					}
					else if (rnd == 2 && e2Amount > 0)
					{
						e2InGroup++;
						e2Amount--;
						filled = true;
					}
					else if (rnd == 3 && e3Amount > 0)
					{
						e3InGroup++;
						e3Amount--;
						filled = true;
					}
				}
			}
			//Add groups to waveGroups list.
			//If the group was the last group in the wave, the normal group delay will be overwritten with the delay between waves.
			if((e1Amount == 0) && (e2Amount == 0) && (e3Amount == 0)){
				waveGroups.Add(new Group(e1InGroup, e2InGroup, e3InGroup, waveDelay));
			}
			else{
				waveGroups.Add(new Group(e1InGroup, e2InGroup, e3InGroup));
			}
			e1InGroup = e2InGroup = e3InGroup = 0;
		}
	}

	//Gives back the next group in the waveGroup list.
	public Group NextGroup()
	{
		if (groupPointer >= waveGroups.Count) return null;
		Group ret = waveGroups[groupPointer];
		groupPointer++;
		return ret;
	}
}
