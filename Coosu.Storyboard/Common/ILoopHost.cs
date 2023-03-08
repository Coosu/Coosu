using System.Collections.Generic;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Common;

public interface ILoopHost
{
    IReadOnlyList<Loop> LoopList { get; }
    void AddLoop(Loop loop);
    bool RemoveLoop(Loop loop);
    void ClearLoops();
}