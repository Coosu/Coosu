using System;
using System.Collections.Generic;
using System.Text;

namespace OSharp.Storyboard.Management
{
    public class ElementManager
    {
        private SortedSet<ElementGroup> GroupList { get; set; } = new SortedSet<ElementGroup>(new GroupComparer());

        public void CreateGroup(int layerIndex)
        {
            GroupList.Add(new ElementGroup(layerIndex));
        }

        public void Add(ElementGroup elementGroup)
        {
            GroupList.Add(elementGroup);
        }

        public static ElementGroup Adjust(ElementGroup elementGroup, float offsetX, float offsetY, int offsetTiming)
        {
            foreach (var obj in elementGroup.ElementList)
            {
                obj.Adjust(offsetX, offsetY, offsetTiming);
            }
            return elementGroup;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var a in GroupList)
            {
                sb.Append(a);
            }

            return sb.ToString();
        }

        public void Save(string path) =>
            System.IO.File.WriteAllText(path, "[Events]" + Environment.NewLine + "//Background and Video events" + Environment.NewLine +
                   "//Storyboard Layer 0 (Background)" + Environment.NewLine + ToString() + "//Storyboard Sound Samples" + Environment.NewLine);

    }
}
