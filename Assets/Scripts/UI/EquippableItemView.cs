using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GInventory
{
    public class EquippableItemView : ItemInstanceView
    {
        [SerializeField] private Image _placeholderIcon;
        public CharacterEquipment CharacterTarget;
        public EquippableSlotType SlotType;
        private void Awake()
        {
            Setup();
        }

        public override bool CanMove(ItemInstance item)
        {
            if(item.IsEmpty)
            {
                return true;
            }
            var asEquippable = item.ItemType.Value as EquippableType;
            if (asEquippable != null)
            {
                if (asEquippable.Slot == SlotType)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void SetupTypeDisposable()
        {
            CharacterTarget.Unset(SlotType);
            _typeDisposable = Item.ItemType.TakeWhile((x) => Item != null).TakeUntilDestroy(this).Subscribe(type =>
            {
                if (type == null)
                {
                    CharacterTarget.Unset(SlotType);
                    _placeholderIcon.gameObject.SetActive(true);
                    _icon.enabled = false;
                    _quantityLabel.gameObject.SetActive(false);
                }
                else
                {
                    var equippable = type as EquippableType;
                    if(equippable != null)
                    {
                        CharacterTarget.Set(equippable);
                    }
                    if (Item.ItemType.Value.Icon != null)
                    {
                        _placeholderIcon.gameObject.SetActive(false);
                        _icon.enabled = true;
                        _icon.sprite = Item.ItemType.Value.Icon;
                    }
                    else
                    {
                        _icon.enabled = false;
                    }
                }
            });
        }
    }
}