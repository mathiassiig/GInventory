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
        [SerializeField] protected TextMeshProUGUI _quantityLabel;
        [SerializeField] protected Image _icon;
        [SerializeField] protected ClickableObject _button;
        public CanvasGroup _canvasGroup; // todo: rename
        protected IDisposable _quantityDisposable;
        protected IDisposable _typeDisposable;
        public bool IsEmpty
        {
            get
            {
                return Item.ItemType.Value == null;
            }
        }
        protected ItemInstance _item;
        public ItemInstance Item
        {
            get
            {
                if (_item == null)
                {
                    _item = new ItemInstance();
                    SetItem(_item);
                }
                return _item;
            }
            protected set
            {
                _item = value;
            }
        }

        private void Awake()
        {
            Setup();
        }

        public virtual bool CanMove(ItemInstance i)
        {
            return true;
        }

        protected void Setup()
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

        protected void ResetDisposables()
        {
            TryDispose(_typeDisposable);
            TryDispose(_quantityDisposable);
        }

        protected void CheckForNull()
        {
            if (Item.ItemType.Value == null)
            {
                _icon.enabled = false;
                _quantityLabel.gameObject.SetActive(false);
            }
        }

        protected virtual void SetupTypeDisposable()
        {
            _typeDisposable = Item.ItemType.TakeWhile((x) => Item != null).TakeUntilDestroy(this).Subscribe(type =>
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
        }

        protected virtual void SetupQuantityDisposable()
        {
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

        public virtual void SetItem(ItemInstance item)
        {
            ResetDisposables();
            if (_item == null)
            {
                Item = item;
            }
            Item.Set(item);
            CheckForNull();
            SetupTypeDisposable();
            SetupQuantityDisposable();
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