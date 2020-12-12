using System;
using System.Collections.Generic;
using System.Text;

namespace Coosu.Storyboard.Management
{
    public class ElementManager
    {
        public Dictionary<int, ElementGroup> GroupList { get; } = new Dictionary<int, ElementGroup>();

        public bool ContainsGroup(int zIndex)
        {
            return GroupList.ContainsKey(zIndex);
        }

        public ElementGroup GetOrAddGroup(int zIndex)
        {
            if (ContainsGroup(zIndex))
                return GroupList[zIndex];
            return CreateGroup(zIndex);
        }

        public ElementGroup CreateGroup(int zIndex)
        {
            var elementGroup = new ElementGroup(zIndex);
            GroupList.Add(zIndex, elementGroup);
            return elementGroup;
        }

        public void AddGroup(ElementGroup elementGroup)
        {
            GroupList.Add(elementGroup.Index, elementGroup);
        }

        public void DeleteGroup(ElementGroup elementGroup)
        {
            GroupList.Remove(elementGroup.Index);
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
