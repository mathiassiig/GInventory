using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GInventory
{
    public class ItemDeleter : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var item = other.gameObject.GetComponent<ItemInstanceComponent>();
			if(item != null)
			{
				Destroy(item.gameObject);
			}
        }
    }
}