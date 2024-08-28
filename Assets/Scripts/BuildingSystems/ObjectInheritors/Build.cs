using UnityEngine;
using Object = BuildingSystems.Base.Object;

namespace BuildingSystems.ObjectInheritors
{
    public class Build : Object
    {
        private static int _idCounter; //возможно так себе идея

        public override void SetConfiguration(IGridObject gridObject, int id =0)
        {
            base.SetConfiguration(gridObject);
            _id = _idCounter++;
            renderer.sprite = (Sprite)gridObject.GetRenderObject();
        }
    }
}