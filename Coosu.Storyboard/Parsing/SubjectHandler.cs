using System.Collections.Generic;

namespace Coosu.Storyboard.Parsing
{
    public abstract class SubjectHandler<T> : ISubjectParsingHandler<T> where T : EventContainer
    {
        public abstract string Flag { get; }
        EventContainer ISubjectParsingHandler.Deserialize(string[] split)
        {
            return Deserialize(split);
        }

        object IParsingHandler.Deserialize(string[] split)
        {
            return Deserialize(split);
        }

        string ISubjectParsingHandler.Serialize(EventContainer raw)
        {
            return Serialize((T)raw);
        }

        string IParsingHandler.Serialize(object raw)
        {
            return Serialize((T)raw);
        }

        public abstract string Serialize(T raw);

        public abstract T Deserialize(string[] split);

        public IParsingHandler RegisterAction(IActionParsingHandler handler)
        {
            _actionHandlerDic.Add(handler.Flag, handler);
            return handler;
        }

        public IActionParsingHandler GetActionHandler(string magicWord)
        {
            return _actionHandlerDic.ContainsKey(magicWord) ? _actionHandlerDic[magicWord] : null;
        }

        private readonly Dictionary<string, IActionParsingHandler> _actionHandlerDic = new Dictionary<string, IActionParsingHandler>();
    }
}