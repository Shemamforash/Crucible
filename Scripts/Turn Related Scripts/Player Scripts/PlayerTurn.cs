using UnityEngine;
using System.Collections;

public class PlayerTurn : TurnInfo
{
	public GameObject tempObject;
	public bool isOkToColonise, systemHasBeenColonised, checkFirstContact = true;

	void Update()
	{
		if(Input.GetMouseButtonDown(0) && cameraFunctionsScript.selectedSystem != null) //Assigns scripts to selected system.
		{			
			tempObject = cameraFunctionsScript.selectedSystem;
			
			if(tempObject != null && tempObject.tag == "StarSystem")
			{
				systemSIMData = tempObject.GetComponent<SystemSIMData>();
				improvementsBasic = tempObject.GetComponent<ImprovementsBasic>();
			}
		}
		
		cameraFunctionsScript.CentreCamera(); //Checks if camera needs centreing
	}

	public void FindSystem(int system) //This function is used to check if the highlighted system can be colonised, and if it can, to colonise it
	{				
		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{			
			int j = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);
			
			if(systemListConstructor.systemList[j].systemOwnedBy == playerTurnScript.playerRace)
			{
				isOkToColonise = true;
			}
			
			else
			{
				continue;
			}
		}

		if(systemSIMData.guardedBy == "" || systemSIMData.guardedBy == playerTurnScript.playerRace)
		{
			if(isOkToColonise == true && wealth >= 10.0f)
			{
				systemSIMData = systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

				if(checkFirstContact == true)
				{
					for(int i = 0; i < diplomacyScript.relationsList.Count; ++i)
					{
						checkFirstContact = false;
						
						if(diplomacyScript.relationsList[i].firstContact == false)
						{
							checkFirstContact = true;
						}
					}

					for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
					{
						int j = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);

						for(int k = 0; k < diplomacyScript.relationsList.Count; ++k)
						{
							if(diplomacyScript.relationsList[k].playerOne.playerRace == systemListConstructor.systemList[system].systemOwnedBy)
							{
								if(diplomacyScript.relationsList[k].playerTwo.playerRace == systemListConstructor.systemList[j].systemOwnedBy)
								{
									if(diplomacyScript.relationsList[k].firstContact == false)
									{
										diplomacyScript.relationsList[k].firstContact = true;
									}
								}
							}
							if(diplomacyScript.relationsList[k].playerTwo.playerRace == systemListConstructor.systemList[system].systemOwnedBy)
							{
								if(diplomacyScript.relationsList[k].playerOne.playerRace == systemListConstructor.systemList[j].systemOwnedBy)
								{
									if(diplomacyScript.relationsList[k].firstContact == false)
									{
										diplomacyScript.relationsList[k].firstContact = true;
									}
								}
							}
						}
					}
				}
		
				systemSIMData.guardedBy = null;

				systemListConstructor.systemList[system].systemOwnedBy = playerRace;

				//voronoiGenerator.voronoiCells[system].renderer.material = materialInUse; //TODO
				//voronoiGenerator.voronoiCells[system].renderer.material.shader = Shader.Find("Transparent/Diffuse");
				
				playerTurnScript.wealth -= 10.0f;

				++turnInfoScript.systemsInPlay;

				++systemsColonisedThisTurn;
				
				cameraFunctionsScript.coloniseMenu = false;
				
				isOkToColonise = false;

				systemHasBeenColonised = true;
			}
		}
	}

	public void StartTurn()
	{
		isPlayer = true;

		PickRace ();

		cameraFunctionsScript.selectedSystem = GameObject.Find (homeSystem); //Set the selected system
		cameraFunctionsScript.selectedSystemNumber = RefreshCurrentSystem (cameraFunctionsScript.selectedSystem);
		turnInfoScript.systemsInPlay++;
		
		int i = RefreshCurrentSystem(cameraFunctionsScript.selectedSystem);

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

		Vector3 temp = systemListConstructor.systemList [i].systemObject.transform.position;

		systemPopup.mainCamera.transform.position = new Vector3(temp.x, temp.y, -45f);
	}
}
