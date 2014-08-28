using UnityEngine;
using System.Collections;

public class CameraFunctions : MonoBehaviour
{
	//This class contains all functions related to camera behaviour. This includes panning and zooming of the main camera, as well as using raycasts to update the current selected object.
	//It also includes mouse functions (double click).
	
	public Camera cameraMain;
	public float zoomSpeed, minZoom, maxZoom, panSpeed, zPosition, t;
	[HideInInspector]
	public GameObject selectedSystem;
	public int selectedSystemNumber;
	[HideInInspector]
	public bool doubleClick = false, singleClick = false, coloniseMenu = false, openMenu = false, moveCamera = false, zoom;
	
	private float leftBound = 0f, rightBound = 90f, upperBound = 90f, lowerBound = 0f;
	private float timer = 0.0f, clickTimer;
	private float updatedX, updatedY;
	private GameObject thisObject;
	private TechTreeGUI techTreeGUI;

	private Vector3 initPos, finalPos, lastFrameMousePos = Vector3.zero;

	void Start()
	{
		techTreeGUI = GameObject.Find ("GUIContainer").GetComponent<TechTreeGUI> ();
		minZoom = (0.1333333f * MasterScript.systemListConstructor.mapSize) - 29f;
		zoomSpeed = (maxZoom - minZoom) / -5f;
		zPosition = minZoom;
		clickTimer = 0f;
	}

	void Update()
	{
		if(openMenu != true)
		{
			DoubleClick(); //Checks for double click
			ZoomCamera();
			PanCamera();
			ClickSystem ();
			ClickAndDrag();
		}
		
		if(Input.GetKeyDown ("escape")) //Used to close all open menus, and to reset doubleclick
		{
			CloseAllWindows();
		}
	}

	public void CloseAllWindows()
	{
		coloniseMenu = false;
		openMenu = false;
		doubleClick = false;
		clickTimer = -1f;
		MasterScript.heroGUI.openHeroLevellingScreen = false;
		MasterScript.invasionGUI.openInvasionMenu = false;

		if(techTreeGUI.techTree.activeInHierarchy == true)
		{
			techTreeGUI.ShowTechTree();
		}
	}

	private void ClickAndDrag()
	{
		if(Input.GetMouseButton(0)) //If the mouse is held down
		{
			Vector3 curMousePos = Input.mousePosition; //Get the current mouse pos in screen coordinates
			
			curMousePos = new Vector3(curMousePos.x, curMousePos.y, cameraMain.transform.position.z); //Set its z position equal to the height of the camera from the game
			
			curMousePos = cameraMain.ScreenToWorldPoint(curMousePos); //Get that position in world coordinates
			
			if(lastFrameMousePos != curMousePos && lastFrameMousePos != Vector3.zero) //If last frame position isn't 0 or the current position
			{
				Vector3 difference = curMousePos - lastFrameMousePos; //Get the difference between the last frame position and the current position
				
				cameraMain.transform.position += difference / 2; //Add half of this (for smoothing) to the current camera position
			}

			lastFrameMousePos = curMousePos; //Set the last frame position to be this frame's position
		}
		else //If it isn't
		{
			lastFrameMousePos = Vector3.zero; //Reset the last frame value
		}
	}

	private void ClickSystem()
	{
		if(Input.GetMouseButtonDown(0)) //Used to start double click events and to identify systems when clicked on. Throws up error if click on a connector object.
		{				
			RaycastHit hit = new RaycastHit(); //Create a raycast
			
			if(Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) //Send it out in direction of mouse
			{
				if(hit.collider.gameObject.tag == "StarSystem") //If it collides with a star system
				{
					selectedSystem = hit.collider.gameObject; //Set the selected system
					selectedSystemNumber = MasterScript.RefreshCurrentSystem(selectedSystem); //And get it's number
				}
			
				SingleClickFunctions();
				DoubleClickFunctions();
			}
		}
	}

	private void SingleClickFunctions()
	{
		if(MasterScript.systemListConstructor.systemList[selectedSystemNumber].systemOwnedBy == null) //If it is owned by nobody
		{
			MasterScript.systemSIMData = MasterScript.systemListConstructor.systemList[selectedSystemNumber].systemObject.GetComponent<SystemSIMData>();
			
			if(MasterScript.systemSIMData.guardedBy == MasterScript.playerTurnScript.playerRace || MasterScript.systemSIMData.guardedBy == null) //If the system is guarded by you or nobody
			{
				coloniseMenu = true; //Show the colonise button
			}
		}
	}

	private void DoubleClickFunctions()
	{
		if(doubleClick == true && MasterScript.heroResource.improvementScreen.activeInHierarchy == false) //If a double cick has occured and the improvement screen is not visible
		{
			doubleClick = false; //Reset double click
			
			if(MasterScript.systemListConstructor.systemList[selectedSystemNumber].systemOwnedBy == MasterScript.playerTurnScript.playerRace) //If the system is owned by you
			{
				if(MasterScript.galaxyGUI.planetSelectionWindow.activeInHierarchy == false) //And the first planet colonise window is not open
				{
					openMenu = true; //View the system
				}
			}
		}
	}

