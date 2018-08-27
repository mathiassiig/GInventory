using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GInventory
{
    [CustomEditor(typeof(ItemType))]
    public class ItemTypeEditor : Editor
    {
        ItemType _target;

        private void OnEnable()
        {
            _target = (ItemType)target;
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            Texture2D newIcon = new Texture2D(width, height);

            if (_target.Icon != null)
            {
                EditorUtility.CopySerialized(_target.Icon.texture, newIcon);
                return newIcon;
            }
            else
            {
                Texture2D defaultCustomIcon = AssetDatabase.LoadAssetAtPath("Assets/WinxProduction/Editor/Editor Default Resources/StateMachine Icon.png", typeof(Texture2D)) as Texture2D;

                if (defaultCustomIcon != null)
                {
                    EditorUtility.CopySerialized(defaultCustomIcon, newIcon);
					var sprite = Sprite.Create(newIcon, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                    _target.Icon = sprite;

                    AssetDatabase.AddObjectToAsset(sprite, _target);

                    EditorUtility.SetDirty(_target);

                    return newIcon;
                }
            }

            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }
    }
}