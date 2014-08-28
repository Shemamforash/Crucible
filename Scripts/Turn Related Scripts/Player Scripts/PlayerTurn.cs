using UnityEngine;
using System.Collections;

public class PlayerTurn : TurnInfo
{
	public GameObject tempObject;
	public bool isOkToColonise, systemHasBeenColonised, checkFirstContact = true;
	private SystemSIMData systemSIMData;

	void Update()
	{
		if(Input.GetMouseButtonDown(0) && MasterScript.cameraFunctionsScript.selectedSystem != null) //Assigns scripts to selected system.
		{			
			tempObject = MasterScript.cameraFunctionsScript.selectedSystem;
			
			if(tempObject != null && tempObject.tag == "StarSystem")
			{
				systemSIMData = tempObject.GetComponent<SystemSIMData>();
			}
		}
		
		MasterScript.cameraFunctionsScript.CentreCamera(); //Checks if camera needs centreing
	}

	public void FindSystem(int system) //This function is used to check if the highlighted system can be colonised, and if it can, to colonise it
	{				
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{			
			int j = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[system].permanentConnections[i]);
			
			if(MasterScript.systemListConstructor.systemList[j].systemOwnedBy == MasterScript.playerTurnScript.playerRace)
			{
				isOkToColonise = true;
			}
			
			else
			{
				continue;
			}
		}

		if(systemSIMData.guardedBy == "" || systemSIMData.guardedBy == MasterScript.playerTurnScript.playerRace)
		{
			if(isOkToColonise == true && wealth >= 10.0f)
			{
				systemSIMData = MasterScript.systemListConstructor.systemList[system].systemObject.GetComponent<SystemSIMData>();

				if(checkFirstContact == true)
				{
					for(int i = 0; i < MasterScript.diplomacyScript.relationsList.Count; ++i)
					{
						checkFirstContact = false;
						
						if(MasterScript.diplomacyScript.relationsList[i].firstContact == false)
						{
							checkFirstContact = true;
						}
					}

					for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].permanentConnections.Count; ++i)
					{
						int j = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[system].permanentConnections[i]);

						for(int k = 0; k < MasterScript.diplomacyScript.relationsList.Count; ++k)
						{
							if(MasterScript.diplomacyScript.relationsList[k].playerOne.playerRace == MasterScript.systemListConstructor.systemList[system].systemOwnedBy)
							{
								if(MasterScript.diplomacyScript.relationsList[k].playerTwo.playerRace == MasterScript.systemListConstructor.systemList[j].systemOwnedBy)
								{
									if(MasterScript.diplomacyScript.relationsList[k].firstContact == false)
									{
										MasterScript.diplomacyScript.relationsList[k].firstContact = true;
									}
								}
							}
							if(MasterScript.diplomacyScript.relationsList[k].playerTwo.playerRace == MasterScript.systemListConstructor.systemList[system].systemOwnedBy)
							{
								if(MasterScript.diplomacyScript.relationsList[k].playerOne.playerRace == MasterScript.systemListConstructor.systemList[j].systemOwnedBy)
								{
									if(MasterScript.diplomacyScript.relationsList[k].firstContact == false)
									{
										MasterScript.diplomacyScript.relationsList[k].firstContact = true;
									}
								}
							}
						}
					}
				}
		
				systemSIMData.guardedBy = null;

				MasterScript.systemListConstructor.systemList[system].systemOwnedBy = playerRace;

				//voronoiGenerator.voronoiCells[system].renderer.material = materialInUse; //TODO
				//voronoiGenerator.voronoiCells[system].renderer.material.shader = Shader.Find("Transparent/Diffuse");
				
				MasterScript.playerTurnScript.wealth -= 10.0f;

				++MasterScript.turnInfoScript.systemsInPlay;

				++systemsColonisedThisTurn;
				
				MasterScript.cameraFunctionsScript.coloniseMenu = false;
				
				isOkToColonise = false;

				systemHasBeenColonised = true;
			}
		}
	}

	public void StartTurn()
	{
		isPlayer = true;

		PickRace ();

		MasterScript.cameraFunctionsScript.selectedSystem = GameObject.Find (homeSystem); //Set the selected system
		MasterScript.cameraFunctionsScript.selectedSystemNumber = MasterScript.RefreshCurrentSystem (MasterScript.cameraFunctionsScript.selectedSystem);
		MasterScript.turnInfoScript.systemsInPlay++;
		
		int i = MasterScript.RefreshCurrentSystem(MasterScript.cameraFunctionsScript.selectedSystem);

		MasterScript.systemListConstructor.systemList[i].systemOwnedBy = playerRace;

		//voronoiGenerator.voronoiCells[i].renderer.material = materialInUse;
		//voronoiGenerator.voronoiCells[i].renderer.material.shader = Shader.Find("Transparent/Diffuse");

		for(int j = 0; j < MasterScript.systemListConstructor.systemList[i].systemSize; ++j)
		{
			if(MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetType == homePlanetType)
			{
				MasterScript.systemListConstructor.systemList[i].planetsInSystem[j].planetColonised = true;
				break;
			}
		}

		Vector3 temp = MasterScript.systemListConstructor.systemList [i].systemObject.transform.position;

		MasterScript.systemPopup.mainCamera.transform.position = new Vector3(temp.x, temp.y, -45f);
	}
}
