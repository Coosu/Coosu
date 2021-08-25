using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    public class SpriteGroup : ISceneObject, ISpriteHost
    {
        static SpriteGroup()
        {
            ObjectType.SignType(10, "SpriteGroup");
        }

        public SpriteGroup()
        {
        }

        public SpriteGroup(double defaultX, double defaultY, double defaultZ, OriginType origin)
        {
            Camera2.DefaultX = defaultX;
            Camera2.DefaultY = defaultY;
            Camera2.DefaultZ = defaultZ;
            Camera2.OriginType = origin;
        }

        public ObjectType ObjectType { get; } = 10;
        public int? RowInSource { get; set; }
        public async Task WriteHeaderAsync(TextWriter writer)
        {
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
        }

        public object Clone()
        {
            return new SpriteGroup
            {
                EnableGroupedSerialization = EnableGroupedSerialization,
                Camera2 = (Camera2)Camera2.Clone(),
                RowInSource = RowInSource,
                Sprites = Sprites.Select(k => k.Clone()).Cast<Sprite>().ToList()
            };

        }

        public double MaxTime =>
            NumericHelper.GetMaxValue(
                Events.Select(k => k.EndTime)
            );
        public double MinTime =>
            NumericHelper.GetMinValue(
                Events.Select(k => k.StartTime)
            );
        public double MaxStartTime =>
            NumericHelper.GetMaxValue(
                Events.Select(k => k.StartTime)
            );
        public double MinEndTime =>
            NumericHelper.GetMinValue(
                Events.Select(k => k.EndTime)
            );

        public bool EnableGroupedSerialization { get; set; }

        public ICollection<ICommonEvent> Events
        {
            get => Camera2.Events;
            set => Camera2.Events = value;
        }

        public void AddEvent(ICommonEvent @event)
        {
            Camera2.AddEvent(@event);
        }

        public double DefaultX
        {
            get => Camera2.DefaultX;
            set => Camera2.DefaultX = value;
        }

        public double DefaultY
        {
            get => Camera2.DefaultY;
            set => Camera2.DefaultY = value;
        }

        public double DefaultZ
        {
            get => Camera2.DefaultZ;
            set => Camera2.DefaultZ = value;
        }

        public string CameraIdentifier
        {
            get => Camera2.CameraIdentifier;
            set => Camera2.CameraIdentifier = value;
        }

        #region ISpriteHost

        public Camera2 Camera2 { get; private set; } = new();

        public ICollection<Sprite> Sprites { get; private set; } = new List<Sprite>();

        public IEnumerator<Sprite> GetEnumerator()
        {
            return Sprites.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}