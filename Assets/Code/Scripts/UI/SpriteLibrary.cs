using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteLibrary", menuName = "KnightAfterShadows/SpriteLibrary")]
public class SpriteLibrary : ScriptableObject
{
    [Serializable]
    public struct SpriteMapping
    {
        public string key;
        public Sprite sprite;
    }

    [SerializeField] private List<SpriteMapping> mappings;

    public Sprite GetSprite(string key)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.key == key) return mapping.sprite;
        }
        return null;
    }
}
