using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TokenManagement : MonoBehaviour 
{
	private TokenBehaviour behaviour;
	public GameObject token;
	public List<GameObject> allTokens = new List<GameObject>();

	void Start()
	{
		behaviour = MasterScript.invasionGUI.tokenContainer.GetComponent<TokenBehaviour> ();
	}

	public void CacheInvasionInfo() //Used to cache the invasion info
	{
		SystemInvasionInfo cachedInvasion = new SystemInvasionInfo (); //Create a new invasion object
		
		int invasionLoc = -1; //Set a counter
		int system = MasterScript.RefreshCurrentSystem(MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent> ().heroLocation);
		
		for(int i = 0; i < MasterScript.systemInvasion.currentInvasions.Count; ++i) //For all current cached invasions
		{
			if(MasterScript.systemInvasion.currentInvasions[i].system == MasterScript.systemListConstructor.systemList[system].systemObject) //If this system already has an invasion underway
			{
				invasionLoc = i; //Set the counter
			}
		}
		
		cachedInvasion.system = MasterScript.systemListConstructor.systemList[system].systemObject; //Set the system to equal this system
		cachedInvasion.player = MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent> ().heroOwnedBy;
		
		for(int i = 0; i < MasterScript.systemListConstructor.systemList[system].systemSize; ++i) //For all planets in the system
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
				temp.originalHero = tokenScript.originalHero;
				temp.tokenPositions = tokenScript.tokenPositions;
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
			MasterScript.systemInvasion.currentInvasions.Add (cachedInvasion); //Cache it
			SystemDefence systemDefence = MasterScript.systemListConstructor.systemList[system].systemObject.GetComponent<SystemDefence>();
			systemDefence.underInvasion = true;
		}
		else //If it is
		{
			MasterScript.systemInvasion.currentInvasions[invasionLoc] = cachedInvasion; //Replace it with the updated one
		}
	}

	public void CreateTokens(string tokenType, HeroScriptParent hero, int pos) //Used to create, position and assign the tokens for the heroes
	{
		int tokenCount = 0;
		List<GameObject> container = new List<GameObject>();
		
		switch (tokenType) //Used to assign both the parent for the token, and the number of tokens available to that hero
		{
		case "Assault":
			tokenCount = hero.assaultTokens;
			container = MasterScript.invasionGUI.heroInvasionLabels[pos].assaultTokenPositions;
			break;
		case "Auxiliary":
			tokenCount = hero.auxiliaryTokens;
			container = MasterScript.invasionGUI.heroInvasionLabels[pos].auxiliaryTokenPositions;
			break;
		case "Defence":
			tokenCount = hero.defenceTokens;
			container = MasterScript.invasionGUI.heroInvasionLabels[pos].defenceTokenPositions;
			break;
		default:
			break;
		}
		
		for(int i = 0; i < 6; ++i) //For all possible token positions
		{
			if(i < tokenCount) //If the hero has this token
			{
				NGUITools.SetActive(container[i], true); //Set the position image to true (the faded greyscale image)
				GameObject tempToken = NGUITools.AddChild(container[i].transform.parent.gameObject, token); //Instantiate the token
				EventDelegate.Add (tempToken.GetComponent<UIButton>().onClick, behaviour.ButtonClicked); //Add button clicked event

				tempToken.transform.position = container[i].transform.position; //Set the position of the token
				
				TokenUI tokenUI = tempToken.GetComponent<TokenUI>(); //Get a reference to the token's script
				tokenUI.originalPosition = i; //Assign the original position of the token
				tokenUI.originalParent = container[i].transform.parent.gameObject; //Assign the original parent
				tokenUI.originalHero = pos;
				tokenUI.tokenPositions = container;
				tokenUI.hero = MasterScript.heroGUI.currentHero; //And assign which hero owns it
				
				UIButton tokenButton = tempToken.GetComponent<UIButton>(); //Get the button attached to the token
				
				AssignTokenButton(tokenButton, tokenType);

				switch (tokenType) //And change the button's sprites based on what kind of token it is
				{
				case "Assault":
					tokenUI.name = "Assault Token";
					MasterScript.invasionGUI.heroInvasionLabels[pos].assaultTokensList.Add (tempToken);
					break;
				case "Auxiliary":
					tokenUI.name = "Auxiliary Token";
					MasterScript.invasionGUI.heroInvasionLabels[pos].auxiliaryTokensList.Add (tempToken);
					break;
				case "Defence":
					tokenUI.name = "Defence Token";
					MasterScript.invasionGUI.heroInvasionLabels[pos].defenceTokensList.Add (tempToken);
					break;
				default:
					break;
				}

				allTokens.Add (tempToken);
			}
			
			else
			{
				NGUITools.SetActive(container[i], false); //If the hero does not have this token, disable the image and don't do anything
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
		for(int i = 0; i < MasterScript.invasionGUI.heroInvasionLabels.Count; ++i) //For all active heroes
		{
			if(MasterScript.invasionGUI.heroInvasionLabels[i].reset == UIButton.current.gameObject || UIButton.current.gameObject.name == "Reset All") //If button pressed corresponds to this hero, or the button was reset all
			{
				ResetTokenPositions(MasterScript.invasionGUI.heroInvasionLabels[i].assaultTokensList, false); //Call the reset function with the gameobject lists containing the tokens but do not destroy tokens
				ResetTokenPositions(MasterScript.invasionGUI.heroInvasionLabels[i].auxiliaryTokensList, false);
				ResetTokenPositions(MasterScript.invasionGUI.heroInvasionLabels[i].defenceTokensList, false);
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

				if(tokenList[i].transform.parent.name == "Defence Token" || tokenList[i].transform.parent.name == "Auxiliary Token" || tokenList[i].transform.parent.name == "Assault Token") //If it already has a parent (is already in a container)
				{
					UILabel label = tokenList[i].transform.parent.Find ("Label").gameObject.GetComponent<UILabel>(); //Decrease the container's value
					int j = int.Parse (label.text);
					label.text = (j - 1).ToString();
				}

				tokenList[i].transform.position = token.tokenPositions[token.originalPosition].transform.position;

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
		for(int i = 0; i < MasterScript.playerTurnScript.playerOwnedHeroes.Count; ++i)
		{
			if(MasterScript.playerTurnScript.playerOwnedHeroes[i] == MasterScript.heroGUI.currentHero)
			{
				continue;
			}
			
			HeroScriptParent heroScript = MasterScript.playerTurnScript.playerOwnedHeroes[i].GetComponent<HeroScriptParent>();
			
			if(heroScript.heroLocation == MasterScript.heroGUI.currentHero.GetComponent<HeroScriptParent>().heroLocation)
			{
				int tempNum = 0;
				
				if(UIButton.current.gameObject == MasterScript.invasionGUI.includeHero2)
				{
					NGUITools.SetActive(MasterScript.invasionGUI.includeHero2, false);
					MasterScript.invasionGUI.includeHero2.transform.Find ("Label").GetComponent<UILabel>().text = "";
					tempNum = 1;
				}
				if(UIButton.current.gameObject == MasterScript.invasionGUI.includeHero3)
				{
					NGUITools.SetActive(MasterScript.invasionGUI.includeHero3, false);
					MasterScript.invasionGUI.includeHero3.transform.Find ("Label").GetComponent<UILabel>().text = "";
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
