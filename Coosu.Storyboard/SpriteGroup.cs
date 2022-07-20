using System;
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

        private ICollection<IKeyEvent> _events = new SortedSet<IKeyEvent>(EventSequenceComparer.Instance);
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

        public double MaxTime() =>
            NumericHelper.GetMaxValue(
                Events.Select(k => k.EndTime)
            );
        public double MinTime() =>
            NumericHelper.GetMinValue(
                Events.Select(k => k.StartTime)
            );
        public double MaxStartTime() =>
            NumericHelper.GetMaxValue(
                Events.Select(k => k.StartTime)
            );
        public double MinEndTime() =>
            NumericHelper.GetMinValue(
                Events.Select(k => k.EndTime)
            );

        public bool EnableGroupedSerialization { get; set; }

        public IReadOnlyCollection<IKeyEvent> Events
        {
            get => (IReadOnlyCollection<IKeyEvent>)_events;
            internal set => _events = value as ICollection<IKeyEvent> ?? throw new Exception(
                $"The collection should be {nameof(ICollection<IKeyEvent>)}");
        }

        IReadOnlyCollection<IKeyEvent> IEventHost.Events => Events;
        
        public void AddEvent(IKeyEvent @event)
        {
            Camera2.AddEvent(@event);
        }

        public bool RemoveEvent(IKeyEvent @event)
        {
            return Camera2.RemoveEvent(@event);
        }

        public void ResetEventCollection(IComparer<IKeyEvent>? comparer = null)
        {
            Camera2.ResetEventCollection(comparer);
        }


        /// <inheritdoc />
        public double DefaultX
        {
            get => Camera2.DefaultX;
            set => Camera2.DefaultX = value;
        }

        /// <inheritdoc />
        public double DefaultY
        {
            get => Camera2.DefaultY;
            set => Camera2.DefaultY = value;
        }

        /// <inheritdoc />
        public double DefaultZ
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