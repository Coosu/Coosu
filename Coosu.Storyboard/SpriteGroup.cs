using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard
{
    public sealed class SpriteGroup : ISceneObject, ISpriteHost
    {
        static SpriteGroup()
        {
            ObjectType.SignType(10, "SpriteGroup");
        }

        public SpriteGroup()
        {
        }

        public SpriteGroup(float defaultX, float defaultY, float defaultZ, OriginType origin)
        {
            Camera2.DefaultX = defaultX;
            Camera2.DefaultY = defaultY;
            Camera2.DefaultZ = defaultZ;
            Camera2.OriginType = origin;
        }

        public ObjectType ObjectType { get; } = 10;
        public int? RowInSource { get; set; }
        public object Tag { get; set; }

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

        public float MaxTime() =>
            NumericHelper.GetMaxValue(
                Events.Select(k => k.EndTime)
            );
        public float MinTime() =>
            NumericHelper.GetMinValue(
                Events.Select(k => k.StartTime)
            );
        public float MaxStartTime() =>
            NumericHelper.GetMaxValue(
                Events.Select(k => k.StartTime)
            );
        public float MinEndTime() =>
            NumericHelper.GetMinValue(
                Events.Select(k => k.EndTime)
            );

        public bool EnableGroupedSerialization { get; set; }

        public ICollection<IKeyEvent> Events
        {
            get => Camera2.Events;
            set => Camera2.Events = value;
        }

        public void AddEvent(IKeyEvent @event)
        {
            Camera2.AddEvent(@event);
        }

        /// <inheritdoc />
        public float DefaultX
        {
            get => Camera2.DefaultX;
            set => Camera2.DefaultX = value;
        }

        /// <inheritdoc />
        public float DefaultY
        {
            get => Camera2.DefaultY;
            set => Camera2.DefaultY = value;
        }

        /// <inheritdoc />
        public float DefaultZ
        {
            get => Camera2.DefaultZ;
            set => Camera2.DefaultZ = value;
        }

        /// <inheritdoc />
        public string CameraIdentifier
        {
            get => Camera2.CameraIdentifier;
            set => Camera2.CameraIdentifier = value;
        }

        #region ISpriteHost

        public Camera2 Camera2 { get; private set; } = new();
        public void AddSprite(Sprite sprite)
        {
            Sprites.Add(sprite);
        }

        public void AddSubHost(ISpriteHost spriteHost)
        {
            SubHosts.Add(spriteHost);
        }

        public ISpriteHost? BaseHost { get; set; }

        public IList<Sprite> Sprites { get; private set; } = new List<Sprite>();
        public IList<ISpriteHost> SubHosts { get; private set; } = new List<ISpriteHost>();

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