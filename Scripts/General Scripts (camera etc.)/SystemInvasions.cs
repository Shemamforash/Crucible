using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemInvasions : MonoBehaviour
{
	public GameObject invasionQuad;
	public HeroScriptParent hero;
	public List<SystemInvasionInfo> currentInvasions = new List<SystemInvasionInfo>();
	private TokenManagement management;

	void Start()
	{
		management = MasterScript.invasionGUI.tokenContainer.GetComponent<TokenManagement> ();
	}

	private float CalculateTotalTokenValue(List<TokenInfo> tokenList, string damageType)
	{
		float total = 0;
		
		for(int k = 0; k < tokenList.Count; ++k)
		{
			HeroScriptParent heroScript = tokenList[k].heroOwner.GetComponent<HeroScriptParent>();

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
			SystemDefence systemDefence = currentInvasions[i].system.GetComponent<SystemDefence>();
			int system = MasterScript.RefreshCurrentSystem(currentInvasions[i].system);

			if(MasterScript.invasionGUI.invasionScreen.activeInHierarchy == true && MasterScript.invasionGUI.system == system)
			{
				management.CacheInvasionInfo();
			}

			for(int j = 0; j < MasterScript.systemListConstructor.systemList[system].systemSize; ++j)
			{
				float assaultDamage = CalculateTotalTokenValue(currentInvasions[i].tokenAllocation[j].assaultTokenAllocation, "Assault");
				float auxiliaryDamage = CalculateTotalTokenValue(currentInvasions[i].tokenAllocation[j].auxiliaryTokenAllocation, "Auxiliary") - MasterScript.systemListConstructor.systemList[system].planetsInSystem[j].planetCurrentDefence / 10f;

				if(auxiliaryDamage < 0)
				{
					auxiliaryDamage = 0;
				}

				systemDefence.TakeDamage(assaultDamage/2, auxiliaryDamage/2, j);

				if(MasterScript.systemListConstructor.systemList [system].planetsInSystem [j].planetPopulation <= 0)
				{
					MasterScript.systemListConstructor.systemList [system].planetsInSystem [j].planetColonised = false;
					MasterScript.systemListConstructor.systemList [system].planetsInSystem [j].expansionPenaltyTimer = 0f;
					MasterScript.systemListConstructor.systemList [system].planetsInSystem [j].improvementsBuilt.Clear ();
					MasterScript.systemListConstructor.systemList [system].planetsInSystem [j].planetImprovementLevel = 0;
					MasterScript.systemListConstructor.systemList [system].planetsInSystem [j].planetPopulation = 0;
				}
			}

			CheckSystemStatus (system, currentInvasions[i].player);
		}
	}

	private void CheckSystemStatus(int system, string player) //Used to check if system has been defeated
	{
		int planetsDestroyed = 0; //Counter for number of planets destroyed

		for(int i = 0; i < MasterScript.systemListConstructor.systemList [system].systemSize; ++i) //For all planets in system
		{
			if(MasterScript.systemListConstructor.systemList [system].planetsInSystem [i].planetColonised == false) //If it has not been colonised
			{
				++planetsDestroyed; //Add it to destroyed counter
				continue;
			}
		}
		
		if(planetsDestroyed == MasterScript.systemListConstructor.systemList [system].systemSize) //If the number of destroyed planets is equal to the system size
		{
			bool captured = false;

			for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].permanentConnections.Count; ++i) //For all systems connected to this one
			{
				int sys = MasterScript.RefreshCurrentSystem(MasterScript.systemListConstructor.systemList[system].permanentConnections[i]); //Get the connections number

				if(MasterScript.systemListConstructor.systemList[sys].systemOwnedBy == player) //If it is owned by the player
				{
					OwnSystem(system); //Capture this system
					MasterScript.invasionGUI.openInvasionMenu = false; //Close the invasion screen
					captured = true; //Prevent the system from being destroyed
					break;
				}
			}

			if(captured == false) //If it has no friendly neighbours
			{
				DestroySystem(system); //Destroy the system
				MasterScript.invasionGUI.openInvasionMenu = false; //Close the invasion screen
			}
		}
	}
	
	private void DestroySystem(int system)
	{
		SystemDefence systemDefence = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();

		MasterScript.systemListConstructor.systemList [system].systemDefence = 0;
		MasterScript.systemListConstructor.systemList [system].systemOwnedBy = null;
	
		MasterScript.systemListConstructor.systemList [system].systemObject.renderer.material = MasterScript.turnInfoScript.emptyMaterial;

		systemDefence = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		systemDefence.underInvasion = false;
	}
	
	private void OwnSystem(int system)
	{
		MasterScript.systemListConstructor.systemList [system].systemOwnedBy = hero.heroOwnedBy;

		switch(hero.heroOwnedBy)
		{
		case "Humans":
			MasterScript.voronoiGenerator.voronoiCells[system].renderer.sharedMaterial = MasterScript.turnInfoScript.humansMaterial;
			break;
		case "Selkies":
			MasterScript.voronoiGenerator.voronoiCells[system].renderer.sharedMaterial = MasterScript.turnInfoScript.selkiesMaterial;
			break;
		case "Nereides":
			MasterScript.voronoiGenerator.voronoiCells[system].renderer.sharedMaterial = MasterScript.turnInfoScript.nereidesMaterial;
			break;
		default:
			MasterScript.voronoiGenerator.voronoiCells[system].renderer.sharedMaterial = MasterScript.turnInfoScript.emptyMaterial;
			break;
		}

		MasterScript.voronoiGenerator.voronoiCells[system].renderer.material.shader = Shader.Find("Transparent/Diffuse");

		SystemDefence systemDefence = MasterScript.systemListConstructor.systemList [system].systemObject.GetComponent<SystemDefence> ();
		
		systemDefence.underInvasion = false;
		
		for(int i = 0; i < MasterScript.systemListConstructor.systemList [system].systemSize; ++i)
		{
			for(int j = 0; j < MasterScript.systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.Count; ++j)
			{
				MasterScript.systemListConstructor.systemList [system].planetsInSystem[i].improvementsBuilt.Clear ();
			}
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
	public int originalPosition, originalHero;
	public Vector3 currentPosition;
	public GameObject heroOwner, originalParent, currentParent;
	public string name;	
	public List<GameObject> tokenPositions = new List<GameObject>();
}
