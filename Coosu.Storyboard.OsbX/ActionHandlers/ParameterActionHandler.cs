using System;
using System.Collections.Generic;
using System.IO;
using Coosu.Shared;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard.OsbX.ActionHandlers;

public class ParameterActionHandler : ActionHandler<Parameter>
{
    public override string Flag => "P";
    public override Parameter Deserialize(ref ValueListBuilder<string> split)
    {
        if (split.Length != 5)
        {
            throw new ArgumentException("Wrong parameter definition");
        }

        var easing = EasingConvert.ToEasing(split[1]);
        var startTime = double.Parse(split[2]);
        var endTime = string.IsNullOrWhiteSpace(split[3]) ? startTime : double.Parse(split[3]);
        var type = split[4];
        return new Parameter(startTime, endTime, new List<double> { (int)type.ToParameterEnum() });
    }

    public override string Serialize(Parameter e)
    {
        using var sw = new StringWriter();
        e.WriteScriptAsync(sw).Wait();
        return sw.ToString();
    }
}