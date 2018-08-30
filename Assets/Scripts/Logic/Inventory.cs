using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GInventory
{
    public class Inventory : MonoBehaviour
    {
        public List<ItemInstance> Items;
        public int Capacity;

        void Awake()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            Items = new List<ItemInstance>();
            for (int i = 0; i < Capacity; i++)
            {
                Items.Add(new ItemInstance());
            }
        }

        public bool Add(ItemInstance item)
        {
            var existing = Items.Where(x => x != null).Where(x => x.ItemType.Value == item.ItemType.Value && x.Quantity.Value < x.ItemType.Value.MaxQuantityInStack).FirstOrDefault();
            if (existing != null)
            {
                var overflow = existing.Add(item.Quantity.Value);
                if (overflow == 0)
                    return true;
                else
                    item.Quantity.Value = overflow;
            }
            var empty = Items.FirstOrDefault(x => x.ItemType.Value == null);
            if (empty == null)
            {
                return false;
            }
            empty.Set(item);
            return true;
        }
    }
}