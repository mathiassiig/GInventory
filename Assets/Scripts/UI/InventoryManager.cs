using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace GInventory
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _mainCanvas;
        private ItemInstanceView _originalLiftedItem;
        private ItemInstanceView _clonedLiftedItem;
        private bool _lifting;

        public void HandleClick(ItemInstanceView item)
        {
            if (!_lifting && !item.IsEmpty)
            {
                Lift(item);
            }
            else
            {
                Move(_originalLiftedItem, item);
                CancelLift();
            }
        }

        private void Move(ItemInstanceView from, ItemInstanceView to)
        {
            var fromItem = from.Item;
            var toItem = to.Item;
            to.SetItem(fromItem);
            from.SetItem(toItem);
        }

        public void Lift(ItemInstanceView original)
        {
            _originalLiftedItem = original;
            _clonedLiftedItem = Instantiate(_originalLiftedItem).GetComponent<ItemInstanceView>();
            _clonedLiftedItem._canvasGroup.blocksRaycasts = false;
            _clonedLiftedItem._canvasGroup.interactable = false;
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

        private void CancelLift()
        {
            _lifting = false;
            Destroy(_clonedLiftedItem.gameObject);
            _clonedLiftedItem = null;
            _originalLiftedItem._canvasGroup.alpha = 1;
            _originalLiftedItem = null;
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