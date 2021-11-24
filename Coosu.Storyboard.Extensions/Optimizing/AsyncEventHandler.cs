using System;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    [Serializable]
    public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs e);
}