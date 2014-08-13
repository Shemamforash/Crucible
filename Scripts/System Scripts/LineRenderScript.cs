using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LineRenderScript : MasterScript 
{
	[HideInInspector]
	public List<ConnectorLine> connectorLines = new List<ConnectorLine>();
	public GameObject line, clone;
	[HideInInspector]
	public int thisSystem;
	[HideInInspector]
	public Material opaqueMaterial;
	public Transform connectorLineContainer;
	private Quaternion rotation;
	private Vector3 midPoint, scale;
	private float pixelWidth, pixelHeight, systemPixelSize;

	public void StartUp()
	{	
		connectorLineContainer = GameObject.Find ("Connector Lines Container").transform;
		systemSIMData = gameObject.GetComponent<SystemSIMData>();
		thisSystem = RefreshCurrentSystem (gameObject);

		CreateLines ();
	}

	void Update()
	{
		if(systemListConstructor.systemList[thisSystem].systemOwnedBy == playerTurnScript.playerRace)
		{
			ViewNearbySystems();
		}

		for(int i = 0; i < connectorLines.Count; ++i)
		{
			UpdateLine(systemListConstructor.systemList[thisSystem].permanentConnections[i], i);
		}
	}

	private void CreateLines()
	{
		for(int i = 0; i < systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i)
		{
			CreateNewLine(i);
		}
	}

	public void CreateNewLine(int system)
	{
		ConnectorLine newLine = new ConnectorLine ();
		
		GameObject clone = (GameObject)Instantiate(line, Vector3.zero, Quaternion.identity); //NGUITools.AddChild(connectorLineContainer.gameObject, line);
		
		clone.transform.parent = connectorLineContainer;
		
		newLine.thisLine = clone;
		
		connectorLines.Add (newLine);
	}

	private void SetPosition(GameObject target, int i)
	{
		midPoint = (gameObject.transform.position + target.transform.position) / 2; //Get midpoint between target and current system
		
		midPoint = new Vector3 (midPoint.x, midPoint.y, 0.0f); //Create vector from midpoint

		connectorLines[i].thisLine.transform.position = midPoint; //Set position of line to midpoint
	}

	private void SetRotation(GameObject target, int i)
	{
		if(target == null)
		{
			Debug.Log (target);
		}

		float distance = Vector3.Distance (gameObject.transform.position, target.transform.position);

		float rotationZRad = Mathf.Acos ((target.transform.position.y - gameObject.transform.position.y) / distance);
		
		float rotationZ = rotationZRad * Mathf.Rad2Deg;
		
		if(gameObject.transform.position.x < target.transform.position.x)
		{
			rotationZ = -rotationZ;
		}
		
		Vector3 vectRotation = new Vector3(0.0f, 0.0f, rotationZ);
		
		rotation.eulerAngles = vectRotation;
		
		connectorLines[i].thisLine.transform.rotation = rotation;
	}


	private void UpdateLine(GameObject target, int i)
	{
		SetPosition (target, i);
		SetRotation (target, i);
		
		connectorLines[i].thisLine.gameObject.renderer.material = SetRaceLineColour();

		Vector3 start = gameObject.transform.position;
		Vector3 end = target.transform.position;

		float distance = Vector3.Distance (start, end);
		distance = distance - 4f;

		connectorLines [i].thisLine.transform.localScale = new Vector3 (0.14f, distance, 0.0f);
	}

	public Material SetRaceLineColour()
	{
		if(systemListConstructor.systemList[thisSystem].systemOwnedBy == "Humans")
		{
			return turnInfoScript.humansLineMaterial;
		}
		if(systemListConstructor.systemList[thisSystem].systemOwnedBy == "Selkies")
		{
			return turnInfoScript.selkiesLineMaterial;
		}
		if(systemListConstructor.systemList[thisSystem].systemOwnedBy == "Nereides")
		{
			return turnInfoScript.nereidesLineMaterial;
		}

		return turnInfoScript.unownedLineMaterial;
	}

	private void ViewNearbySystems()
	{
		int system = RefreshCurrentSystem (gameObject);

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			int tempSystem = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]);

			if(systemListConstructor.systemList[tempSystem].systemOwnedBy == null)
			{
				//systemListConstructor.systemList[system].permanentConnections[i].renderer.material = opaqueMaterial;
			}
		}
	}
}

public class ConnectorLine
{
	public Quaternion rotation;
	public Vector3 midPoint;
	public GameObject thisLine;
}