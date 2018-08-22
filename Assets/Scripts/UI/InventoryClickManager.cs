using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace GInventory
{
    public class InventoryClickManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _mainCanvas;
        private ItemInstanceView _originalLiftedItem;
        private ItemInstanceView _clonedLiftedItem;
        private bool _lifting;
        private Action OnCancel;
        private Action<ItemInstanceView> OnComplete;

        public void HandleClick(ItemInstanceView item)
        {
            if (!_lifting && !item.IsEmpty)
            {
                Lift(item);
            }
            else
            {
                OnComplete(item);
                FinishLift();
            }
        }

        public void HandleRightClick(ItemInstanceView item)
        {
            if (!_lifting && !item.IsEmpty)
            {
                Lift(item, true);
            }
            else if(_lifting)
            {
                OnCancel();
            }
        }

        private void Move(ItemInstanceView from, ItemInstanceView to)
        {
            var fromItem = from.Item;
            var toItem = to.Item;
            if(fromItem != null && toItem != null && fromItem.ItemType == toItem.ItemType)
            {
                var overflow = toItem.Add(fromItem.Quantity.Value);
                fromItem.Quantity.Value = overflow;
            }
            else // swap
            {
                to.SetItem(fromItem);
                from.SetItem(toItem);
            }
        }

        public void Lift(ItemInstanceView original, bool half = false)
        {
            _originalLiftedItem = original;
            _clonedLiftedItem = Instantiate(_originalLiftedItem).GetComponent<ItemInstanceView>();
            _clonedLiftedItem._canvasGroup.blocksRaycasts = false;
            _clonedLiftedItem._canvasGroup.interactable = false;
            _clonedLiftedItem.SetItem(new ItemInstance(_originalLiftedItem.Item));
            if(half && _originalLiftedItem.Item.Quantity.Value > 1)
            {
                var amountToLift = original.Item.Quantity.Value / 2;
                var amountLeft = original.Item.Quantity.Value - amountToLift;
                original.Item.Quantity.Value = amountLeft;
                _clonedLiftedItem.Item.Quantity.Value = amountToLift;
                OnCancel = () =>
                {
                    original.Item.Quantity.Value = amountToLift + amountLeft;
                    CancelLift();
                };
                OnComplete = (target) =>
                {
                    target.SetItem(new ItemInstance(_originalLiftedItem.Item));
                    target.Item.Quantity.Value = amountToLift;
                };
            }
            else
            {
                OnCancel = CancelLift;
                OnComplete = (target) => Move(_originalLiftedItem, target);
            }
            original._canvasGroup.alpha = 0.33f;
            _clonedLiftedItem.transform.SetParent(_mainCanvas);
            _clonedLiftedItem.transform.localScale = Vector3.one;
            var rect = _clonedLiftedItem.transform as RectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            _lifting = true;
            Observable.EveryUpdate().TakeWhile((x) => _lifting).Subscribe(x =>
            {
                var mousePos = Input.mousePosition;
                Vector2 rectPos = Vector2.zero;

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainCanvas, mousePos, null, out rectPos))
                {
                    rect.anchoredPosition = rectPos;
                }
            });
        }

        private void FinishLift()
        {
            _lifting = false;
            Destroy(_clonedLiftedItem.gameObject);
            _clonedLiftedItem = null;
            _originalLiftedItem._canvasGroup.alpha = 1;
            _originalLiftedItem = null;
        }

        private void CancelLift()
        {
            FinishLift();
        }

        private void Update()
        {
            if (_lifting && Input.GetMouseButtonDown(1))
            {
                CancelLift();
            }
        }
    }
}