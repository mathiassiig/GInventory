using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GInventory
{
    [System.Serializable]
    public class BodyTransformPair
    {
        public BodyTarget BodyName;
        public Transform TransformTarget;
    }
    public class CharacterEquipment : MonoBehaviour
    {
        public List<BodyTransformPair> BodyDictionary;
        private Dictionary<EquippableSlotType, List<GameObject>> _bodyInstances = new Dictionary<EquippableSlotType, List<GameObject>>();
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;

        public void Unset(EquippableSlotType slot)
        {
            List<GameObject> list;
            if (_bodyInstances.TryGetValue(slot, out list))
			{
                Unset(list);
            }
        }
        public void Set(EquippableType item)
        {
            List<GameObject> list;
            if (_bodyInstances.TryGetValue(item.Slot, out list))
			{
                Unset(list);
            }
            var slotInstances = new List<GameObject>();
            _bodyInstances[item.Slot] = slotInstances;
            if (item.Rigged)
            {
                AddModel(item.RiggedModel, slotInstances);
            }
            else
            {
                foreach (var model in item.StaticModels)
                {
                    AddModel(model, slotInstances);
                }
            }
        }

        private void AddModel(GameObject riggedModel, List<GameObject> list)
        {
            var modelInstance = Instantiate(riggedModel, transform);
            var skeletonCopier = modelInstance.GetComponentInChildren<CopySkeleton>();
            if(skeletonCopier == null)
            {
                var mesh = modelInstance.GetComponentInChildren<SkinnedMeshRenderer>();
                skeletonCopier = mesh.gameObject.AddComponent<CopySkeleton>();
            }
            skeletonCopier.Character = _meshRenderer;
            list.Add(modelInstance);
        }

        private void AddModel(EquippableInstanceModel model, List<GameObject> list)
        {
            var pair = BodyDictionary.FirstOrDefault(x => x.BodyName == model.Target);
            if (pair != null && pair.TransformTarget != null)
            {
                var modelInstance = Instantiate(model.Prefab, pair.TransformTarget);
                modelInstance.transform.localRotation = Quaternion.Euler(model.LocalRotation);
                modelInstance.transform.localPosition = model.LocalPosition;
                modelInstance.transform.localScale = model.LocalScale;
                list.Add(modelInstance);
            }
        }

        public void Unset(List<GameObject> instances)
        {
            foreach (var model in instances)
            {
                Destroy(model);
            }
            instances = null;
        }
    }
}