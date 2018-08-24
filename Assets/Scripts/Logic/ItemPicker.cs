using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GInventory
{
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject>
    {
    }

    public class ItemPicker : MonoBehaviour
    {
        public Inventory TargetInventory;
        public GameObjectEvent OnCannotInput;
        private void OnTriggerEnter(Collider other)
        {
            var item = other.gameObject.GetComponent<ItemInstanceComponent>();
            if (item != null)
            {
                if (TargetInventory.Add(item.ItemInstance))
                {
                    Destroy(item.gameObject);
                }
                else
                {
                    if (OnCannotInput != null)
                    {
                        OnCannotInput.Invoke(other.gameObject);
                    }
                }
            }
        }
    }
}