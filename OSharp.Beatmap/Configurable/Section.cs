using System.Reflection;

namespace OSharp.Beatmap.Configurable
{
    public abstract class Section : SerializeWritableObject, ISection
    {
        [SectionIgnore]
        public string SectionName { get; }

        protected Section()
        {
            var type = GetType();
            var sb = type.GetCustomAttribute<SectionPropertyAttribute>();
            SectionName = sb != null ? sb.Name : type.Name;
        }

        public abstract void Match(string line);
    }
}
