using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensibility
{
    public abstract class ActionHandler<T> : IActionParsingHandler<T> where T : CommonEvent
    {
        public abstract string Flag { get; }
        public abstract int ParameterDimension { get; }

        CommonEvent IActionParsingHandler.Deserialize(string[] split)
        {
            return Deserialize(split);
        }
        
        string IActionParsingHandler.Serialize(CommonEvent raw)
        {
            return Serialize((T)raw);
        }

        string IParsingHandler.Serialize(object raw)
        {
            return Serialize((T)raw);
        }

        object IParsingHandler.Deserialize(string[] split)
        {
            return Deserialize(split);
        }

        public abstract T Deserialize(string[] split);
        public abstract string Serialize(T raw);
    }
}