using System;
using System.Collections.Generic;
using System.Text;

namespace Coosu.Storyboard.Management
{
    public class ElementManager
    {
        public Dictionary<float, ElementGroup> GroupList { get; } = new Dictionary<float, ElementGroup>();

        public bool ContainsGroup(float zDistance)
        {
            return GroupList.ContainsKey(zDistance);
        }

        public ElementGroup GetOrAddGroup(float zDistance)
        {
            if (ContainsGroup(zDistance))
                return GroupList[zDistance];
            return CreateGroup(zDistance);
        }

        public ElementGroup CreateGroup(float zDistance)
        {
            var elementGroup = new ElementGroup(zDistance);
            GroupList.Add(zDistance, elementGroup);
            return elementGroup;
        }

        public void AddGroup(ElementGroup elementGroup)
        {
            GroupList.Add(elementGroup.ZDistance, elementGroup);
        }

        public void DeleteGroup(ElementGroup elementGroup)
        {
            GroupList.Remove(elementGroup.ZDistance);
        }

        public void DeleteGroup(int zIndex)
        {
            GroupList.Remove(zIndex);
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

            foreach (var a in GroupList.Values)
            {
                sb.Append(a.ToOsbString());
            }

            return sb.ToString();
        }

        public void Save(string path) =>
            System.IO.File.WriteAllText(path, "[Events]" + Environment.NewLine + "//Background and Video events" + Environment.NewLine +
                   "//Storyboard Layer 0 (Background)" + Environment.NewLine + ToString() + "//Storyboard Sound Samples" + Environment.NewLine);

    }
}
