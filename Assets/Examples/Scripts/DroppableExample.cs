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
			var apple = new ItemInstance(Resources.Load<ItemType>("Items/Apple"));
			inventory.Add(apple);
        }
    }
}