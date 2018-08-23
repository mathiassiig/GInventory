using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GInventory
{
    public class InventoryClickManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform _mainCanvas;
        [SerializeField] private LayerMask _droppableLayerMask;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        private ItemInstanceView _originalLiftedItem;
        private ItemInstanceView _clonedLiftedItem;
        private float _heightDrop = 0.25f;
        private bool _lifting;
        private bool _hoveringUI = false;
        private Action OnCancel;
        private Action<ItemInstance> OnComplete;

        public void HandleClick(ItemInstanceView item)
        {
            if (!_lifting && !item.IsEmpty)
            {
                Lift(item);
            }
            else if (_lifting)
            {
                OnComplete(item.Item);
                FinishLift();
            }
        }

        public void HandleRightClick(ItemInstanceView item)
        {
            if (!_lifting && !item.IsEmpty)
            {
                Lift(item, true);
            }
            else if (_lifting)
            {
                OnCancel();
            }
        }

        private void Move(ItemInstance from, ItemInstance to)
        {
            if (from != null && to != null && from.ItemType.Value == to.ItemType.Value)
            {
                var overflow = to.Add(from.Quantity.Value);
                from.Quantity.Value = overflow;
            }
            else // swap
            {
                var fromCopy = new ItemInstance(from);
                from.Quantity.Value = to.Quantity.Value;
                from.ItemType.Value = to.ItemType.Value;

                to.Quantity.Value = fromCopy.Quantity.Value;
                to.ItemType.Value = fromCopy.ItemType.Value;
            }
        }



        public void Lift(ItemInstanceView original, bool half = false)
        {
            _originalLiftedItem = original;
            _clonedLiftedItem = Instantiate(_originalLiftedItem).GetComponent<ItemInstanceView>();
            _clonedLiftedItem._canvasGroup.blocksRaycasts = false;
            _clonedLiftedItem._canvasGroup.interactable = false;
            _clonedLiftedItem.SetItem(new ItemInstance(_originalLiftedItem.Item));
            _clonedLiftedItem.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64); // todo; hacky
            if (half && _originalLiftedItem.Item.Quantity.Value > 1)
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
                    target.ItemType.Value = _originalLiftedItem.Item.ItemType.Value;
                    target.Quantity.Value = amountToLift;
                };
            }
            else
            {
                OnCancel = CancelLift;
                OnComplete = (target) => Move(_originalLiftedItem.Item, target);
            }
            original._canvasGroup.alpha = 0.33f;
            _clonedLiftedItem.transform.SetParent(_mainCanvas);
            _clonedLiftedItem.transform.localScale = Vector3.one;
            var rect = _clonedLiftedItem.transform as RectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            MoveRectToMouse(rect);

            _lifting = true;
            Observable.EveryUpdate().TakeWhile((x) => _lifting).Subscribe(x =>
            {
                MoveRectToMouse(rect);
            });
        }

        private void MoveRectToMouse(RectTransform rect)
        {
            var mousePos = Input.mousePosition;
            Vector2 rectPos = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainCanvas, mousePos, null, out rectPos))
            {
                rect.anchoredPosition = rectPos;
            }
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

        public void DropItem(Vector3 position)
        {
            if (_clonedLiftedItem.Item.ItemType.Value.Prefab != null)
            {
                var instance = Instantiate(_clonedLiftedItem.Item.ItemType.Value.Prefab, position, Quaternion.identity);
                var itemInstanceComponent = instance.AddComponent<ItemInstanceComponent>();
                var itemInstanceTarget = new ItemInstance();
                OnComplete(itemInstanceTarget);
                itemInstanceComponent.Init(itemInstanceTarget);
                FinishLift();
            }
        }

        private void TryDrop()
        {
            var mousePos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, float.MaxValue, _droppableLayerMask))
            {
                DropItem(raycastHit.point + Vector3.up * _heightDrop);
            }
        }

        void Update()
        {
            if (_lifting)
            {
                if (Input.GetMouseButtonDown(0) && !_hoveringUI)
                {
                    TryDrop();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    CancelLift();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hoveringUI = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hoveringUI = false;
        }
    }
}