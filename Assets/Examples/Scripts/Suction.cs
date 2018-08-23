using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suction : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
		var rb = other.gameObject.GetComponent<Rigidbody>();
		var direction = (transform.position - rb.transform.position).normalized;
		rb.AddForce(direction);
    }
}
