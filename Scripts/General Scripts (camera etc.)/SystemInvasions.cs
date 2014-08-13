using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemInvasions : MasterScript
{
	public GameObject invasionQuad;
	public HeroScriptParent hero;
	public List<SystemInvasionInfo> currentInvasions = new List<SystemInvasionInfo>();
	private TokenManagement management;

	void Start()
	{
		management = invasionGUI.tokenContainer.GetComponent<TokenManagement> ();
	}

	private float CalculateTotalTokenValue(List<TokenInfo> tokenList, string damageType)
	{
		float total = 0;
		
		for(int k = 0; k < tokenList.Count; ++k)
		{
			heroScript = tokenList[k].heroOwner.GetComponent<HeroScriptParent>();

			float damageTotal = 0;

			if(damageType == "Assault")
			{
				damageTotal = heroScript.assaultDamage;
			}
			if(damageType == "Auxiliary")
			{
				damageTotal = heroScript.auxiliaryDamage;
			}
			
			total += damageTotal / heroScript.assaultTokens;
		}

		return total;
	}

	public void UpdateInvasions()
	{
		for(int i = 0; i < currentInvasions.Count; ++i)
		{
			systemDefence = currentInvasions[i].system.GetComponent<SystemDefence>();
			int system = RefreshCurrentSystem(currentInvasions[i].system);

			if(invasionGUI.invasionScreen.activeInHierarchy == true && invasionGUI.system == system)
			{
				management.CacheInvasionInfo();
			}

			for(int j = 0; j < systemListConstructor.systemList[system].systemSize; ++j)
			{
				float assaultDamage = CalculateTotalTokenValue(currentInvasions[i].tokenAllocation[j].assaultTokenAllocation, "Assault");
				float auxiliaryDamage = CalculateTotalTokenValue(currentInvasions[i].tokenAllocation[j].auxiliaryTokenAllocation, "Auxiliary") - systemListConstructor.systemList[system].planetsInSystem[j].planetCurrentDefence / 10f;

				systemDefence.TakeDamage(assaultDamage/2, auxiliaryDamage/2, j);

				if(systemListConstructor.systemList [system].planetsInSystem [j].planetPopulation <= 0)
				{
					systemListConstructor.systemList [system].planetsInSystem [j].planetColonised = false;
					systemListConstructor.systemList [system].planetsInSystem [j].expansionPenaltyTimer = 0f;
					systemListConstructor.systemList [system].planetsInSystem [j].improvementsBuilt.Clear ();
					systemListConstructor.systemList [system].planetsInSystem [j].planetImprovementLevel = 0;
					systemListConstructor.systemList [system].planetsInSystem [j].planetPopulation = 0;
				}
			}

			CheckSystemStatus (system, currentInvasions[i].player);
		}
	}

	private void CheckSystemStatus(int system, string player) //Used to check if system has been defeated
	{
		int planetsDestroyed = 0; //Counter for number of planets destroyed

		for(int i = 0; i < systemListConstructor.systemList [system].systemSize; ++i) //For all planets in system
		{
			if(systemListConstructor.systemList [system].planetsInSystem [i].planetColonised == false) //If it has not been colonised
			{
				++planetsDestroyed; //Add it to destroyed counter
				continue;
			}
		}
		
		if(planetsDestroyed == systemListConstructor.systemList [system].systemSize) //If the number of destroyed planets is equal to the system size
		{
			bool captured = false;

			for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i) //For all systems connected to this one
			{
				int sys = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]); //Get the connections number

				if(systemListConstructor.systemList[sys].systemOwnedBy == player) //If it is owned by the player
				{
					OwnSystem(system); //Capture this system
					invasionGUI.openInvasionMenu = false; //Close the invasion screen
					captured = true; //Prevent the system from being destroyed
					break;
				}
			}

			if(captured == false) //If it has no friendly neighbours
			{
				DestroySystem(system); //Destroy the system
				invasionGUI.openInvasionMenu = false; //Close the invasion screen
			}
		}
	}
	
	private void DestroySystem(int system)
	{
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		systemListConstructor.systemList [system].systemDefence = 0;
		systemListConstructor.systemList [system].systemOwnedBy = null;
		
		turnInfoScript = GameObject.Find ("ScriptsContainer").GetComponent<TurnInfo> ();
		
		//systemListConstructor.systemList [system].systemObject.renderer.material = turnInfoScript.emptyMaterial;

		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		systemDefence.underInvasion = false;
	}
	
	private void OwnSystem(int system)
	{
		systemListConstructor.systemList [system].systemOwnedBy = hero.heroOwnedBy;

		switch(hero.heroOwnedBy)
		{
		case "Humans":
			voronoiGenerator.voronoiCells[system].renderer.material = turnInfoScript.humansMaterial;
			break;
		case "Selkies":
			voronoiGenerator.voronoiCells[system].renderer.material = turnInfoScript.selkiesMaterial;
			break;
		case "Nereides":
			voronoiGenerator.voronoiCells[system].renderer.material = turnInfoScript.nereidesMaterial;
			break;
		default:
			voronoiGenerator.voronoiCells[system].renderer.material = turnInfoScript.emptyMaterial;
			break;
		}

		voronoiGenerator.voronoiCells[system].renderer.material.shader = Shader.Find("Transparent/Diffuse");

		improvementsBasic = systemListConstructor.systemList [system].systemObject.GetComponent<ImprovementsBasic> ();
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		
		systemDefence.underInvasion = false;
		
		for(int i = 0; i < systemListConstructor.systemList [system].systemSize; ++i)
		{
			for(int j = 0; j < systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.Count; ++j)
			{
				systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.Clear ();
			}
		}
	}
	
	public void StartSystemInvasion(int system)
	{
		hero.isInvading = true;
		
		hero.invasionObject = (GameObject)Instantiate (invasionQuad, systemListConstructor.systemList[system].systemObject.transform.position, 
		                                          systemListConstructor.systemList[system].systemObject.transform.rotation);
		
		systemListConstructor.systemList [system].enemyHero = gameObject;
		
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		
		systemDefence.underInvasion = true;
	}
	
	public void ContinueInvasion(int system)
	{		
		systemDefence = systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		hero.currentHealth -= systemListConstructor.systemList [system].systemOffence / (hero.currentHealth * hero.classModifier);
		systemListConstructor.systemList [system].systemDefence -= hero.assaultDamage;

		DiplomaticPosition temp = diplomacyScript.ReturnDiplomaticRelation (hero.heroOwnedBy, systemListConstructor.systemList[system].systemOwnedBy);
		temp.stateCounter -= 1;
		
		if(systemListConstructor.systemList [system].systemDefence <= 0)
		{
			systemListConstructor.systemList [system].systemDefence = 0;
			systemDefence.canEnter = true;
			Destroy(hero.invasionObject);
		}
	}
}

public class SystemInvasionInfo
{
	public GameObject system;
	public string player;
	public List<PlanetInvasionInfo> tokenAllocation = new List<PlanetInvasionInfo> ();
}

public class PlanetInvasionInfo
{
	public List<TokenInfo> assaultTokenAllocation = new List<TokenInfo>();
	public List<TokenInfo> auxiliaryTokenAllocation = new List<TokenInfo>();
	public List<TokenInfo> defenceTokenAllocation = new List<TokenInfo>();
}

public class TokenInfo
{
	public Vector3 originalPosition, currentPosition;
	public GameObject heroOwner, originalParent, currentParent;
	public string name;
}
