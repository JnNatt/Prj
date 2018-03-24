using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureMapper : MonoBehaviour
{

    public List<Sprite> imageList;

    public List<TimepointPictures> mapping;
	
    [Serializable]
    public class TimepointPictures
    {
        public Sprite icon;
        public Sprite detailPicture;
    }

    public static readonly Dictionary<int, TimepointPictures> PictureMapping = new Dictionary<int, TimepointPictures>();

    void Start()
    {
        if (PictureMapping.Count == 0)
        {
            for (var index = 0; index < mapping.Count; index++)
            {
                var item = mapping[index];
                PictureMapping.Add(index, item);
            }
        }
        
    }

    public static Sprite GetIconPic(int timepointId)
    {
        if (PictureMapping.ContainsKey(timepointId))
        {
            return PictureMapping[timepointId].icon;
        }
        return null;
    }
    public static Sprite GetDetailPic(int timepointId)
    {
        if (PictureMapping.ContainsKey(timepointId))
        {
            return PictureMapping[timepointId].detailPicture;
        }
        return null;
    }
}
