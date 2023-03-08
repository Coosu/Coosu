using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Internal;

namespace Coosu.Storyboard;

public static class LoopTriggerExtensions
{
    /// <summary>
    /// Create a loop object, and execute after calling `Dispose` .
    /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="loopCount"></param>
    /// <remarks>This method returns a standalone disposable loop object, which is different from <see cref="StartLoop"/>, can provide split control from the outer sprite. Support since v2.3.11.</remarks>
    /// <returns></returns>
    public static IEventHostDisposable CreateLoop(this ILoopHost loopHost, int startTime, int loopCount)
    {
        var loop = new Loop(startTime, loopCount);
        return new LoopTriggerCreationWrapper(loopHost, loop);
    }

    /// <summary>
    /// Create a trigger object, and execute after calling `Dispose` .
    /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
    /// </summary>
    /// <param name="startTime">Group start time.</param>
    /// <param name="endTime">Group end time.</param>
    /// <param name="triggerType">Trigger type. It can be specified in a flag form like TriggerType.HitSoundWhistle | TriggerType.HitSoundSoft.</param>
    /// <param name="listenSample">If use the listenSample, the trigger will listen to all hitsound in a track like HitsoundAllNormal.</param>
    /// <param name="customSampleSet">Listen to a specific track. 0 represents default track.</param>
    /// <remarks>This method returns a standalone disposable trigger object, which is different from <see cref="StartLoop"/>, can provide split control from the outer sprite. Support since v2.3.11.</remarks>
    /// <returns></returns>
    public static IEventHostDisposable CreateTrigger(this ITriggerHost triggerHost, int startTime, int endTime, TriggerType triggerType, bool listenSample = false, uint? customSampleSet = null)
    {
        var trigger = new Trigger(startTime, endTime, triggerType, listenSample, customSampleSet);
        return new LoopTriggerCreationWrapper(triggerHost, trigger);
    }

    /// <summary>
    /// Create a trigger object, and execute after calling `Dispose` .
    /// For more information, see: https://osu.ppy.sh/help/wiki/Storyboard_Scripting/Compound_Commands
    /// </summary>
    /// <param name="startTime">Group start time.</param>
    /// <param name="endTime">Group end time.</param>
    /// <param name="triggerName">A valid trigger name.</param>
    /// <remarks>This method returns a standalone disposable trigger object, which is different from <see cref="StartLoop"/>, can provide split control from the outer sprite. Support since v2.3.11.</remarks>
    /// <returns></returns>
    public static IEventHostDisposable CreateTrigger(this ITriggerHost triggerHost, int startTime, int endTime, string triggerName)
    {
        var trigger = new Trigger(startTime, endTime, triggerName);
        return new LoopTriggerCreationWrapper(triggerHost, trigger);
    }
}