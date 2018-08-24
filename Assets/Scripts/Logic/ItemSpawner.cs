using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GInventory
{
    public class ItemSpawner : MonoBehaviour
    {
		[SerializeField] private ItemType _itemToSpawn;
		 public Vector3 LocalSpawnPosition;

		public void Spawn()
		{
			var prefab = _itemToSpawn.Prefab;
			var instance = Instantiate(prefab, transform.position + LocalSpawnPosition, Quaternion.identity);
			var itemInstanceComponent = instance.AddComponent<ItemInstanceComponent>();
			itemInstanceComponent.Init(new ItemInstance(_itemToSpawn));
		}
    }
}