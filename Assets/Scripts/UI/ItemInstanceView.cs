using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using System;

namespace GInventory
{
    public class ItemInstanceView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _quantityLabel;
        [SerializeField] private Image _icon;
        [SerializeField] private ClickableObject _button;
        public CanvasGroup _canvasGroup; // todo: rename
        private IDisposable _quantityDisposable;
        private IDisposable _iconDisposable;
        public bool IsEmpty
        {
            get
            {
                return Item.ItemType.Value == null;
            }
        }

        public ItemInstance Item { get; private set; }

        private void Awake()
        {
            var inventoryManager = FindObjectOfType<InventoryClickManager>();
            _button.OnLeftClick.AddListener(() =>
            {
                inventoryManager.HandleClick(this);
            });
            _button.OnRightClick.AddListener(() =>
            {
                inventoryManager.HandleRightClick(this);
            });
            _quantityLabel.gameObject.SetActive(false);
        }

        public void SetItem(ItemInstance item)
        {
            Item = item;
            if (item.ItemType.Value == null)
            {
                _icon.enabled = false;
                _quantityLabel.gameObject.SetActive(false);
            }
            TryDispose(_iconDisposable);
            TryDispose(_quantityDisposable);
            _iconDisposable = Item.ItemType.TakeWhile((x) => Item != null).TakeUntilDestroy(this).Subscribe(type =>
            {
                if (type == null)
                {
                    _icon.enabled = false;
                    _quantityLabel.gameObject.SetActive(false);
                }
                else if (Item.ItemType.Value.Icon != null)
                {
                    _icon.enabled = true;
                    _icon.sprite = Item.ItemType.Value.Icon;
                }
                else
                {
                    _icon.enabled = false;
                }
            });
            _quantityDisposable = Item.Quantity.TakeWhile((x) => Item != null).TakeUntilDestroy(this).Subscribe(quantity =>
            {
                if (quantity == 0)
                {
                    _quantityLabel.gameObject.SetActive(false);
                    _icon.enabled = false;
                }
                else
                {
                    _quantityLabel.gameObject.SetActive(true);
                    _quantityLabel.text = quantity.ToString();
                }
            });
        }

        private void TryDispose(IDisposable d)
        {
            if (d != null)
            {
                d.Dispose();
            }
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