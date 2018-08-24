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
        [SerializeField] private ItemInstanceView _itemViewPrefab;
        [SerializeField] private Inventory _inventory;

        private List<ItemInstanceView> _itemViews;
        public Inventory Inventory 
        {
            get
            {
                return _inventory;
            }
            private set
            {
                _inventory = value;
            }
        }

        private void Start()
        {
            _itemViews = new List<ItemInstanceView>();
            for (int i = 0; i < Inventory.Capacity; i++)
            {
                var instance = Instantiate(_itemViewPrefab, _itemsContent);
                instance.SetItem(Inventory.Items[i]);
            }
        }

    }
}