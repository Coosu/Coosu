using System;
using System.Collections.Generic;
using OSharp.Storyboard.Events;
using OSharp.Storyboard.Management;

namespace OSharp.Storyboard.Camera
{
    class Camera2D
    {
        private readonly IReadOnlyList<Element> _objects;
        public Element[] NewObjects { get; set; }
        public int Fps { get; set; } = 15;

        public Camera2D(params Element[] objects)
        {
            this._objects = objects;
        }

        public Camera2D(ElementGroup group)
        {
            this._objects = group.ElementList;
        }

        public void Translate(EasingType easing, float startTime, float endTime, float x, float y)
        {
            throw new NotImplementedException();
        }

        public void Rotate(EasingType easing, float startTime, float endTime, float deg)
        {
            throw new NotImplementedException();
        }

        public void Scale(EasingType easing, float startTime, float endTime, float sx, float sy)
        {
            throw new NotImplementedException();
        }
    }
}
