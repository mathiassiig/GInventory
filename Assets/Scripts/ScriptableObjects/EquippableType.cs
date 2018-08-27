using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GInventory
{
    public enum EquippableSlotType
    {
        Head,
        Chest,
        Legs,
        Feet
    }

    public enum BodyTarget
    {
        Head
    }

    [System.Serializable]
    public class EquippableInstanceModel
    {
        public GameObject Prefab;
        public BodyTarget Target;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
        public Vector3 LocalScale;

    }

    [CreateAssetMenu(fileName = "New equippable", menuName = "GInventory/Equippable")]
    public class EquippableType : ItemType
    {
        public EquippableSlotType Slot;
        public List<EquippableInstanceModel> Models;
        
        public override bool CanMove(ItemInstance itemInstance)
        {
            var asEquippable = itemInstance.ItemType.Value as EquippableType;
            if(itemInstance.ItemType.Value == null)
            {
                return true;
            }
            if(asEquippable != null)
            {
                if(asEquippable.Slot == Slot)
                {
                    return true;
                }
            }
            return false;
        }
    }
}