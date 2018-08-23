using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GInventory.Examples
{
    public class MultiInventoryExample : MonoBehaviour
    {

        [SerializeField] private InventoryView _inventoryA;
        [SerializeField] private InventoryView _inventoryB;

        void Start()
        {
            var apple = new ItemInstance(Resources.Load<ItemType>("Items/Apple"));

            _inventoryA.Inventory.Add(apple);
            _inventoryA.Inventory.Add(apple);
        }
    }
}