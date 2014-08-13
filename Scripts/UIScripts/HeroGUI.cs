using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroGUI : MasterScript 
{
	public bool openHeroLevellingScreen;
	public GameObject heroObject, merchantQuad, invasionButton, embargoButton, promoteButton, buttonContainer, turnInfoBar, heroDetailsContainer, currentHero, guardButton;
	public UILabel[] heroHealth = new UILabel[3];
	public UILabel[] heroName = new UILabel[3];
	private RaycastHit hit;

	void Update()
	{
		hit = new RaycastHit();
		
		if(Input.GetMouseButtonDown(0)) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "Hero")
				{
					currentHero = hit.collider.gameObject;
					heroShip = currentHero.GetComponent<HeroShip>();
					heroScript = currentHero.GetComponent<HeroScriptParent>();

					if(heroScript.heroOwnedBy != playerTurnScript.playerRace)
					{
						currentHero = null;
					}
				}
			}
		}

		if(Input.GetMouseButtonDown (1) && currentHero != null)
		{
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit))
			{
				if(hit.collider.gameObject.tag == "StarSystem")
				{
					heroMovement = currentHero.GetComponent<HeroMovement> ();
					heroMovement.FindPath(heroScript.heroLocation, hit.collider.gameObject);
					if(heroScript.invasionObject != null)
					{
						Destroy (heroScript.invasionObject);
					}
				}
			}
		}

		ShowHeroDetails();

		if(currentHero != null)
		{
			heroShip.UpdateButtons();
		}
	}

	public void ShowHeroDetails()
	{
		for(int i = 0; i < 3; ++i)
		{
			if(i < playerTurnScript.playerOwnedHeroes.Count)
			{
				heroName[i].transform.parent.GetComponent<UISprite>().spriteName = "Button Normal";
				heroName[i].transform.Find("Button").GetComponent<UIButton>().isEnabled = true;
				heroName[i].transform.Find("Button").GetComponent<UISprite>().spriteName = "Button Click";
				heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();
				heroHealth[i].text = Math.Round(heroScript.currentHealth, 1) + "/" + Math.Round(heroScript.maxHealth, 1);
				heroName[i].text = "Hero Dude/Ette";
			}
			else
			{
				heroHealth[i].text = "";
				heroName[i].text = "";
				heroName[i].transform.parent.GetComponent<UISprite>().spriteName = "Button Deactivated";
				heroName[i].transform.Find("Button").GetComponent<UIButton>().isEnabled = false;
				heroName[i].transform.Find("Button").GetComponent<UISprite>().spriteName = "Empty Line";
			}
		}
	}

	public void InvasionButtonClick()
	{
		heroScript = currentHero.GetComponent<HeroScriptParent> ();
		systemInvasion.hero = heroScript;
		systemInvasion.StartSystemInvasion(heroScript.system);
	}

	public void Embargo()
	{
		heroScript = currentHero.GetComponent<HeroScriptParent> ();
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.promotedBy = null;
		systemSIMData.embargoedBy = heroScript.heroOwnedBy;
		systemSIMData.embargoTimer = Time.time;
	}

	public void Promote()
	{
		heroScript = currentHero.GetComponent<HeroScriptParent> ();
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		systemSIMData.embargoedBy = null;
		systemSIMData.promotedBy = heroScript.heroOwnedBy;
		systemSIMData.promotionTimer = Time.time;
	}

	public void GuardProtectSwitch()
	{
		systemSIMData = heroScript.heroLocation.GetComponent<SystemSIMData> ();
		if(guardButton.GetComponent<UILabel>().text == "Guard")
		{
			systemSIMData.guardedBy = heroScript.heroOwnedBy;
		}
		if(guardButton.GetComponent<UILabel>().text == "Protect")
		{
			systemSIMData.protectedBy = heroScript.gameObject;
		}
	}
}
