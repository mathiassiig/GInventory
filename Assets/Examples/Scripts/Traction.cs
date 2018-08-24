using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traction : MonoBehaviour
{
	public Vector3 Velocity;
	public bool KeepUpRight = true;
    private void OnTriggerStay(Collider other)
    {
		var rb = other.gameObject.GetComponent<Rigidbody>();
		rb.velocity = Velocity;
		if(KeepUpRight)
		{
			rb.gameObject.transform.localRotation = Quaternion.identity;
			rb.angularVelocity = Vector3.zero;
		}
    }
}
