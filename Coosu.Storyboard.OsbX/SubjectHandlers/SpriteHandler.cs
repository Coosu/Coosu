using System;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.ActionHandlers;

namespace Coosu.Storyboard.OsbX.SubjectHandlers
{
    public class SpriteHandler : SubjectHandler<Sprite>
    {
        public SpriteHandler()
        {
            RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveXActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<MoveYActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<FadeActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<ScaleActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<RotateActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<VectorActionHandler>());
            RegisterAction(HandlerRegister.GetActionHandlerInstance<OriginActionHandler>());
        }

        public override string Flag => "Sprite";

        //Sprite,0,Centre,"",320,240
        public override string Serialize(Sprite raw)
        {
            return $"{Flag},{raw.LayerType},{raw.OriginType},\"{raw.ImagePath}\",{raw.DefaultX},{raw.DefaultY},{raw.CameraIdentifier},{raw.DefaultZ},0";
        }

        public override Sprite Deserialize(string[] split)
        {
            if (split.Length is not (6 or 9)) throw new ArgumentOutOfRangeException();

            var type = ObjectType.Parse(split[0]);
            var layerType = (LayerType)Enum.Parse(typeof(LayerType), split[1]);
            var origin = (OriginType)Enum.Parse(typeof(OriginType), split[2]);
            var path = split[3].Trim('\"');
            var defX = double.Parse(split[4]);
            var defY = double.Parse(split[5]);

            double defaultZ = 1;
            string cameraIdentifier = Guid.Empty.ToString();
            if (split.Length == 9)
            {
                cameraIdentifier = split[6];
                defaultZ = double.TryParse(split[7], out var result) ? result : 1f;
                var absolute = int.Parse(split[8]) != 0;
                if (absolute)
                {
                    throw new NotImplementedException("Absolute mode currently is not supported.");
                }
            }

            return new Sprite(layerType, origin, path, defX, defY)
            {
                DefaultZ = defaultZ,
                CameraIdentifier = cameraIdentifier
            };

        }
    }
}