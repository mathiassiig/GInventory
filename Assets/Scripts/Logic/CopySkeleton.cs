using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopySkeleton : MonoBehaviour
{
    public SkinnedMeshRenderer Character;

    void Start()
    {
        SkinnedMeshRenderer targetRenderer = Character.GetComponent<SkinnedMeshRenderer>();
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in targetRenderer.bones)
        {
            boneMap[bone.name] = bone;
        }

        SkinnedMeshRenderer thisRenderer = GetComponent<SkinnedMeshRenderer>();
        Transform[] boneArray = thisRenderer.bones;
        for (int idx = 0; idx < boneArray.Length; ++idx)
        {
            string boneName = boneArray[idx].name;
            if (false == boneMap.TryGetValue(boneName, out boneArray[idx]))
            {

            }
        }
        thisRenderer.bones = boneArray;
    }
}
