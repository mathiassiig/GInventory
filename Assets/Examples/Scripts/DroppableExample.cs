using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GInventory
{
    public class DroppableExample : MonoBehaviour
    {
        void Start()
        {
			var inventory = GetComponent<Inventory>();
			var appleType = Resources.Load<ItemType>("Items/Apple");
			for (int i = 0; i < 20; i++)
			{
				inventory.Add(new ItemInstance(appleType));
			}
        }
    }
}