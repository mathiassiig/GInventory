using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GInventory
{
    public class ItemPicker : MonoBehaviour
    {
        public Inventory TargetInventory;
        private void OnTriggerEnter(Collider other)
        {
			var item = other.gameObject.GetComponent<ItemInstanceComponent>();
			if(item != null)
			{
				TargetInventory.Add(item.ItemInstance);
				Destroy(item.gameObject);
			}
        }
    }
}