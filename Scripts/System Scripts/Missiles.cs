using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Missiles : MonoBehaviour 
{
	public GameObject invasionButton, invasionLine, currentPointer = null, currentLine, missilePrefab;
	private Vector3 anchorPoint, currentPoint;
	private int system, target;
	private QuadraticBezierCurve curveBuilder;
	private List<Vector2> tempPath = new List<Vector2> ();

	private void RotateLine() //Rotates the line between selected system and target
	{
		float distance = Vector3.Distance (anchorPoint, currentPoint);
		
		if(distance != 0)
		{
			float rotationZRad = Mathf.Acos ((currentPoint.y - anchorPoint.y) / distance);
			
			float rotationZ = rotationZRad * Mathf.Rad2Deg;
			
			if(anchorPoint.x < currentPoint.x)
			{
				rotationZ = -rotationZ;
			}
			
			Vector3 vectRotation = new Vector3(0.0f, 0.0f, rotationZ);
			
			Quaternion rotation = new Quaternion ();
			
			rotation.eulerAngles = vectRotation;
			
			currentLine.transform.rotation = rotation;
		}
	}
	
	private void ResizeLine() //Resizes line to fit between system and target
	{
		Vector3 midpoint = (currentPoint + anchorPoint) / 2;
		
		float distance = Vector3.Distance (anchorPoint, currentPoint);
		
		currentLine.transform.localScale = new Vector3 (0.05f, distance, 0f);
		
		currentLine.transform.position = midpoint;
	}

	private Vector2 FindTrajectoryIntersections(Vector3 start, Vector3 end) //Finds nearest perpendicular side of obstructive systems
	{
		float A1 = end.y - start.y; //start is the system firing, end is the system fired at
		float B1 = start.x - end.x;
		float C1 = (A1 * start.x) + (B1 * start.y);

		float bMinusAX = end.x - start.x; //This is used to get the direction between the two points, (A,B) is perpendicular to the desired line, so this function finds the line perpendicular to that (the desired line!)
		float bMinusAY = end.y - start.y;
		float eucNormA = Mathf.Sqrt ((bMinusAX * bMinusAX) + (bMinusAY * bMinusAY)); //Euclidean normal = sqrt((e.x-s.x)^2 + (e.y-s.y)^2) 

		Vector3 dir = new Vector3 (bMinusAX / eucNormA, bMinusAY / eucNormA, 0f); //Direction = (x || y) / euclidean normal

		Ray ray = new Ray (start, dir); //Create ray from start point in direction of end point
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, 150f)) //If a collision occurs
		{
			if(hit.collider.gameObject.tag == "StarSystem") //Check it's a star
			{
				Vector3 colliderCentre = hit.collider.gameObject.transform.position; //Set collider object centre as vector3

				if(colliderCentre == MasterScript.systemListConstructor.systemList[target].systemObject.transform.position) //If the collision centre is equal to the targets position
				{
					return Vector2.zero; //Return the collider centre (we have reached the target)
				}

				float m = (colliderCentre.y - start.y) / (colliderCentre.x - start.x); //Get gradient from collider to start
				m = -1 / m; //Get inverse reciprocal of the gradient
				float c = colliderCentre.y - (m * colliderCentre.x); //Find y intercept
				float xIntersectOne = colliderCentre.x + MasterScript.systemListConstructor.systemScale * 4.0f / Mathf.Sqrt(1 + m * m); //Find first x intersection of boundary of collider with line
				float yIntersectOne = m * xIntersectOne + c; //And the corresponding y, these lie on the same line so share the same y intercept
				float xIntersectTwo = colliderCentre.x - MasterScript.systemListConstructor.systemScale * 4.0f / Mathf.Sqrt(1 + m * m); //Find second x intersection of boundary of collider with line
				float yIntersectTwo = m * xIntersectTwo + c; //Corresponding y

				Vector3 pointA = new Vector3(xIntersectOne, yIntersectOne, 0.0f); //Create point to represent first intersection
				Vector3 pointB = new Vector3(xIntersectTwo, yIntersectTwo, 0.0f); //And the second

				GameObject.Instantiate(MasterScript.systemInvasion.invasionQuad, pointA, Quaternion.identity); //This instantiates gameobjects to check the position of the intersections- these work okay
				GameObject.Instantiate(MasterScript.systemInvasion.invasionQuad, pointB, Quaternion.identity);

				float A2 = pointB.y - pointA.y; //Find equation of line between the two points DOES THIS WORK?
				float B2 = pointA.x - pointB.x;
				float C2 = (A2 * pointA.x) + (B2 * pointA.y);
				
				float determinant = (A1 * B2) - (A2 * B1);

				float x = ((B2 * C1) - (B1 * C2)) / determinant;
				float y = ((A1 * C2) - (A2 * C1)) / determinant;
				
				Vector3 intersection = new Vector3(x, y, 0f);

				GameObject.Instantiate(MasterScript.systemInvasion.invasionQuad, intersection, Quaternion.identity);

				float distanceOne = Vector3.Distance(pointA, intersection);
				float distanceTwo = Vector3.Distance(pointB, intersection);

				if(distanceOne < distanceTwo)
				{
					return new Vector2(pointA.x, pointA.y);
				}
				else
				{
					return new Vector2(pointB.x, pointB.y);
				}
			}
		}

		return Vector2.zero;
	}

	private void LaunchMissile() //Function used to launch missile
	{
		if(Input.GetMouseButtonDown(1) && target != -1) //Ensures target is selected when mouse clicked
		{
			tempPath.Clear ();

			GameObject missile = (GameObject)GameObject.Instantiate(missilePrefab, MasterScript.systemListConstructor.systemList[system].systemObject.transform.position, Quaternion.identity);

			curveBuilder = missile.GetComponent<QuadraticBezierCurve>();
			curveBuilder.target = target;

			tempPath.Add (missile.transform.position);

			bool pathfinding = true;

			Vector2 tempPos = missile.transform.position;
			
			while(pathfinding == true)
			{
				tempPos = FindTrajectoryIntersections(tempPos, MasterScript.systemListConstructor.systemList[target].systemObject.transform.position);
				
				if(tempPos == Vector2.zero)
				{
					tempPath.Add(MasterScript.systemListConstructor.systemList[curveBuilder.target].systemObject.transform.position);
					pathfinding = false;
					break;
				}

				else
				{
					tempPath.Add (tempPos);
				}
			}

			for(int i = 0; i < tempPath.Count; ++i)
			{
				curveBuilder.pathToFollow.Add (new Vector3(tempPath[i].x, tempPath[i].y, 0f));
			}

			curveBuilder.moving = true;
		}
	}

	private void UpdateInvadeLine()
	{
		if(Input.GetKeyDown("i") && system != -1) //If i pressed and system selected
		{
			if(MasterScript.systemListConstructor.systemList[system].systemOwnedBy == MasterScript.playerTurnScript.playerRace) //Make sure system is owned by player
			{
				bool ignore = false;
				
				if(currentPointer != null) //Reset pointer
				{
					GameObject.Destroy (currentPointer);
					GameObject.Destroy (currentLine);
					ignore = true;
				}
				
				if(currentPointer == null && ignore == false) //Set anchor points
				{
					anchorPoint = MasterScript.systemListConstructor.systemList[system].systemObject.transform.position;
					currentPointer = (GameObject)GameObject.Instantiate(invasionButton, anchorPoint, Quaternion.identity);
					currentLine = (GameObject)GameObject.Instantiate(invasionLine, anchorPoint, Quaternion.identity);
				}
			}
		}
		
		if(currentPointer != null) //If pointer is active
		{
			anchorPoint = MasterScript.systemListConstructor.systemList[system].systemObject.transform.position; //Update anchor
			Ray temp = Camera.main.ScreenPointToRay(Input.mousePosition);
			float angle = Vector3.Angle (Vector3.forward, temp.direction);
			float hyp = Camera.main.transform.position.z / (Mathf.Cos(angle * Mathf.Deg2Rad));
			currentPoint = temp.origin - temp.direction * hyp;
			currentPoint = new Vector3(currentPoint.x, currentPoint.y, 0f); //Update current point position
			bool foundNearbySystem = false;
			
			for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i) //Check if point is near a system
			{
				Vector3 sysPos = MasterScript.systemListConstructor.systemList[i].systemObject.transform.position;
				
				if(currentPoint.x < sysPos.x + 3f && currentPoint.x > sysPos.x - 3f)
				{
					if(currentPoint.y < sysPos.y + 3f && currentPoint.y > sysPos.y - 3f)
					{
						currentPoint = sysPos; //If it is, snap to target
						target = i;
						foundNearbySystem = true;
						break;
					}
				}
			}

			if(foundNearbySystem == false)
			{
				target = -1;
			}
			
			currentPointer.transform.position = currentPoint; 
			ResizeLine();
			RotateLine();
		}
	}

	private void Update () 
	{
		if(MasterScript.cameraFunctionsScript.selectedSystemNumber != -1)
		{
			system = MasterScript.cameraFunctionsScript.selectedSystemNumber;

			UpdateInvadeLine ();
			LaunchMissile();
		}
	}
}
