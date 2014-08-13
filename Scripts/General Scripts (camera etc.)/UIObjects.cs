using UnityEngine;
using System.Collections;

public class UIObjects : MasterScript 
{
	public GameObject CreateConnectionLine(GameObject playerSys, GameObject enemySys)
	{
		float distance = Vector3.Distance(playerSys.transform.position, enemySys.transform.position);
		
		float rotationZRad = Mathf.Acos ((enemySys.transform.position.y - playerSys.transform.position.y) / distance);
		
		float rotationZ = rotationZRad * Mathf.Rad2Deg;
		
		if(playerSys.transform.position.x < enemySys.transform.position.x)
		{
			rotationZ = -rotationZ;
		}
		
		Vector3 rotation = new Vector3(0.0f, 0.0f, rotationZ);
		
		Vector3 midPoint = (playerSys.transform.position + enemySys.transform.position)/2;
		
		Vector3 scale = new Vector3(0.2f, distance, 0.0f);
		
		Quaternion directQuat = new Quaternion();
		
		directQuat.eulerAngles = rotation;
		
		GameObject line = (GameObject)Instantiate (heroGUI.merchantQuad, midPoint, directQuat);
		
		line.transform.localScale = scale;
		
		return line;
	}
}
