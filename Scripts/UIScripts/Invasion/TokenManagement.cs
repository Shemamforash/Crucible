using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TokenManagement : MasterScript 
{
	private TokenBehaviour behaviour;
	public GameObject token;
	public List<GameObject> allTokens = new List<GameObject>();

	void Start()
	{
		behaviour = invasionGUI.tokenContainer.GetComponent<TokenBehaviour> ();
	}

	public void CacheInvasionInfo() //Used to cache the invasion info
	{
		SystemInvasionInfo cachedInvasion = new SystemInvasionInfo (); //Create a new invasion object
		
		int invasionLoc = -1; //Set a counter
		int system = RefreshCurrentSystem(heroGUI.currentHero.GetComponent<HeroScriptParent> ().heroLocation);
		
		for(int i = 0; i < systemInvasion.currentInvasions.Count; ++i) //For all current cached invasions
		{
			if(systemInvasion.currentInvasions[i].system == systemListConstructor.systemList[system].systemObject) //If this system already has an invasion underway
			{
				invasionLoc = i; //Set the counter
			}
		}
		
		cachedInvasion.system = systemListConstructor.systemList[system].systemObject; //Set the system to equal this system
		cachedInvasion.player = heroGUI.currentHero.GetComponent<HeroScriptParent> ().heroOwnedBy;
		
		for(int i = 0; i < systemListConstructor.systemList[system].systemSize; ++i) //For all planets in the system
		{
			PlanetInvasionInfo cachedPlanet = new PlanetInvasionInfo(); //Create object for planet

			for(int j = 0; j < allTokens.Count; ++j)//else //It must be a token
			{
				TokenUI tokenScript = allTokens[j].GetComponent<TokenUI>(); //So get a reference to it's script

				TokenInfo temp = new TokenInfo();

				temp.originalPosition = tokenScript.originalPosition;
				temp.currentPosition = allTokens[j].transform.position;
				temp.heroOwner = tokenScript.hero;
				temp.originalParent = tokenScript.originalParent;
				temp.currentParent = allTokens[j].transform.parent.gameObject;
				temp.name = tokenScript.name;

				switch(temp.name) //Switch based on name of container
				{
				case "Defence Token":
					cachedPlanet.defenceTokenAllocation.Add (temp); //If it's a defence token cache the token's hero into the planet list
					break;
				case "Assault Token":
					cachedPlanet.assaultTokenAllocation.Add (temp); //Same for assault token
					break;
				case "Auxiliary Token":
					cachedPlanet.auxiliaryTokenAllocation.Add (temp); //And for auxiliary token
					break;
				default:
					break;
				}
			}

			cachedInvasion.tokenAllocation.Add (cachedPlanet);
		}
		
		if(invasionLoc == -1) //If this invasion is not already cached
		{
			systemInvasion.currentInvasions.Add (cachedInvasion); //Cache it
			systemDefence = systemListConstructor.systemList[system].systemObject.GetComponent<SystemDefence>();
			systemDefence.underInvasion = true;
			allTokens.Clear ();
		}
		else //If it is
		{
			systemInvasion.currentInvasions[invasionLoc] = cachedInvasion; //Replace it with the updated one
			allTokens.Clear ();
		}
	}

	public void CreateTokens(string tokenType, HeroScriptParent hero, int pos) //Used to create, position and assign the tokens for the heroes
	{
		int tokenCount = 0;
		string container = null;
		
		switch (tokenType) //Used to assign both the parent for the token, and the number of tokens available to that hero
		{
		case "Assault":
			tokenCount = hero.assaultTokens;
			container = "Assault Tokens";
			break;
		case "Auxiliary":
			tokenCount = hero.auxiliaryTokens;
			container = "Auxiliary Tokens";
			break;
		case "Defence":
			tokenCount = hero.defenceTokens;
			container = "Defence Tokens";
			break;
		default:
			break;
		}
		
		for(int i = 0; i < 6; ++i) //For all possible token positions
		{
			GameObject parent = invasionGUI.heroInterfaces[pos].transform.Find (container).gameObject; //Get the relevant parent for the token
			GameObject tokenPositionObject = parent.transform.GetChild(i).gameObject; //Get the correct position
			
			if(i < tokenCount) //If the hero has this token
			{
				NGUITools.SetActive(tokenPositionObject, true); //Set the position image to true (the faded greyscale image)
				GameObject tempToken = NGUITools.AddChild(parent, token); //Instantiate the token
				EventDelegate.Add (tempToken.GetComponent<UIButton>().onClick, behaviour.ButtonClicked); //Add button clicked event

				tempToken.transform.position = parent.transform.GetChild(i).transform.position; //Set the position of the token
				
				TokenUI tokenUI = tempToken.GetComponent<TokenUI>(); //Get a reference to the token's script
				tokenUI.originalPosition = tempToken.transform.position; //Assign the original position of the token
				tokenUI.originalParent = parent; //Assign the original parent
				tokenUI.hero = heroGUI.currentHero; //And assign which hero owns it
				
				UIButton tokenButton = tempToken.GetComponent<UIButton>(); //Get the button attached to the token
				
				AssignTokenButton(tokenButton, tokenType);

				switch (tokenType) //And change the button's sprites based on what kind of token it is
				{
				case "Assault":
					tokenUI.name = "Assault Token";
					invasionGUI.heroInvasionLabels[pos].assaultTokensList.Add (tempToken);
					break;
				case "Auxiliary":
					tokenUI.name = "Auxiliary Token";
					invasionGUI.heroInvasionLabels[pos].auxiliaryTokensList.Add (tempToken);
					break;
				case "Defence":
					tokenUI.name = "Defence Token";
					invasionGUI.heroInvasionLabels[pos].defenceTokensList.Add (tempToken);
					break;
				default:
					break;
				}

				allTokens.Add (tempToken);
			}
			
			else
			{
				NGUITools.SetActive(tokenPositionObject, false); //If the hero does not have this token, disable the image and don't do anything
			}
		}
	}

	public void AssignTokenButton(UIButton tokenButton, string tokenType)
	{
		switch (tokenType) //And change the button's sprites based on what kind of token it is
		{
		case "Assault":
			tokenButton.normalSprite = "Primary Weapon Normal";
			tokenButton.hoverSprite = "Primary Weapon Hover";
			tokenButton.pressedSprite = "Primary Weapon Pressed";
			tokenButton.disabledSprite = "Primary Weapon Pressed";
			break;
		case "Auxiliary":
			tokenButton.normalSprite = "Secondary Weapon Normal";
			tokenButton.hoverSprite = "Secondary Weapon Hover";
			tokenButton.pressedSprite = "Secondary Weapon Pressed";
			tokenButton.disabledSprite = "Secondary Weapon Pressed";
			break;
		case "Defence":
			tokenButton.normalSprite = "Defence Normal";
			tokenButton.hoverSprite = "Defence Hover";
			tokenButton.pressedSprite = "Defence Pressed";
			tokenButton.disabledSprite = "Defence Pressed";
			break;
		default:
			break;
		}
	}

	public void ResetTokens() //Method activated when reset buttons are pressed
	{
		for(int i = 0; i < invasionGUI.heroInvasionLabels.Count; ++i) //For all active heroes
		{
			if(invasionGUI.heroInvasionLabels[i].reset == UIButton.current.gameObject || UIButton.current.gameObject.name == "Reset All") //If button pressed corresponds to this hero, or the button was reset all
			{
				ResetTokenPositions(invasionGUI.heroInvasionLabels[i].assaultTokensList, false); //Call the reset function with the gameobject lists containing the tokens but do not destroy tokens
				ResetTokenPositions(invasionGUI.heroInvasionLabels[i].auxiliaryTokensList, false);
				ResetTokenPositions(invasionGUI.heroInvasionLabels[i].defenceTokensList, false);
			}
		}
	}
	
	public void ResetTokenPositions(List<GameObject> tokenList, bool remove) //Method used to reset the token parent and position
	{
		for(int i = 0; i < tokenList.Count; ++i) //For all the tokens in the token list
		{
			if(remove == false) //Used if tokens are not to be destroyed
			{
				TokenUI token = tokenList[i].GetComponent<TokenUI>(); //Get a reference to the attached script

				Debug.Log(tokenList[i].transform.parent);

				if(tokenList[i].transform.parent.name == "Defence Token" || tokenList[i].transform.parent.name == "Auxiliary Token" || tokenList[i].transform.parent.name == "Assault Token") //If it already has a parent (is already in a container)
				{
					UILabel label = tokenList[i].transform.parent.Find ("Label").gameObject.GetComponent<UILabel>(); //Decrease the container's value
					int j = int.Parse (label.text);
					label.text = (j - 1).ToString();
				}

				tokenList[i].transform.position = token.originalPosition; //Set the position to the original position
				tokenList[i].transform.parent = token.originalParent.transform; //Set the parent to the original parent
			}
			if(remove == true) //Used if tokens are to be destroyed
			{
				NGUITools.Destroy(tokenList[i]);
			}
		}
		
		if(remove == true) //And empty list if destroyed
		{
			tokenList.Clear ();
		}
	}

	public void IncludeHero()
	{
		for(int i = 0; i < playerTurnScript.playerOwnedHeroes.Count; ++i)
		{
			if(playerTurnScript.playerOwnedHeroes[i] == heroGUI.currentHero)
			{
				continue;
			}
			
			heroScript = playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();
			
			if(heroScript.heroLocation == heroGUI.currentHero.GetComponent<HeroScriptParent>().heroLocation)
			{
				int tempNum = 0;
				
				if(UIButton.current.gameObject == invasionGUI.includeHero2)
				{
					NGUITools.SetActive(invasionGUI.includeHero2, false);
					invasionGUI.includeHero2.transform.Find ("Label").GetComponent<UILabel>().text = "";
					tempNum = 1;
				}
				if(UIButton.current.gameObject == invasionGUI.includeHero3)
				{
					NGUITools.SetActive(invasionGUI.includeHero3, false);
					invasionGUI.includeHero3.transform.Find ("Label").GetComponent<UILabel>().text = "";
					tempNum = 2;
				}
				
				CreateTokens("Assault", heroScript, tempNum);
				CreateTokens("Auxiliary", heroScript, tempNum);
				CreateTokens("Defence", heroScript, tempNum);
				break;
			}
		}
		
		NGUITools.SetActive (UIButton.current.gameObject, false);
	}
}
