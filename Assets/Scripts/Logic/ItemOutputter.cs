using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GInventory
{
    public class ItemOutputter : MonoBehaviour
    {
        public Vector3 LocalOutputPosition;
        public Inventory Inventory;

        public void Output()
        {
            var last = Inventory.Items.Where(x => x != null && x.ItemType.Value != null).LastOrDefault();
            if (last != null)
            {
                var type = last.ItemType.Value;
                last.Remove(1);
                var instance = Instantiate(type.Prefab, transform.position + LocalOutputPosition, Quaternion.identity);
                var itemInstanceComponent = instance.AddComponent<ItemInstanceComponent>();
                itemInstanceComponent.Init(new ItemInstance(type));
            }

        }
    }
}