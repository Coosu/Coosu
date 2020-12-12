using System;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;

namespace Coosu.Storyboard.OsbX.SubjectHandlers
{
    public class SpriteHandler : SubjectHandler<Element>
    {
        public SpriteHandler()
        {
            RegisterAction(Register.GetActionHandlerInstance<MoveActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<MoveXActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<MoveYActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<FadeActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<ScaleActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<RotateActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<VectorActionHandler>());
            RegisterAction(Register.GetActionHandlerInstance<OriginActionHandler>());
        }

        public override string Flag => "Sprite";

        //Sprite,0,Centre,"",320,240
        public override string Serialize(Element raw)
        {
            return $"{Flag},{raw.ZIndex},{raw.Origin},\"{raw.ImagePath}\",{raw.DefaultX},{raw.DefaultY}";
        }

        public override Element Deserialize(string[] split)
        {
            if (split.Length == 6)
            {
                var type = ElementTypeSign.Parse(split[0]);
                var zIndex = int.TryParse(split[1], out var result) ? result : 1;
                var origin = (OriginType)Enum.Parse(typeof(OriginType), split[2]);
                var path = split[3].Trim('\"');
                var defX = float.Parse(split[4]);
                var defY = float.Parse(split[5]);
                return new Element(type, LayerType.Foreground, origin, path, defX, defY)
                {
                    ZIndex = zIndex
                };
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}