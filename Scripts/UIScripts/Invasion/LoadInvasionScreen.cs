using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadInvasionScreen : MasterScript 
{
	private TokenManagement management;

	void Start()
	{
		management = gameObject.GetComponent<TokenManagement> ();
	}

	public int CheckForExistingInvasion(int system)
	{
		for(int i = 0; i < systemInvasion.currentInvasions.Count; ++i)
		{
			if(systemInvasion.currentInvasions[i].system == systemListConstructor.systemList[system].systemObject)
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
			newToken.transform.position = tokenList[i].currentPosition;
			TokenUI tokenScript = newToken.GetComponent<TokenUI>();

			tokenScript.hero = tokenList[i].heroOwner;
			tokenScript.originalParent = tokenList[i].originalParent;
			tokenScript.originalPosition = tokenList[i].originalPosition;
			tokenScript.name = tokenList[i].name;

			Debug.Log (newToken.transform.position + " | " + tokenScript.originalPosition);

			management.AssignTokenButton(newToken.GetComponent<UIButton>(), type);

			management.allTokens.Add(newToken);
		}
	}

	public void ReloadInvasionScreen(int loc)
	{
		for(int i = 0; i < systemInvasion.currentInvasions[loc].tokenAllocation.Count; ++i)
		{
			PositionTokens (systemInvasion.currentInvasions[loc].tokenAllocation[i].assaultTokenAllocation, i, "Assault");
			PositionTokens (systemInvasion.currentInvasions[loc].tokenAllocation[i].auxiliaryTokenAllocation, i, "Auxiliary");
			PositionTokens (systemInvasion.currentInvasions[loc].tokenAllocation[i].defenceTokenAllocation, i, "Defence");
		}
	}
}
