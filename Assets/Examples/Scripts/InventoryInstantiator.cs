using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GInventory;
using System;

[System.Serializable]
public class ItemInstanceInstantiationTask
{
    public ItemType ItemType;
    public int Quantity = 1;
}

public class InventoryInstantiator : MonoBehaviour
{
    public List<ItemInstanceInstantiationTask> Instantiations;
	public Inventory TargetInventory; 

    private void Start()
    {
		foreach(var task in Instantiations)
		{
            var instance = new ItemInstance(task.ItemType, task.Quantity);
			TargetInventory.Add(instance);
		}
    }
}
