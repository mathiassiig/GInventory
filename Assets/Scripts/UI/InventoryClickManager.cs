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
        private ItemInstanceView _originalLiftedItem;
        private ItemInstanceView _clonedLiftedItem;
        private float _heightDrop = 0.25f;
        private bool _lifting;
        private bool _hoveringUI = false;
        private Action OnCancel;
        private Action<ItemInstanceView> OnTryLiftEnd;
        private Action<ItemInstance> OnComplete;

        public void HandleClick(ItemInstanceView item)
        {
            if (!_lifting && !item.IsEmpty)
            {
                Lift(item);
            }
            else if (_lifting && item == _originalLiftedItem)
            {
                OnCancel();
            }
            else if (_lifting)
            {
                OnTryLiftEnd(item);
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

        private bool Move(ItemInstance from, ItemInstance to)
        {
            if (from != null && to != null && from.ItemType.Value == to.ItemType.Value)
            {
                var overflow = to.Add(from.Quantity.Value);
                from.Quantity.Value = overflow;
                if (from.Quantity.Value == 0)
                {
                    from.Clear();
                }
                return true;
            }
            else
            {
                var fromType = from.ItemType.Value;
                var fromQuantity = from.Quantity.Value;

                from.Quantity.Value = to.Quantity.Value;
                from.ItemType.Value = to.ItemType.Value;

                to.Quantity.Value = fromQuantity;
                to.ItemType.Value = fromType;
                return true;
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
                OnTryLiftEnd = (target) =>
                {
                    var copy = new ItemInstance(_originalLiftedItem.Item.ItemType.Value, amountToLift);
                    bool success = target.Item.Set(copy);
                    if (success)
                    {
                        FinishLift();
                    }
                    else
                    {
                        OnCancel();
                    }
                };
                OnComplete = (target) =>
                {
                    var copy = new ItemInstance(_originalLiftedItem.Item.ItemType.Value, amountToLift);
                    bool success = target.Set(copy);
                    if (success)
                    {
                        FinishLift();
                    }
                    else
                    {
                        OnCancel();
                    }
                };
            }
            else
            {
                OnCancel = CancelLift;
                OnTryLiftEnd = (target) =>
                {
                    bool canMove = target.CanMove(_originalLiftedItem.Item);
                    if (canMove)
                    {
                        OnComplete(target.Item);
                    }
                    else
                    {
                        OnCancel();
                    }
                };
                OnComplete = (target) =>
                {
                    Move(_originalLiftedItem.Item, target);
                    FinishLift();
                };
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
                var instantiateIndividuals = _clonedLiftedItem.Item.ItemType.Value.InstantiateIndividuals;
                var count = instantiateIndividuals ? _clonedLiftedItem.Item.Quantity.Value : 1;
                var prefab = _clonedLiftedItem.Item.ItemType.Value.Prefab;
                // copy the item over
                var itemInstanceTarget = new ItemInstance();
                OnComplete(itemInstanceTarget);
                for (int i = 0; i < count; i++)
                {
                    var instance = Instantiate(prefab, position + new Vector3(0, _heightDrop * i, 0), Quaternion.identity);
                    var itemInstanceComponent = instance.AddComponent<ItemInstanceComponent>();
                    var individualCopy = instantiateIndividuals ? new ItemInstance(itemInstanceTarget) : itemInstanceTarget;
                    if (instantiateIndividuals)
                    {
                        individualCopy.Quantity.Value = 1;
                    }
                    itemInstanceComponent.Init(individualCopy);
                }
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
                    OnCancel();
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