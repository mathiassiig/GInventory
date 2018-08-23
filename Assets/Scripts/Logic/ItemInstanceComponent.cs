using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GInventory
{
    public class ItemInstanceComponent : MonoBehaviour
    {
        public ItemInstance ItemInstance { get; private set; }
        public void Init(ItemInstance itemInstance)
        {
			ItemInstance = itemInstance;
        }
    }
}