using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	[SerializeField] private Vector3 _worldPos;

	public void TeleportGameobject(GameObject g)
	{
		g.transform.position = _worldPos;
	}
}
