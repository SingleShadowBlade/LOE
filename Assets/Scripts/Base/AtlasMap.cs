using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasMap : ScriptableObject
{
    public List<int> spriteNames;
    public UnityEngine.Sprite[] sprites;

    public Sprite GetSprite(string name)
    {
        int nameInt = Animator.StringToHash(name);
        Sprite sp = null;
        int idx = spriteNames.IndexOf(nameInt);
        if (idx >= 0 && idx < sprites.Length)
            sp = sprites[idx];
        return sp;
    }

    public int IndexOf(string name)
    {
        int nameInt = Animator.StringToHash(name);
        int idx = spriteNames.IndexOf(nameInt);
        return idx;
    }

    /// <summary>
    /// sprite 个数
    /// </summary>
    public int Length
    {
        get
        {
            if (sprites != null)
                return sprites.Length;
            else
                return 0;
        }
    }

    public void Dispose()
    {
        spriteNames = null;
        sprites = null;
    }
}