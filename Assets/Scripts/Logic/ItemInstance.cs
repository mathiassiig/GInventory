﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace GInventory
{
    public class ItemInstance
    {
        public ReactiveProperty<int> Quantity = new ReactiveProperty<int>();
        public ItemType ItemType;

        public ItemInstance(ItemInstance i)
        {
            ItemType = i.ItemType;
            Quantity.Value = i.Quantity.Value;
        }
        
        public ItemInstance(ItemType type)
        {
            ItemType = type;
            Quantity.Value = 1;
        }

        public bool CanAddAll(int toAdd)
        {
            if(ItemType.MaxQuantityInStack == 0)
            {
                return true;
            }
            var suggestedQuantity = Quantity.Value + toAdd;
            if(suggestedQuantity > ItemType.MaxQuantityInStack)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add a quantity to the stack
        /// </summary>
        /// <param name="toAdd">The amount of items to add to the stack</param>
        /// <returns>If there is an overflow, i.e. more items are added to the stack than is allowed, the overflow is returned</returns>
        public virtual int Add(int toAdd)
        {
            var suggestedQuantity = Quantity.Value + toAdd;
            Quantity.Value = Mathf.Clamp(suggestedQuantity, 0, ItemType.MaxQuantityInStack == 0 ? int.MaxValue : ItemType.MaxQuantityInStack);
            var overflow = Mathf.Clamp(suggestedQuantity - ItemType.MaxQuantityInStack, 0, int.MaxValue);
            return overflow;
        }

        /// <summary>
        /// Remove a quantity from the stack
        /// </summary>
        /// <param name="toRemove">The amount of items to remove from the stack</param>
        /// <returns>If there is an underflow, i.e. the stack goes negative, the underflow is returned</returns>
        public virtual int Remove(int toRemove)
        {
            var quantityValue = Quantity.Value - toRemove;
            if (quantityValue < 0)
            {
                var underflow = quantityValue;
                Quantity.Value = 0;
                return underflow;
            }
            Quantity.Value = quantityValue;
            return 0;
        }
    }
}