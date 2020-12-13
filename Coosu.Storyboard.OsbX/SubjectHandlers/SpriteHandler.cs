using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;
using System;

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
            return $"{Flag},{raw.Layer},{raw.Origin},\"{raw.ImagePath}\",{raw.DefaultX},{raw.DefaultY},{raw.CameraId},{raw.ZDistance},0";
        }

        public override Element Deserialize(string[] split)
        {
            if (split.Length == 6 || split.Length == 9)
            {
                var type = ElementTypeSign.Parse(split[0]);
                var layerType = (LayerType)Enum.Parse(typeof(LayerType), split[1]);
                var origin = (OriginType)Enum.Parse(typeof(OriginType), split[2]);
                var path = split[3].Trim('\"');
                var defX = float.Parse(split[4]);
                var defY = float.Parse(split[5]);

                float zDistance = 1;
                int cameraId = 0;
                if (split.Length == 9)
                {
                    cameraId = int.Parse(split[6]);
                    zDistance = float.TryParse(split[7], out var result) ? result : 1f;
                    var absolute = int.Parse(split[8]) != 0;
                    if (absolute)
                    {
                        throw new NotImplementedException("Absolute mode currently is not supported.");
                    }
                }

                return new Element(type, layerType, origin, path, defX, defY)
                {
                    ZDistance = zDistance,
                    CameraId = cameraId
                };
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}