using UnityEngine;
using System.Collections;
using System;

public static class MathsFunctions
{
	public static bool CheckPointIsCloseToPoint(Vector3 pointA, Vector3 pointB)
	{
		if(pointA.x - 0.001f <= pointB.x && pointA.x + 0.001f >= pointB.x)
		{
			if(pointA.y - 0.001f <= pointB.y && pointA.y + 0.001f >= pointB.y)
			{
				return true;
			}
		}
		
		return false;
	}

	public static float AreaOfTriangle(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
	{
		float determinant = vertexA.x * (vertexB.y - vertexC.y) + vertexB.x * (vertexC.y - vertexA.y) + vertexC.x * (vertexA.y - vertexB.y); //det = x1(y2-y3) + x2(y3-y1) + x3(y1-y2)
		return determinant / 2f; //area is half determinant
	}

	public static bool PointsAreColinear(Vector3 pointA, Vector3 pointB, Vector3 pointC)
	{
		float distanceAB = Vector3.Distance (pointA, pointB);
		float distanceBC = Vector3.Distance (pointB, pointC);
		float distanceCA = Vector3.Distance (pointC, pointA);

		float maxDist = Mathf.Max (distanceAB, distanceBC, distanceCA);
		float minDistA = 0f, minDistB = 0f;

		if(maxDist == distanceAB)
		{
			minDistA = distanceBC;
			minDistB = distanceCA;
		}

		if(maxDist == distanceBC)
		{
			minDistA = distanceAB;
			minDistB = distanceCA;
		}

		if(maxDist == distanceCA)
		{
			minDistA = distanceBC;
			minDistB = distanceAB;
		}

		if(maxDist + 0.01f >= minDistA + minDistB && maxDist - 0.01f <= minDistA + minDistB)
		{
			return true;
		}

		return false;
	}

	public static bool IsInTriangle(Vector3 vertexOne, Vector3 vertexTwo, Vector3 VertexThree, Vector3 point)
	{
		float originalArea = AreaOfTriangle(vertexOne, vertexTwo, VertexThree); //Get area of triangle
		float areaA = AreaOfTriangle(vertexOne, vertexTwo, point); //Get area of new triangle formed by a and b of original triangle and new point to be tested
		float areaB = AreaOfTriangle(vertexTwo, VertexThree, point); //Get area of new triangle formed by b and c of original triangle and new point to be tested
		float areaC = AreaOfTriangle(VertexThree, vertexOne, point); //Get area of new triangle formed by c and a of original triangle and new point to be tested
		
		float u = areaC / originalArea; //Calculate u of barycentric coordinates
		float v = areaA / originalArea; //Calculate v of barycentric coordinates
		float w = areaB / originalArea; //Calculate w of barycentric coordinate
		
		if(0f <= u && u <= 1) //If u is within 0 and 1
		{
			if(0f <= v && v <= 1) //If v is within 0 and 1
			{
				if(0f <= w && w <= 1) //If w is within 0 and 1
				{
					return true; //Point lies within triangle so return true
				}
			}
		}
		
		return false;
	}

	public static float RotationOfLine(Vector3 point, Vector3 origin)
	{
		float xDif = (float)Math.Round(point.x - origin.x, 3);
		float yDif = (float)Math.Round(point.y - origin.y, 3);
		float angle = Mathf.Atan (yDif / xDif);
		
		angle = angle * Mathf.Rad2Deg;
		
		if(xDif == 0f && yDif > 0f) //If x equals zero and y is positive the angle is 90 degrees (vertical up)
		{
			angle = 90f;
		}
		
		if(xDif == 0f && yDif < 0f) //If x equals zero and y is negative the angle is 270 degrees (vertical down)
		{
			angle = 270f;
		}
		
		if(yDif == 0f && xDif < 0f) //If y equals zero and x is negative the angle is 180 degrees (horizontal back)
		{
			angle = 180f;
		}
		
		if(yDif == 0f && xDif > 0f) //If y equals zero and x is positive then angle is 360 degrees (horizontal forward
		{
			angle = 0f;
		}
		
		if(xDif < 0f && yDif > 0f) //If x is negative and y is positive the angle is in the top left quadrant
		{
			angle = 180f + angle;
		}
		
		if(xDif < 0f && yDif < 0f) //If x is negative and y is negative the angle is in the bottom left quadrant
		{
			angle = 180f + angle;
		}
		
		if(xDif > 0f && yDif < 0f) //If x is positive and y is negative the angle is in the bottom right quadrant
		{
			angle = 360f + angle;
		}
		
		return angle;
	}

	public static Vector3 ABCLineEquation(Vector3 pointA, Vector3 pointB)
	{
		float A = pointB.y - pointA.y;
		float B = pointA.x - pointB.x;
		float C = (A * pointA.x) + (B * pointA.y);
		
		return new Vector3 (A, B, C);
	}

	public static Vector2 IntersectionOfTwoLines (Vector3 lineA, Vector3 lineB)
	{
		float determinant = (float)Math.Round((lineA.x * lineB.y) - (lineB.x * lineA.y), 3);
		
		if(determinant == 0f)
		{
			return Vector2.zero;
		}
		
		float x = (lineB.y * lineA.z - lineA.y * lineB.z) / determinant;
		float y = (lineA.x * lineB.z - lineB.x * lineA.z) / determinant;
		
		Vector2 intersection = new Vector3(x, y);
		
		return intersection;
	}

	public static Vector3 PerpendicularLineEquation(Vector3 systemA, Vector3 systemB)
	{
		Vector3 midpoint = (systemA + systemB) / 2;
		float gradient = (systemB.y - systemA.y) / (systemB.x - systemA.x);
		
		if(systemB.x - systemA.x < 0.0001f && systemB.x - systemA.x > -0.0001f)
		{
			return new Vector3(0, 1, midpoint.y);
		}
		
		if(systemB.y - systemA.y < 0.0001f && systemB.y - systemA.y > -0.0001f)
		{
			return new Vector3(1, 0, midpoint.x);
		}
		
		else
		{
			float perpGradient = -1/gradient;
			float yIntersect = midpoint.y - (perpGradient * midpoint.x);
			Vector3 perpSecondPoint = new Vector3(0, yIntersect, midpoint.z);
			return ABCLineEquation(midpoint, perpSecondPoint);
		}
	}

	public static bool PointLiesOnLine(Vector3 pointAVec3, Vector3 pointBVec3, Vector3 intersectionVec3)
	{	
		if(intersectionVec3.x - 0.001f <= Mathf.Max(pointAVec3.x, pointBVec3.x) && intersectionVec3.x + 0.001f >= Mathf.Min (pointAVec3.x, pointBVec3.x))
		{
			if(intersectionVec3.y - 0.001f <= Mathf.Max(pointAVec3.y, pointBVec3.y) && intersectionVec3.y + 0.001f >= Mathf.Min (pointAVec3.y, pointBVec3.y))
			{
				return true;
			}
		}
		
		return false;
	}

	public static float AngleBetweenLineSegments(Vector3 origin, Vector3 pointA, Vector3 pointB)
	{
		float angleA = MathsFunctions.RotationOfLine(pointA, origin);
		float angleB = MathsFunctions.RotationOfLine(pointB, origin);
		
		float testAngle = Mathf.Max (angleA, angleB) - Mathf.Min (angleA, angleB);
		
		if(testAngle > 180f)
		{
			testAngle = 360f - testAngle;
		}
		
		return testAngle;
	}
}
