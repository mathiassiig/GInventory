using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;

namespace GInventory
{
    public class ItemInstanceView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _quantityLabel;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _button;
        public CanvasGroup _canvasGroup; // todo: rename
        public bool IsEmpty
        {
            get
            {
                return Item == null;
            }
        }

        public ItemInstance Item { get; private set; }

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                FindObjectOfType<InventoryManager>().HandleClick(this);
            });
            _quantityLabel.gameObject.SetActive(false);
        }


        public void SetItem(ItemInstance item)
        {
            Item = item;
            if(item == null)
            {
                _icon.enabled = false;
                _quantityLabel.gameObject.SetActive(false);
                return;
            }
            if(Item.ItemType.Icon != null)
            {
                _icon.enabled = true;
                _icon.sprite = Item.ItemType.Icon;
            }
            else
            {
                _icon.enabled = false;
            }
            _quantityLabel.gameObject.SetActive(true);
            Item.Quantity.TakeWhile((x) => Item != null).Subscribe(quantity =>
            {
                if(quantity == 0)
                {
                    Item = null;
                }
                else
                {
                    _quantityLabel.text = quantity.ToString();
                }
            });
        }

        public void UnsetItem()
        {
            Item = null;
            _icon.sprite = null;
            _icon.enabled = false;
            _quantityLabel.text = "";
            _quantityLabel.gameObject.SetActive(false);
        }

    }
}