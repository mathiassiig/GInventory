using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GInventory
{
    [CreateAssetMenu(fileName = "GInventorySettings", menuName = "GInventory/Settings")]
    public class GInventorySettings : ScriptableObject
    {
        [Tooltip("If set to false, the quantity string will not show for item views where there is only 1 in the stack")]
        public bool ShowQuantityLabelIfSingle;
    }

    public static class InventorySettingsManager
    {
        private static GInventorySettings _settings;
        public static GInventorySettings Settings
        {
            get
            {
                if(_settings == null)
                {
                    _settings = Resources.Load<GInventorySettings>("Settings/GInventorySettings");
                    if(_settings == null)
                    {
                        Debug.LogError("Create a GInventorySettings file and place it under 'Resources/Settings'");
                    }
                }
                return _settings;
            }
        }
    }
}