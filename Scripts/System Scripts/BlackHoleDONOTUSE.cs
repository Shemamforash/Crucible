using UnityEngine;
using System.Collections;

public class BlackHoleDONOTUSE : MonoBehaviour 
{
	private float g = 6.7384f;

	void Update () 
	{
		for(int i = 0; i < MasterScript.systemListConstructor.systemList.Count; ++i)
		{
			float force = g * gameObject.rigidbody.mass * 100 * MasterScript.systemListConstructor.systemList[i].systemObject.rigidbody.mass;
			float distance = Vector3.Distance (gameObject.transform.position, MasterScript.systemListConstructor.systemList[i].systemObject.transform.position);
			force = force / Mathf.Pow(distance, 2f);
			MasterScript.systemListConstructor.systemList[i].systemObject.rigidbody.AddForce((gameObject.transform.position - MasterScript.systemListConstructor.systemList[i].systemObject.transform.position) * force * Time.smoothDeltaTime);
		}
	}
}
