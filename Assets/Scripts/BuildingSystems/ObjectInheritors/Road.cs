using System;
using UnityEngine;
using Object = BuildingSystems.Base.Object;

namespace BuildingSystems.ObjectInheritors
{
    public class Road : Object
    {
        public Action<Sprite> SetRoadSprite;

        private void Awake()
        {
            SetRoadSprite = SetSprite;
        }

        private void SetSprite(Sprite sprite)
        {
            renderer.sprite = sprite;
        }

        public override void SetConfiguration(IGridObject gridObject, int id = 0)
        {
            base.SetConfiguration(gridObject, id);
            _id = id;
        }
    }
}