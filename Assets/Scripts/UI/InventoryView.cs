using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

namespace GInventory
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemsContent;

        private int _visualCapacity;
        private List<ItemInstanceView> _itemInstanceViews;

        public void Initialize(int capacity)
        {
            var itemInstancePrefab = Resources.Load<GameObject>("Prefabs/ItemInstanceView");

            _itemInstanceViews = new List<ItemInstanceView>();
            for (int i = 0; i < capacity; i++)
            {
                var itemInstance = Instantiate(itemInstancePrefab, _itemsContent);
                _itemInstanceViews.Add(itemInstance.GetComponent<ItemInstanceView>());
            }
            _visualCapacity = capacity;
        }

        public bool Add(ItemInstance item)
        {
            var existing = _itemInstanceViews.Where(x => x.Item != null).FirstOrDefault(x => x.Item.ItemType == item.ItemType);
            if(existing != null)
            {
                var overflow = existing.Item.Add(item.Quantity.Value);
                if (overflow == 0)
                    return true;
                else
                    item.Quantity.Value = overflow;
            }

            var empty = _itemInstanceViews.FirstOrDefault(x => x.IsEmpty);
            if(empty == null)
            {
                return false;
            }

            empty.SetItem(item);
            return true;
        }

    }
}