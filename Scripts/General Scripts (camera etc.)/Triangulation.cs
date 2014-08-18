using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class Triangulation : MasterScript 
{
	public List<Triangle> triangles = new List<Triangle> ();
	private List<Triangle> tempTri = new List<Triangle>();
	private List<GameObject> unvisitedStars = new List<GameObject> ();
	private List<GameObject> externalPoints = new List<GameObject> ();
	public bool start, iterate = false;
	public float timer = -1f;
	public int iterator = 0;

	public void Update()
	{
		if(systemListConstructor.loaded == true && iterator == 0)
		{
			start = true;
		}

		if(start == true)
		{
			triangles.Clear();
			tempTri.Clear();
			unvisitedStars.Clear ();
			externalPoints.Clear ();

			CacheNearestStars (); //First add all the star systems to a list
			
			Triangle newTri = new Triangle(); //Create a new triangle
			newTri.points.Add (unvisitedStars [0]); //Add the first 3 ordered points from the centre
			newTri.points.Add (unvisitedStars [1]);
			newTri.points.Add (unvisitedStars[2]);
			
			newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position)); //Calculate the line equations between the points
			newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position));
			newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position));
			
			externalPoints.Add (unvisitedStars [0]); //Add the points to the external points list
			externalPoints.Add (unvisitedStars [1]);
			externalPoints.Add (unvisitedStars [2]);
			
			triangles.Add (newTri); //Add the triangle to the triangle list
			
			unvisitedStars.RemoveRange(0, 3); //Remove the points from the unvisited points list

			start = false;
			iterate = true;

			timer = Time.time;
		}

		if(start == false && iterate == true)
		{
				if(iterator < unvisitedStars.Count)
				{
					LinkPointToTris(iterator);
					CacheTempTris(iterator);
					DrawDebugTriangles();
					++iterator;
					timer = Time.time;
				}

				if(iterator == unvisitedStars.Count)
				{
					DrawDebugTriangles();
					iterate = false;
				}
		}
	}

	private void DrawDebugTriangles()
	{
		for(int i = 0; i < triangles.Count; ++i)
		{
			voronoiGenerator.DrawDebugLine(triangles[i].points[0].transform.position, triangles[i].points[1].transform.position, turnInfoScript.selkiesMaterial);
			voronoiGenerator.DrawDebugLine(triangles[i].points[1].transform.position, triangles[i].points[2].transform.position, turnInfoScript.selkiesMaterial);
			voronoiGenerator.DrawDebugLine(triangles[i].points[2].transform.position, triangles[i].points[0].transform.position, turnInfoScript.selkiesMaterial);
		}
	}

	public void SimpleTriangulation() //This function controls the triangulation and conversion to delaunay of the stars
	{
		CacheNearestStars (); //First add all the star systems to a list
		
		Triangle newTri = new Triangle(); //Create a new triangle
		newTri.points.Add (unvisitedStars [0]); //Add the first 3 ordered points from the centre
		newTri.points.Add (unvisitedStars [1]);
		newTri.points.Add (unvisitedStars[2]);

		newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[0].transform.position, newTri.points[1].transform.position)); //Calculate the line equations between the points
		newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[1].transform.position, newTri.points[2].transform.position));
		newTri.lines.Add (MathsFunctions.ABCLineEquation (newTri.points[2].transform.position, newTri.points[0].transform.position));

		externalPoints.Add (unvisitedStars [0]); //Add the points to the external points list
		externalPoints.Add (unvisitedStars [1]);
		externalPoints.Add (unvisitedStars [2]);

		triangles.Add (newTri); //Add the triangle to the triangle list

		unvisitedStars.RemoveRange(0, 3); //Remove the points from the unvisited points list

		for(int i = 0; i < unvisitedStars.Count; ++i) //For all unchecked points
		{
			LinkPointToTris(i); //Link this unvisited point to all possible external points
			CacheTempTris(i); //Add all the triangles formed by this linking to the triangle list
		}
			
		bool isDelaunay = false; //Say that the list isn't delaunay

		while(isDelaunay == false) //While it isn't delaunay
		{
			isDelaunay = voronoiGenerator.TriangulationToDelaunay (); //Make it delaunay, if the method returns true, the triangulation is delaunay and the while loop will stop
		}

		CheckForNonDelaunayTriangles (); //Debugging method outputs the number of non-delaunay triangles to the log. This has not output any bad values for some time.
	}
	
	private void CacheNearestStars() //Used to add the star systems to a list of points and sort them NO ISSUES HERE
	{
		for(int i = 0; i < systemListConstructor.systemList.Count; ++i) //Add all systems to a list of unvisited nodes
		{
			unvisitedStars.Add (systemListConstructor.systemList[i].systemObject);
		}

		float theta = 0f; //Set a variable to represent angle

		while(theta < 360f) //While the angle is less than 360 degrees
		{
			float xPos = 50f + 70f * Mathf.Cos (theta * Mathf.Deg2Rad); //Get the xposition of a point 60 units from the centre of the map at the desired angle from 0 degrees.
			float yPos = 50f + 70f * Mathf.Sin (theta * Mathf.Deg2Rad); //Get the yposition

			Vector3 newPos = new Vector3 (xPos, yPos, 0f); //Create a new vector3 for this position
			
			GameObject edgePoint = new GameObject(); //Create a new gameobject called edgepoint
			edgePoint.name = ((int)(theta / 15f)).ToString(); //Set the name to the angle over 15 (this will make the points have names 0-23)
			edgePoint.transform.position = newPos; //Set the position of the gameobject to the previously calculated position
			unvisitedStars.Add (edgePoint); //This adds a circular boundary to the points so that the voronoi cells are calculated properly
			theta += 15f; //Move onto the next point on the circle
		}
		
		Vector3 centre = new Vector3 (50f, 50f, 0f); //Create centre point at middle of map
		
		for(int j = unvisitedStars.Count; j > 0; --j) //For all unvisited stars
		{
			bool swapsMade = false; //Say no swaps have been made
			
			for(int k = 1; k < j; ++k) //While k is less than j (anything above current j value is sorted)
			{
				float distanceA = Vector3.Distance (unvisitedStars[k].transform.position, centre); //Check distance to centre
				float distanceB = Vector3.Distance (unvisitedStars[k - 1].transform.position, centre); //Check distance to centre
				
				if(distanceA < distanceB) //Sort smallest to largest
				{
					GameObject temp = unvisitedStars[k]; //Swap the points
					unvisitedStars[k] = unvisitedStars[k - 1];
					unvisitedStars[k - 1] = temp;
					swapsMade = true; //A swap has been made
				}
			}
			
			if(swapsMade == false) //If no swaps made, list must have been sorted
			{
				break; //So break
			}
		}
	}

	private void LinkPointToTris(int curPoint) //Send the current unvisited star as the seed point SOMETHING WRONG
	{
		for(int i = 0; i < externalPoints.Count; ++i) //For all the external points
		{
			int nextPoint = i + 1; //Assign the next point

			if(nextPoint == externalPoints.Count) //If the next point is out of range
			{
				nextPoint = 0; //Set it to the first point in the list
			}

			Vector3 lineCurToExternal = MathsFunctions.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[i].transform.position); //Create a line between the external point and the unvisited star

			if(IsIllegalIntersection(i, curPoint, lineCurToExternal)) //If there is an illegal intersection between the external point-current point line and the external point-unvisited point line
			{
				continue; //Move onto the next external point
			}

			Vector3 lineCurToNextExternal = MathsFunctions.ABCLineEquation(unvisitedStars[curPoint].transform.position, externalPoints[nextPoint].transform.position);

			if(IsIllegalIntersection(nextPoint, curPoint, lineCurToNextExternal)) //If there is an illegal intersection between the next point-current point line and the external point-unvisited point line
			{
				continue; //Move onto the next external point
			}

			bool illegal = false; //Say that the line is not illegal

			for(int j = 0; j < externalPoints.Count; ++j) //Check through all other external points
			{
				if(j == i || j == nextPoint)
				{
					continue;
				}

				if(MathsFunctions.IsInTriangle(externalPoints[i].transform.position, externalPoints[nextPoint].transform.position, unvisitedStars[curPoint].transform.position, externalPoints[j].transform.position) == true) //If the point lies in any of the triangles
				{
					//if(MathsFunctions.PointsAreColinear(externalPoints[i].transform.position, externalPoints[nextPoint].transform.position, externalPoints[j].transform.position) == false)
					//{
						//if(MathsFunctions.PointsAreColinear(externalPoints[i].transform.position, unvisitedStars[curPoint].transform.position, externalPoints[j].transform.position) == false)
						//{
							//if(MathsFunctions.PointsAreColinear(externalPoints[nextPoint].transform.position, unvisitedStars[curPoint].transform.position, externalPoints[j].transform.position) == false)
							//{
								illegal = true; //It is an illegal triangle
								break; //
							//}
						//}
					//}
				}
			}

			if(illegal) //If it's an illegal triangle
			{
				continue; //Skip to the next point
			}

			GameObject pointA = externalPoints[i]; //Otherwise assign the points of the triangle
			GameObject pointB = externalPoints[nextPoint];
			GameObject pointC = unvisitedStars[curPoint];

			Triangle newTri = new Triangle(); //Create a new triangle object
			newTri.points.Add (pointA); //Add the points
			newTri.points.Add (pointB);
			newTri.points.Add (pointC);
			newTri.lines.Add (MathsFunctions.ABCLineEquation (pointA.transform.position, pointB.transform.position)); //Calculate the line equations between the points
			newTri.lines.Add (MathsFunctions.ABCLineEquation (pointB.transform.position, pointC.transform.position));
			newTri.lines.Add (MathsFunctions.ABCLineEquation (pointC.transform.position, pointA.transform.position));
			tempTri.Add (newTri); //Add the triangle to the temptriangle list
		}
	}

	public List<GameObject> CheckIfSharesSide(Triangle triOne, Triangle triTwo) //Just return the points
	{
		List<GameObject> pointList = new List<GameObject> (); //New empty list of points

		for(int i = 0; i < 3; ++i) //For all points in tri one
		{
			for(int j = 0; j < 3; ++j) //For all points in tri two
			{
				if(triOne.points[i].name == triTwo.points[j].name) //If tri one shares a point with tri two
				{
					pointList.Add(triOne.points[i]); //Add the trione point
					break; //Go to the next point in tri one
				}
			}
		}

		if(pointList.Count == 2) //If there are 2 shared points (i.e an edge)
		{
			for(int i = 0; i < 3; ++i) //For all points in the triangles
			{
				if(triOne.points[i].name != pointList[0].name && triOne.points[i].name != pointList[1].name) //Find the unshared points in triOne
				{
					pointList.Add(triOne.points[i]); //Add them to the list
				}
				if(triTwo.points[i].name != pointList[0].name && triTwo.points[i].name != pointList[1].name) //Find the unshared points in triTwo
				{
					pointList.Add (triTwo.points[i]); //Add them to the list
				}
			}
		}
	
		return pointList; //Return the list of points
	}

	private void CheckForNonDelaunayTriangles()
	{
		List<Triangle> numberofnondelaunay = new List<Triangle>();

		for(int i = 0; i < triangles.Count; ++i)
		{
			for(int j = 0; j < triangles.Count; ++j)
			{
				if(i == j)
				{
					continue;
				}

				List<GameObject> sharedSides = CheckIfSharesSide(triangles[i], triangles[j]);

				if(sharedSides.Count == 4)
				{
					GameObject sharedPointA = sharedSides[0];
					GameObject sharedPointB = sharedSides[1];
					GameObject unsharedPointA = sharedSides[2];
					GameObject unsharedPointB = sharedSides[3];
					float angleAlpha = 0f, angleBeta = 0f;
					
					angleAlpha = MathsFunctions.AngleBetweenLineSegments (unsharedPointA.transform.position, sharedPointA.transform.position, sharedPointB.transform.position);

					angleBeta = MathsFunctions.AngleBetweenLineSegments (unsharedPointB.transform.position, sharedPointA.transform.position, sharedPointB.transform.position);
					
					Vector3 sharedPointLine = MathsFunctions.ABCLineEquation (sharedPointA.transform.position, sharedPointB.transform.position);
					Vector3 unsharedPointLine = MathsFunctions.ABCLineEquation (unsharedPointA.transform.position, unsharedPointB.transform.position);
					Vector2 intersection = MathsFunctions.IntersectionOfTwoLines (sharedPointLine, unsharedPointLine);
					
					if(MathsFunctions.PointLiesOnLine(sharedPointA.transform.position, sharedPointB.transform.position, intersection) == false) //Is non-convex
					{
						continue;
					}
				
					if(angleBeta + angleAlpha > 180)
					{
						numberofnondelaunay.Add (triangles[i]);
					}
				}
			}
		}

		for(int i = 0; i < numberofnondelaunay.Count; ++i)
		{
			for(int j = 0; j < 3; ++j)
			{
				int a = j;
				int b = j + 1;

				if(j + 1 == 3)
				{
					b = 0;
				}
			}
		}

		Debug.Log(numberofnondelaunay.Count + " | " + triangles.Count);
	}

	private void CacheTempTris(int curPoint) //Used to cache all the tris formed by connecting the current unvisited point to available external points
	{
		for(int i = 0; i < tempTri.Count; ++i) //For all temporary triangles
		{
			triangles.Add (tempTri[i]); //Add the triangle to the triangle list
		}

		if(tempTri.Count == 0) //If there are no temporary triangles
		{
			Debug.Log ("wut" + unvisitedStars[curPoint]); //Something must have gone wrong so debug the unvisited point
		}

		externalPoints.Add (unvisitedStars [curPoint]); //Add the unvisted star to the external points (it must be external)
		CheckInteriorPoints (); //Check to see if any other points have become internal
		SortExternalPoints (); //Sort the external points clockwise
		tempTri.Clear (); //Clear the temporary triangles list
	}

	private void SortExternalPoints() //Used to sort the external points clockwise from the centre of the map
	{
		for(int j = externalPoints.Count; j > 0; --j) //For all external points
		{
			bool swapsMade = false; //Say no swaps have been made
			
			for(int k = 1; k < j; ++k) //While k is less than j (anything above current j value is sorted)
			{
				float angleK = MathsFunctions.RotationOfLine(new Vector3(50f, 50f, 0f), externalPoints[k].transform.position); //Get the angle the current external point makes with the centre
				float angleKMinus1 = MathsFunctions.RotationOfLine(new Vector3(50f, 50f, 0f), externalPoints[k - 1].transform.position); //Get the angle the previous point makes with the centre

				if(angleK < angleKMinus1) //Sort smallest to largest 
				{
					GameObject tempExternal = externalPoints[k]; //Create a temporary gameobject to store the point
					externalPoints[k] = externalPoints[k - 1]; //And swap them
					externalPoints[k - 1] = tempExternal;
					swapsMade = true; //A swap has been made
				}
			}
			
			if(swapsMade == false) //If no swaps made, list must have been sorted
			{
				break; //So break
			}
		}
	}

	private bool IsIllegalIntersection(int externalPoint, int unvisitedPoint, Vector3 curToExtLine)
	{
		for(int j = 0; j < triangles.Count; ++j) //For every triangle
		{
			if(triangles[j].isInternal == true) //If the triangle is internal we don't need to worry about it, so move onto the next triangle
			{
				continue;
			}

			for(int k = 0; k < 3; ++k) //For each line in that triangle
			{
				GameObject pointA = null; //Get the points at each end of the line
				GameObject pointB = null;
				
				if(k == 0)
				{
					pointA = triangles[j].points[0];
					pointB = triangles[j].points[1];
				}
				if(k == 1)
				{
					pointA = triangles[j].points[1];
					pointB = triangles[j].points[2];
				}
				if(k == 2)
				{
					pointA = triangles[j].points[2];
					pointB = triangles[j].points[0];
				}

				if(pointA.name == externalPoints[externalPoint].name || pointB.name == externalPoints[externalPoint].name) //If the line contains the external point we can ignore it
				{
					continue;
				}

				Vector2 intersection = MathsFunctions.IntersectionOfTwoLines(curToExtLine, triangles[j].lines[k]); //Get the intersection of the line with the current star to external point line
				
				if(MathsFunctions.PointLiesOnLine(externalPoints[externalPoint].transform.position, unvisitedStars[unvisitedPoint].transform.position, intersection)) //If the point lies elsewhere on the line
				{
					if(MathsFunctions.PointLiesOnLine(pointA.transform.position, pointB.transform.position, intersection))
					{
						return true; //This IS an illegal intersection so return that, otherwise keep checking
					}
				}
			}
		}
		
		return false;
	}

	private void CheckInteriorPoints() //Checks through external point list to see if any points have become internal ones (are within the polygon formed by linking triangles
	{
		List<int> pointsToRemove = new List<int> ();
		
		for(int i = 0; i < externalPoints.Count; ++i)
		{
			float tempAngle = 0;
			
			for(int j = 0; j < triangles.Count; ++j)
			{
				if(triangles[j].isInternal)
				{
					continue;
				}

				for(int k = 0; k < 3; ++k)
				{
					if(triangles[j].points[k] == externalPoints[i])
					{
						int a = 0, b = 0;

						if(k == 0)
						{
							a = 1; b = 2;
						}
						if(k == 1)
						{
							a = 0; b = 2;
						}
						if(k == 2)
						{
							a = 0; b = 1;
						}

						tempAngle += MathsFunctions.AngleBetweenLineSegments(triangles[j].points[k].transform.position, triangles[j].points[a].transform.position, triangles[j].points[b].transform.position);

						break;
					}
				}
			}

			if(Math.Round(tempAngle, 2) >= 360f)
			{
				pointsToRemove.Add (i);
			}
		}

		for(int j = 0; j < triangles.Count; ++j) //For all triangles
		{
			bool isInternalTri = true; //Say its internal
			
			for(int k = 0; k < 3; ++k) //For all points in that triangle
			{
				if(externalPoints.Contains (triangles[j].points[k])) //If one of them is external
				{
					isInternalTri = false; //The tri isn't internal
				}
			}
			
			if(isInternalTri)
			{
				triangles[j].isInternal = true;
			}
		}

		int counter = 0;

		for(int i = 0; i < pointsToRemove.Count; ++i)
		{
			externalPoints.RemoveAt(pointsToRemove[i] - counter);
			++counter;
		}
	}
}

public class Triangle
{
	public List<GameObject> points = new List<GameObject>(); //A, B, C points
	public List<Vector3> lines = new List<Vector3>(); //AB, BC, CA line equations
	public bool isInternal = false; //This allows me to ignore any triangles with no external points
}
