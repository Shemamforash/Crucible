using UnityEngine;
using System.Collections;

public class EnemyOne : AIBasicParent
{	
	public void RaceStart(string thisRace)
	{		
		isPlayer = false;

		playerRace = thisRace;

		PickRace ();

		turnInfoScript.systemsInPlay++;

		GameObject home = GameObject.Find (homeSystem);

		for(int i = 0; i < systemListConstructor.systemList.Count; ++i)
		{
			if(systemListConstructor.systemList[i].systemObject == home)
			{
				systemListConstructor.systemList[i].systemOwnedBy = playerRace;

				//voronoiGenerator.voronoiCells[i].renderer.material = materialInUse;
				//voronoiGenerator.voronoiCells[i].renderer.material.shader = Shader.Find("Transparent/Diffuse");

				for(int j = 0; j < systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(systemListConstructor.systemList[i].planetsInSystem[j].planetType == homePlanetType)
					{
						systemListConstructor.systemList[i].planetsInSystem[j].planetColonised = true;
						break;
					}
				}

				break;
			}
		}
	}
}

