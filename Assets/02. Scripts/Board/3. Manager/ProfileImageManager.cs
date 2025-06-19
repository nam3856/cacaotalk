using System.Collections.Generic;
using UnityEngine;

public class ProfileImageManager : MonoBehaviourSingleton<ProfileImageManager>
{
    public List<Sprite> profileSprites; // Index 0~4

    public Sprite GetProfileSprite(int index)
    {
        if (index < 0 || index >= profileSprites.Count)
            return profileSprites[0]; // 기본값
        return profileSprites[index];
    }
}
