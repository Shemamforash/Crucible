using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionDetection : MasterScript 
{
	private float t;
	private Vector2 startA, endA, startB, endB, mp1, mp2;
	private int startNo;

	private List<GameObject> unvisitedSystems = new List<GameObject> ();
	private List<GameObject> firmSystems = new List<GameObject> ();
	
	void Start()
	{
		t = 0f;
		startNo = 0;
	}

	void Update () //FIXED PLS DONT CHANGE THIS FUTURE SAM
	{
		t += Time.deltaTime;

		if(t >= 0.3f)
		{
			if(startNo >= systemListConstructor.systemList.Count)
			{
				startNo = 0;
			}

			for(int i = startNo; i < startNo + 5; ++i)
			{
				UpdateConnections (i);
				CheckForGraphComplete ();
			}

			t = 0f;

			startNo += 5;
		}
	}

	private void CheckVisitedSystems()
	{
		bool foundSystem = false;
		
		for(int i = 0; i < firmSystems.Count; ++i) //For all current systems
		{
			int sys = RefreshCurrentSystem (firmSystems [i]);
			
			for(int j = 0; j < systemListConstructor.systemList[sys].permanentConnections.Count; ++j) //Check the systems permanent connections
			{
				if(firmSystems.Contains(systemListConstructor.systemList[sys].permanentConnections[j])) //If the system is already in firm systems, ignore it
				{
					continue;
				}
				
				firmSystems.Add (systemListConstructor.systemList[sys].permanentConnections[j]); //Add systems to firm systems
				unvisitedSystems.Remove (systemListConstructor.systemList[sys].permanentConnections[j]); //Remove the system from unvisited systems
				
				foundSystem = true; //Found a system
			}
		}

		if(foundSystem == true) //If system is found
		{
			CheckVisitedSystems(); //Repeat
		}
	}

	private void CheckForGraphComplete()
	{
		unvisitedSystems.Clear (); //Clear lists
		firmSystems.Clear ();

		for(int i = 1; i < systemListConstructor.systemList.Count; ++i) //For all systems
		{
			unvisitedSystems.Add (systemListConstructor.systemList[i].systemObject); //Add them to the unvisited systems
		}

		firmSystems.Add (systemListConstructor.systemList[0].systemObject); //Add the first system to firm systems

		CheckVisitedSystems (); //Start the cycle of checking for systems

		if(firmSystems.Count != systemListConstructor.systemList.Count) //If the size of firm systems is not equal to the systemlist, this means that not all systems are connected
		{
			float distance = 10000f;
			int sysToJoinA = -1, sysToJoinB = -1;

			for(int i = 0; i < firmSystems.Count; ++i)
			{
				for(int j = 0; j < unvisitedSystems.Count; ++j)
				{
					float tempDistance = Vector3.Distance (firmSystems[i].transform.position, unvisitedSystems[j].transform.position);

					if(tempDistance < distance)
					{
						if(mapConstructor.TestForIntersection(firmSystems[i].transform.position, unvisitedSystems[j].transform.position, false) == false)
						{
							distance = tempDistance;
							sysToJoinA = i;
							sysToJoinB = j;
						}
					}
				}
			}

			if(sysToJoinA != -1 && sysToJoinB != -1)
			{
				sysToJoinA = RefreshCurrentSystem(firmSystems[sysToJoinA]);
				sysToJoinB = RefreshCurrentSystem(unvisitedSystems[sysToJoinB]);

				systemListConstructor.systemList[sysToJoinA].permanentConnections.Add (systemListConstructor.systemList[sysToJoinB].systemObject);
				systemListConstructor.systemList[sysToJoinA].numberOfConnections = systemListConstructor.systemList[sysToJoinA].permanentConnections.Count;
				systemListConstructor.systemList[sysToJoinB].permanentConnections.Add (systemListConstructor.systemList[sysToJoinA].systemObject);
				systemListConstructor.systemList[sysToJoinB].numberOfConnections = systemListConstructor.systemList[sysToJoinB].permanentConnections.Count;

				lineRenderScript = systemListConstructor.systemList[sysToJoinA].systemObject.GetComponent<LineRenderScript>();
				lineRenderScript.CreateNewLine(sysToJoinA);
				lineRenderScript = systemListConstructor.systemList[sysToJoinB].systemObject.GetComponent<LineRenderScript>();
				lineRenderScript.CreateNewLine(sysToJoinB);
			}
		}
	}

	private void UpdateConnections(int system)
	{
		startA = new Vector2(systemListConstructor.systemList[system].systemObject.transform.position.x, systemListConstructor.systemList[system].systemObject.transform.position.y); //Start vector is this system

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i) //For all connections
		{
			float dist = Vector3.Distance (gameObject.transform.position, systemListConstructor.systemList[system].permanentConnections[i].transform.position);
			
			if(dist > 15f) //If distance is greater than 15
			{
				for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //Look through all systems
				{
					if(j == system) //Ignore self
					{
						continue;
					}
					
					if(Vector3.Distance (systemListConstructor.systemList[j].systemObject.transform.position, gameObject.transform.position) <= 15) //If distance between this system and other system is under 15
					{
						ReconnectSystems(gameObject, systemListConstructor.systemList[system].permanentConnections[i]); //Reconnect the system
						ReconnectSystems(systemListConstructor.systemList[system].permanentConnections[i], gameObject); //Reconnect the system
					}
				}
			}
		}

		for(int i = 0; i < systemListConstructor.systemList[system].permanentConnections.Count; ++i)
		{
			endA = new Vector2(systemListConstructor.systemList[system].permanentConnections[i].transform.position.x, systemListConstructor.systemList[system].permanentConnections[i].transform.position.y);
			
			for(int j = 0; j < systemListConstructor.systemList.Count; ++j)
			{
				startB = new Vector2(systemListConstructor.systemList[j].systemObject.transform.position.x, systemListConstructor.systemList[j].systemObject.transform.position.y);
				
				for(int k = 0; k < systemListConstructor.systemList[j].permanentConnections.Count; ++k)
				{
					endB = new Vector2(systemListConstructor.systemList[j].permanentConnections[k].transform.position.x, systemListConstructor.systemList[j].permanentConnections[k].transform.position.y);
					
					if(startA == startB || startA == endB || endA == startB || endA == endB)
					{
						continue;
					}

					float distanceA = Vector2.Distance(startA, endA);
					float distanceB = Vector2.Distance(startB, endB);
					mp1 = (startA + endA) / 2;
					mp2 = (startB + endB) / 2;
					float distanceC = Vector2.Distance(mp1, mp2);

					if(distanceC * 2 > distanceA + distanceB)
					{
						continue;
					}

					if(LineIntersection(startA, endA, startB, endB) == true)
					{
						if(distanceA > distanceB)
						{
							if(i >= systemListConstructor.systemList[system].permanentConnections.Count)
							{
								continue;
							}

							GameObject target = systemListConstructor.systemList[system].permanentConnections[i];
							
							ReconnectSystems(systemListConstructor.systemList[system].systemObject, target);
							ReconnectSystems(target, systemListConstructor.systemList[system].systemObject);
						}

						if(distanceB >= distanceA)
						{
							continue;
						}
					}
				}
			}
		}

		CheckForGraphComplete();
	}

	private bool LineIntersection (Vector2 a, Vector2 b, Vector2 c, Vector2 d)
	{
		float M1 = (b.y - a.y) / (b.x - a.x);
		float C1 = a.y - (M1 * a.x);
		
		float M2 = (d.y - c.y) / (d.x - c.x);
		float C2 = c.y - (M2 * c.x);
		
		if(M2 == M1)
		{
			return false;
		}
		
		float xIntersect = (C2 - C1) / (M1 - M2);

		if(xIntersect > 95f || xIntersect < 0f)
		{
			return false;
		}

		float yOne = (M1 * xIntersect) + C1;
		float yTwo = (M2 * xIntersect) + C2;
		
		if(yOne == yTwo /*|| Mathf.Abs((yOne - yTwo) / 2) <= 2.0f*/)
		{
			if(yOne > 95f || yOne < 0f)
			{
				return false;
			}

			if(xIntersect < Mathf.Max (a.x, b.x) && xIntersect > Mathf.Min (a.x, b.x))
			{
				if(yOne < Mathf.Max (a.y, b.y) && yOne > Mathf.Min (a.y, b.y))
				{
					if(xIntersect < Mathf.Max (c.x, d.x) && xIntersect > Mathf.Min (c.x, d.x))
					{
						if(yOne < Mathf.Max (c.y, d.y) && yOne > Mathf.Min (c.y, d.y))
						{
							return true;
						}
					}
				}
			}
		}
		
		return false;
	}

	public void ReconnectSystems(GameObject systemA, GameObject systemB)
	{
		int thisSystem = RefreshCurrentSystem (systemA); //Get this system
		
		for(int i = 0; i < systemListConstructor.systemList[thisSystem].permanentConnections.Count; ++i) //For all permanent connections in this system
		{
			if(systemListConstructor.systemList[thisSystem].permanentConnections[i] == systemB) //If connection equals target
			{
				float distance = 100f; //Set max distance
				int newConnection = -1;
				
				for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //For all other systems
				{
					if(systemListConstructor.systemList[j].systemObject == systemA || systemListConstructor.systemList[j].systemObject == systemB) //If system equals either of original systems, continue
					{
						continue;
					}

					/*if(systemListConstructor.systemList[j].numberOfConnections > 7 && systemListConstructor.systemList[thisSystem].numberOfConnections > 0) //If system has more than 7 connections
					{
						continue;
					}*/
					
					if(mapConstructor.TestForIntersection(systemA.transform.position, systemListConstructor.systemList[j].systemObject.transform.position, false) == false) //If there is no intersection between this system and other system
					{
						float tempDistance = Vector3.Distance (systemA.transform.position, systemListConstructor.systemList[j].systemObject.transform.position);
						
						if(tempDistance < distance) //Test if distance is less than current max distance
						{
							distance = tempDistance; //If it is
							newConnection = j; //Set this system as the new connection
						}
					}
				}
				
				if(newConnection != -1) //If the new connection is not -1
				{
					systemListConstructor.systemList[thisSystem].permanentConnections[i] = systemListConstructor.systemList[newConnection].systemObject; //The connection that was system b is now the new connection
					systemListConstructor.systemList[newConnection].permanentConnections.Add(systemListConstructor.systemList[thisSystem].systemObject); //The new connection is now connected to this system

					lineRenderScript = systemListConstructor.systemList[newConnection].systemObject.GetComponent<LineRenderScript>();

					lineRenderScript.CreateNewLine(newConnection);
					
					systemListConstructor.systemList[thisSystem].numberOfConnections = systemListConstructor.systemList[thisSystem].permanentConnections.Count;
					systemListConstructor.systemList[newConnection].numberOfConnections = systemListConstructor.systemList[newConnection].permanentConnections.Count;
				}
				
				if(newConnection == -1)
				{
					int tempSystem = RefreshCurrentSystem(systemListConstructor.systemList[thisSystem].permanentConnections[i]);
					
					lineRenderScript = systemListConstructor.systemList[tempSystem].systemObject.GetComponent<LineRenderScript>();
					
					for(int j = 0; j < systemListConstructor.systemList[tempSystem].permanentConnections.Count; ++j)
					{
						if(systemListConstructor.systemList[tempSystem].permanentConnections[j] = systemListConstructor.systemList[thisSystem].systemObject)
						{
							systemListConstructor.systemList[tempSystem].permanentConnections.RemoveAt (j);
							NGUITools.Destroy(lineRenderScript.connectorLines[j].thisLine);
							lineRenderScript.connectorLines.RemoveAt (j);
							systemListConstructor.systemList[tempSystem].numberOfConnections = systemListConstructor.systemList[thisSystem].permanentConnections.Count;
						}
					}
					
					lineRenderScript = systemListConstructor.systemList[thisSystem].systemObject.GetComponent<LineRenderScript>();
					
					systemListConstructor.systemList[thisSystem].permanentConnections.RemoveAt (i);
					NGUITools.Destroy(lineRenderScript.connectorLines[i].thisLine);
					lineRenderScript.connectorLines.RemoveAt (i);
					
					systemListConstructor.systemList[thisSystem].numberOfConnections = systemListConstructor.systemList[thisSystem].permanentConnections.Count;
				}
				
				break;
			}
		}
	}

	/*
	private void ToBeNamed(int system)
	{
		for(int i = 0; i < systemListConstructor.systemList[system].numberOfConnections; ++i) //For all connections in system
		{
			float distance = 1000f;
			int newSystem = -1;

			for(int j = 0; j < systemListConstructor.systemList.Count; ++j) //For all other systems
			{
				if(systemListConstructor.systemList[j].numberOfConnections < 7) //If target system has fewer than 7 other connections
				{
					float tempDistance = Vector3.Distance (systemListConstructor.systemList[system].systemObject.transform.position, systemListConstructor.systemList[j].systemObject.transform.position); //Get distance to target

					if(tempDistance < distance) //If distance is shorter than current minimum distance
					{
						if(systemListConstructor.systemList[system].permanentConnections.Contains(systemListConstructor.systemList[j].systemObject)) //If target is already in the connections
						{
							distance = tempDistance; //Assign it anyway
							newSystem = j;
							continue;
						}

						else if(mapConstructor.TestForIntersection(systemListConstructor.systemList[system].systemObject.transform.position, systemListConstructor.systemList[j].systemObject.transform.position, false) == false)
						{
							distance = tempDistance; //Else if target does not intersect with existing lines, assign it
							newSystem = j;
							continue;
						}
					}
				}
			}
		
			if(i < systemListConstructor.systemList[system].permanentConnections.Count)
			{
				int targetConnection = RefreshCurrentSystem(systemListConstructor.systemList[system].permanentConnections[i]); //Get connection number

				for(int j = 0; j < systemListConstructor.systemList[targetConnection].permanentConnections.Count; ++j) //For all connections in target connection
				{
					if(systemListConstructor.systemList[targetConnection].permanentConnections[j] == systemListConstructor.systemList[system].systemObject) //If target's connection equals this system
					{
						lineRenderScript = systemListConstructor.systemList[targetConnection].systemObject.GetComponent<LineRenderScript>();

						lineRenderScript.connectorLines.RemoveAt(j);

						systemListConstructor.systemList[targetConnection].permanentConnections.RemoveAt (j); //Remove the connection- it is to be reassigned
						break;
					}
				}
			}

			if(newSystem == -1 && i >= systemListConstructor.systemList[system].permanentConnections.Count)
			{
				systemListConstructor.systemList[system].permanentConnections.RemoveAt(i);

				lineRenderScript = systemListConstructor.systemList[system].systemObject.GetComponent<LineRenderScript>();

				lineRenderScript.connectorLines.RemoveAt (i);
			}

			if(newSystem != -1)
			{
				if(systemListConstructor.systemList[system].permanentConnections.Count == systemListConstructor.systemList[system].numberOfConnections) //If the number of actual connections equals the maximum number
				{
					systemListConstructor.systemList[system].permanentConnections[i] = systemListConstructor.systemList[newSystem].systemObject; //Simply reassign the connection
				}

				if(systemListConstructor.systemList[system].permanentConnections.Count < systemListConstructor.systemList[system].numberOfConnections) //If it is less
				{
					systemListConstructor.systemList[system].permanentConnections.Add (systemListConstructor.systemList[newSystem].systemObject); //Add a new system

					CreateNewLine(system);
				}
			}
		}
	}
	*/
}
