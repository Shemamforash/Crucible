using UnityEngine;
using System.Collections;

public class EnemyOne : AIBasicParent
{	
	public void RaceStart(string thisRace)
	{		
		isPlayer = false;

		playerRace = thisRace;

		PickRace ();

		MasterScript.turnInfoScript.systemsInPlay++;

		GameObject home = GameObject.Find (homeSystem);

		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i)
		{
			if(MasterScript.systemListConstructor.systemList[i].systemObject == home)
			{
				MasterScript.systemListConstructor.systemList[i].systemOwnedBy = playerRace;

				//voronoiGenerator.voronoiCells[i].renderer.material = materialInUse; //TODO
				//voronoiGenerator.voronoiCells[i].renderer.material.shader = Shader.Find("Transparent/Diffuse");

				for(int j = 0; j < MasterScript.systemListConstructor.systemList[i].systemSize; ++j)
				{
					if(MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetType == homePlanetType)
					{
						MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetColonised = true;
						break;
					}
				}

				break;
			}
		}
	}
}

