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
            var cloak = new ItemInstance(Resources.Load<ItemType>("Items/Cloak"));
            var ironIngot = new ItemInstance(Resources.Load<ItemType>("Items/IronIngot"));
            var ring = new ItemInstance(Resources.Load<ItemType>("Items/Ring"));
            var shield = new ItemInstance(Resources.Load<ItemType>("Items/Shield"));
            //_inventoryA.Initialize(132);
            //_inventoryB.Initialize(132);

            _inventoryA.Add(apple);
            _inventoryA.Add(apple);
        }
    }
}