using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class QuadraticBezierCurve : MasterScript
{
	public bool moving = false;
	public int target, currentVertex = 0;
	public List<Vector3> pathToFollow = new List<Vector3> ();
	private SystemRotate rotate;

	void Start()
	{
		rotate = systemListConstructor.systemList [target].systemObject.GetComponent<SystemRotate> ();
	}

	private void AdjustPathValues()
	{
		double angle = -rotate.speed * Mathf.Deg2Rad;
		
		float xPos = (float)(Math.Cos(angle) * (pathToFollow[currentVertex].x - 50f) - Math.Sin(angle) * (pathToFollow[currentVertex].y - 50f) + 50f);
		float yPos = (float)(Math.Sin(angle) * (pathToFollow[currentVertex].x - 50f) + Math.Cos(angle) * (pathToFollow[currentVertex].y - 50f) + 50f);
		
		pathToFollow[currentVertex] = new Vector3 (xPos, yPos, 0f);
	}

	private bool ReachedNextVertex(int vertex)
	{
		if(gameObject.transform.position.x < pathToFollow[vertex].x + 0.1f)
		{
			if(gameObject.transform.position.x > pathToFollow[vertex].x - 0.1f)
			{
				if(gameObject.transform.position.y < pathToFollow[vertex].y + 0.1f)
				{
					if(gameObject.transform.position.y > pathToFollow[vertex].y - 0.1f)
					{
						return true; //Check to see if it has reached the next point on its path
					}
				}
			}
		}

		return false;
	}

	public void Update()
	{
		if(moving == true)
		{
			//FaceTarget();
			gameObject.transform.LookAt(pathToFollow[currentVertex]);
			AdjustPathValues();

			for(int j = currentVertex; j < pathToFollow.Count; ++j)
			{
				bool reachedPoint = ReachedNextVertex(j);	
				
				if(j + 1 == pathToFollow.Count && reachedPoint == true) //If it has reached final point
				{
					systemDefence = systemListConstructor.systemList[target].systemObject.GetComponent<SystemDefence>(); //Get reference to target system
					systemDefence.TakeDamage(500f, 0f, -1); //Force system to take damage
					GameObject.Destroy (gameObject); //Destroy gameobject
				}
				else if(j + 1 != pathToFollow.Count && reachedPoint == true) //If it has not reached final point but has reached a point
				{
					++currentVertex; //Increase the current point
					break;
				}
				else if (reachedPoint == false) //If it has not reached a point
				{
					gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, pathToFollow[currentVertex], 2f * Time.deltaTime); //Move towards next point
				}
			}
		}
	}
}

