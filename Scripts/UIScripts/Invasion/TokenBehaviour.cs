using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TokenBehaviour : MasterScript
{
	private List<GameObject> tokens = new List<GameObject>();
	public bool followingMouse = false;
	private GameObject currentContainer;
	public Transform[] tokenContainers = new Transform[6];
	private float proximity = 20f;

	void Update () 
	{
		if(CheckForContainer () == false) //If the container does not snap to a position
		{
			FollowMouse(); //The container should follow the mouse
		}

		if(tokens.Count > 0)
		{
			UpdateTokenPositions (); //Set all the tokens in the list to follow the gameobject
			PlaceInContainer();
		}
	}

	private void UpdateTokenPositions()
	{
		for(int i = 0; i < tokens.Count; ++i)
		{
			tokens[i].transform.position = gameObject.transform.position;
		}
	}

	private bool CheckForContainer()
	{
		Vector3 mousePos = Input.mousePosition; //Position of mouse
		
		for(int i = 0; i < 6; ++i) //For all children of token container
		{
			if(invasionGUI.planetList[i].activeInHierarchy == true) //If it IS a token container
			{
				foreach(Transform child in tokenContainers[i])
				{
					if(child.tag == "TokenContainer")
					{
						Vector3 childPos = UICamera.mainCamera.WorldToScreenPoint(child.position); //Get position of container in screen coordinates
						
						if(mousePos.x < childPos.x + proximity && mousePos.x > childPos.x - proximity && mousePos.y < childPos.y + proximity && mousePos.y > childPos.y - proximity) //If mouseposition is within distance of container
						{
							gameObject.transform.position = child.position; //Snap to position
							currentContainer = child.gameObject; //Set nearby container
							return true; //Nearby container found return true
						}
					}
				}
			}
		}
		
		currentContainer = null; //Object has no nearby container
		return false; //So return false
	}
	
	private void FollowMouse()
	{
		Vector3 position = Camera.main.ScreenToViewportPoint (Input.mousePosition); //New position is mouse position in viewport coordinates
		
		position = systemPopup.uiCamera.ViewportToWorldPoint (position); //Use uicamera to convert viewport coordinates to world coordinates
		
		Vector3 newPosition = new Vector3(position.x, position.y, -37.0f); //Set position
		
		gameObject.transform.position = newPosition;
	}

	public void PlaceInContainer()
	{
		if(currentContainer != null)
		{
			if(Input.GetMouseButtonDown(1))
			{
				AttachToContainer();
			}
		}

		if(Input.GetKeyDown("x"))
		{
			for(int i = 0; i < tokens.Count; ++i)
			{
				TokenUI token = tokens[i].GetComponent<TokenUI>();
				tokens[i].transform.position = token.originalPosition;
				tokens[i].transform.parent = token.originalParent.transform;
				tokens[i].GetComponent<UIButton>().isEnabled = true;
			}

			tokens.Clear ();
		}
	}

	public void ButtonClicked() //This method is called if the button is not already following the mouse
	{
		if(UICamera.currentTouchID == -1)
		{
			UIButton.current.gameObject.GetComponent<UIButton> ().isEnabled = false;
			
			if(UIButton.current.transform.parent.name == "Defence Token" || UIButton.current.transform.parent.name == "Auxiliary Token" || UIButton.current.transform.parent.name == "Assault Token") //If it already has a parent (is already in a container)
			{
				UILabel label = UIButton.current.transform.parent.Find ("Label").gameObject.GetComponent<UILabel>(); //Decrease the container's value
				int j = int.Parse (label.text);
				label.text = (j - 1).ToString();
			}

			if(tokens.Contains(UIButton.current.gameObject) == false) //If the tokens list does not already contain the clicked object
			{
				if(tokens.Count > 0) //If token has already been inserted into list
				{
					if(tokens[0].name == UIButton.current.gameObject.name) //Check the clicked token's name matches the name of the other tokens in the list
					{
						tokens.Add(UIButton.current.gameObject); //Add it to the list if it does
					}
				}
				else //If the list is empty
				{
					tokens.Add(UIButton.current.gameObject); //Add the token to the list
				}
			}
		}
	}
	
	private void UpdateParent(GameObject container) //This method is used to increase the container's value
	{
		for(int i = 0; i < tokens.Count; ++i)
		{
			tokens[i].transform.parent = container.transform;
			UILabel label = container.transform.Find ("Label").gameObject.GetComponent<UILabel>();
			int j = int.Parse (label.text);
			label.text = (j + 1).ToString();
			tokens[i].GetComponent<UIButton> ().isEnabled = true;
		}

		tokens.Clear ();
		currentContainer = null;
	}
	
	public void AttachToContainer() //This method is called if the button is clicked and the object is already following the mouse
	{
		if(currentContainer != null) //If it is snapped to a container
		{
			switch (currentContainer.name) //If the container name matches the active object
			{
			case "Defence Token":
				if(tokens[0].GetComponent<UISprite>().spriteName == "Defence Pressed")
				{
					UpdateParent(currentContainer); //UpdateParent
				}
				break;
			case "Auxiliary Token":
				if(tokens[0].GetComponent<UISprite>().spriteName == "Secondary Weapon Pressed")
				{
					UpdateParent(currentContainer);
				}
				break;
			case "Assault Token":
				if(tokens[0].GetComponent<UISprite>().spriteName == "Primary Weapon Pressed")
				{
					UpdateParent(currentContainer);
				}
				break;
			default:
				break;
			}
		}
	}
}
