using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroScriptParent : MonoBehaviour 
{
	//This is the basic hero level, with general effects
	public GameObject heroLocation, invasionObject;
	public int system;
	public int lH2Spent, antiSpent, radioSpent, blueCSpent, assaultTokens = 2, auxiliaryTokens = 2, defenceTokens = 2; 
	public string heroOwnedBy, heroType;
	public bool isInvading = false, isBusy;
	public float heroAge, classModifier, maxHealth, currentHealth, assaultDamage, auxiliaryDamage, invasionStrength, movementSpeed, defence;
	public int aiInvadeTarget = -1, aiProtectTarget = -1;
	public float healthMod = 1f, resourceMod = 1f, movementMod = 1f, cloakMod = 1f, assaultMod = 1f, cooldownMod = 1f, auxiliaryMod = 1f;
	private HeroShip heroShip;
	private HeroMovement heroMovement;
	
	void Start()
	{
		heroAge = Time.time;
	
		heroShip = gameObject.GetComponent<HeroShip> ();
		heroMovement = gameObject.GetComponent<HeroMovement> ();

		system = MasterScript.RefreshCurrentSystem (heroLocation);

		movementSpeed = 1;
		currentHealth = 100;
	}

	void Update()
	{
		system = MasterScript.RefreshCurrentSystem (heroLocation);

		if(heroMovement.heroIsMoving == false)
		{
			gameObject.transform.position = heroMovement.HeroPositionAroundStar(heroLocation);
		}
	}

	public DiplomaticPosition FindDiplomaticConnection()
	{
		for(int i = 0; i < MasterScript.diplomacyScript.relationsList.Count; ++i)
		{
			if(heroOwnedBy == MasterScript.diplomacyScript.relationsList[i].playerOne.playerRace)
			{
				if(MasterScript.systemListConstructor.systemList[system].systemOwnedBy == MasterScript.diplomacyScript.relationsList[i].playerTwo.playerRace)
				{
					return MasterScript.diplomacyScript.relationsList[i];
				}
			}

			if(heroOwnedBy == MasterScript.diplomacyScript.relationsList[i].playerTwo.playerRace)
			{
				if(MasterScript.systemListConstructor.systemList[system].systemOwnedBy == MasterScript.diplomacyScript.relationsList[i].playerOne.playerRace)
				{
					return MasterScript.diplomacyScript.relationsList[i];
				}
			}
		}

		return null;
	}

	private void AIHeroFunctions()
	{
		int i = MasterScript.RefreshCurrentSystem(heroLocation);

		if(aiInvadeTarget != -1)
		{
			if(i == aiInvadeTarget && isInvading == false)
			{
				//TODO implement ai invasions
			}
		}
	}

	public void HeroEndTurnFunctions(TurnInfo thisPlayer)
	{
		heroShip = gameObject.GetComponent<HeroShip> ();
		heroShip.ShipAbilities (thisPlayer);

		if(thisPlayer.isPlayer == false)
		{
			AIHeroFunctions();
		}

		if(thisPlayer == MasterScript.playerTurnScript)
		{
			if(isInvading == true)
			{
				MasterScript.systemInvasion.hero = this;

				if(currentHealth < 0f)
				{
					currentHealth = 0f;
				}
				if(currentHealth > maxHealth)
				{
					currentHealth = maxHealth;
				}
			}

			if(isInvading == false && currentHealth != maxHealth)
			{
				currentHealth += maxHealth * 0.02f;

				if(currentHealth > maxHealth)
				{
					currentHealth = maxHealth;
				}
			}
		}
	}
}


