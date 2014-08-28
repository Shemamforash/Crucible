using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadInvasionScreen : MonoBehaviour 
{
	private TokenManagement management;

	void Start()
	{
		management = gameObject.GetComponent<TokenManagement> ();
	}

	public int CheckForExistingInvasion(int system)
	{
		for(int i = 0; i < MasterScript.systemInvasion.currentInvasions.Count; ++i)
		{
			if(MasterScript.systemInvasion.currentInvasions[i].system == MasterScript.systemListConstructor.systemList[system].systemObject)
			{
				return i;
			}
		}
		return -1;
	}

	private void PositionTokens(List<TokenInfo> tokenList, int loc, string type)
	{
		for(int i = 0; i < tokenList.Count; ++i)
		{
			GameObject newToken = NGUITools.AddChild(management.token, tokenList[i].currentParent);
			TokenUI tokenScript = newToken.GetComponent<TokenUI>();

			tokenScript.hero = tokenList[i].heroOwner;
			tokenScript.tokenPositions = tokenList[i].tokenPositions;
			tokenScript.originalHero = tokenList[i].originalHero;
			tokenScript.originalParent = tokenList[i].originalParent;
			tokenScript.name = tokenList[i].name;

			newToken.transform.position = tokenScript.tokenPositions[tokenList[i].originalPosition].transform.position;

			management.AssignTokenButton(newToken.GetComponent<UIButton>(), type);

			management.allTokens.Add(newToken);
		}
	}

	public void ReloadInvasionScreen(int loc)
	{
		for(int i = 0; i < MasterScript.systemInvasion.currentInvasions[loc].tokenAllocation.Count; ++i)
		{
			PositionTokens (MasterScript.systemInvasion.currentInvasions[loc].tokenAllocation[i].assaultTokenAllocation, i, "Assault");
			PositionTokens (MasterScript.systemInvasion.currentInvasions[loc].tokenAllocation[i].auxiliaryTokenAllocation, i, "Auxiliary");
			PositionTokens (MasterScript.systemInvasion.currentInvasions[loc].tokenAllocation[i].defenceTokenAllocation, i, "Defence");
		}
	}
}
