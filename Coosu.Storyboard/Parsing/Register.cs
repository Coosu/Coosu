using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Parsing
{
    /// <summary>
    /// 1. should be pre-compiled for basic
    /// 2. extensible for subject and action
    /// </summary>
    public static class Register
    {
        public static SubjectHandler RegisterSubject(SubjectHandler handler)
        {
            throw new NotImplementedException();
            return handler;
        }
    }

    public abstract class SubjectHandler : IParsingHandler
    {
        public abstract string MahouWord { get; }
        protected List<Type> ActionHandlers { get; private set; }
    }

    //extent
    public class CameraHandler : SubjectHandler
    {
        public override string MahouWord => "Camera";
    }

    public class AnimationHandler : SubjectHandler
    {
        public override string MahouWord => "Animation";
    }

    public class SpriteHandler : SubjectHandler
    {
        public override string MahouWord => "Sprite";
    }

    public interface IParsingHandler<T> : IParsingHandler
    {
        T Parse(string line);
    }

    public interface IParsingHandler
    {
        string MahouWord { get; }
        object Parse(string line);
    }

    public abstract class ActionHandler : IParsingHandler
    {
        public abstract string MahouWord { get; }
    }
    public abstract class BasicTimelineHandler : ActionHandler
    {
    }

    public class FadeActionHandler : ActionHandler
    {
        public override string MahouWord => "F";
    }

    //extent
    public class ZoomInActionHandler : ActionHandler
    {
        public override string MahouWord => "ZI";
    }

    //extent
    public class ZoomOutActionHandler : ActionHandler
    {
        public override string MahouWord => "ZO";
    }
}
