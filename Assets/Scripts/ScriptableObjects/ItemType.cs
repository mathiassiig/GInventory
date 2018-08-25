using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GInventory
{
    [CreateAssetMenu(fileName = "New item", menuName = "GInventory/ItemType")]
    public class ItemType : ScriptableObject
    {
        public string Title;
        public string Description;
        public Sprite Icon; // image to be used, for example in an inventory
        public GameObject Prefab; // if the game allows spawning of items, which gameobject should be created from this type
        public int MaxQuantityInStack; // 0 for unlimited
        public bool InstantiateIndividuals; // if true, it will instantiate 1 prefab instance per quantity

        public virtual bool CanMove(ItemInstance i)
        {
            return true;
        }
    }
}