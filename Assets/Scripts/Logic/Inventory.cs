using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GInventory
{
    public class Inventory : MonoBehaviour
    {
        private List<ItemInstance> _items;
        public int _capacity;

        public void Initialize(int capacity)
        {
            _capacity = capacity;
            _items = new List<ItemInstance>();
            for (int i = 0; i < _capacity; i++)
            {
                var itemInstance = new ItemInstance();
            }
        }

        public bool Add(ItemInstance item)
        {
            var existing = _items.Where(x => x != null).FirstOrDefault(x => x.ItemType == item.ItemType);
            if (existing != null)
            {
                var overflow = existing.Add(item.Quantity.Value);
                if (overflow == 0)
                    return true;
                else
                    item.Quantity.Value = overflow;
            }

            var empty = _items.FirstOrDefault(x => x.ItemType == null);
            if (empty == null)
            {
                return false;
            }

            //empty.SetItem(item);
            return true;
        }
    }
}