	public void PanCamera() //Used to pan camera
	{		
		updatedX = transform.position.x;
		updatedY = transform.position.y;

		if(Input.GetAxis ("Horizontal") > 0)
		{
			moveCamera = false;

			updatedX += panSpeed;
			
			if(updatedX > rightBound) //Prevent scrolling over screen edge
			{
				updatedX = rightBound;
			}
			
			transform.position = new Vector3 (updatedX, transform.position.y, transform.position.z);
		}

		if(Input.GetAxis ("Horizontal") < 0)
		{
			moveCamera = false;

			updatedX -= panSpeed;
			
			if(updatedX < leftBound) //Prevent scrolling over screen edge	
			{
				updatedX = leftBound;
			}
			
			transform.position = new Vector3 (updatedX, transform.position.y, transform.position.z);
		}

		if(Input.GetAxis ("Vertical") > 0)
		{
			moveCamera = false;

			updatedY += panSpeed;
			
			if(updatedY > upperBound) //Prevent scrolling over screen edge	
			{
				updatedY = upperBound;
			}
			
			transform.position = new Vector3 (transform.position.x, updatedY, transform.position.z);
		}

		if(Input.GetAxis ("Vertical") < 0)
		{
			moveCamera = false;

			updatedY -= panSpeed;
			
			if(updatedY < lowerBound) //Prevent scrolling over screen edge	
			{
				updatedY = lowerBound;
			}
			
			transform.position = new Vector3 (transform.position.x, updatedY, transform.position.z);
		}
	}
	
	public void ZoomCamera() //Changes height of camera
	{	
		if(zoom == true) //If camera is currently zooming
		{
			gameObject.transform.position = Vector3.Lerp(initPos, finalPos, t); //Lerp the camera position to target
			t += Time.deltaTime / 0.2f;
			
			if(timer < Time.time) //If timer is less than current time
			{
				transform.position = finalPos; //Auto set to final position
				t = 0f; //Reset t value
				zoom = false; //Camera is no longer zooming
				timer = 0f; //Rest timer
			}
		}

		else //If it isn't currently zooming
		{
			zPosition = transform.position.z; //Get the z position

			float moveDistance = 0f;

			if(Input.GetAxis ("Mouse ScrollWheel") < 0) //Zoom out
			{
				moveDistance = -zoomSpeed;
			}
			if(Input.GetAxis ("Mouse ScrollWheel") > 0) //Zoom in
			{
				moveDistance = zoomSpeed;
			}

			if(moveDistance != 0f)
			{
				moveCamera = false; //Prevent camera from being centered
				
				zPosition += moveDistance; //Move the z position by zoomspeed in +/- direction

				if(zPosition > minZoom) //If zposition is greater than min zoom
				{
					zPosition = minZoom; //Set zposition to min zoom
				}

				if(zPosition < maxZoom) //Same with max zoom
				{
					zPosition = maxZoom;
				}
				
				initPos = transform.position; //Set initial position to current position
				finalPos = new Vector3(transform.position.x, transform.position.y, zPosition); //Set final position to target zposition
				timer = Time.time + 0.2f; //Set timer to time plus 1/5th second
				zoom = true; //Camera is now zooming
			}
		}
	}

	public void CentreCamera() //Used to centre the camera over the last selected object, or the home planet if on first turn.
	{
		if(Input.GetKeyDown("f") && selectedSystem != null)
		{
			moveCamera = true;
			
			timer = Time.time;
			
			thisObject = selectedSystem;
		}
		
		if(moveCamera == true)
		{
			Vector3 homingPlanetPosition = new Vector3(thisObject.transform.position.x, thisObject.transform.position.y, -30.0f); //Target position
			
			Vector3 currentPosition = cameraMain.transform.position;
			
			if(cameraMain.transform.position == homingPlanetPosition || Time.time > timer + 1.0f) //If lerp exceeds timer, camera position will lock to point at object
			{
				homingPlanetPosition = cameraMain.transform.position;
				
				moveCamera = false; //Stop moving camera
				
				timer = 0.0f; //Reset timer
			}

			updatedX = cameraMain.transform.position.x;
			
			updatedY = cameraMain.transform.position.y;
			
			zPosition = cameraMain.transform.position.z;
			
			cameraMain.transform.position = Vector3.Lerp (currentPosition, homingPlanetPosition, 0.1f);
		}
	}
	
	private void DoubleClick() //Function for detecting double click
	{
		if(Input.GetMouseButtonDown(0))
		{
			if(singleClick == false)
			{
				singleClick = true;
				clickTimer = Time.time;
			}
			else
			{
				singleClick = false;
				doubleClick = true;
			}
		}

		if(singleClick == true)
		{
			if(Time.time - clickTimer > 0.2f)
			{
				singleClick = false;
			}
		}
	}
}
