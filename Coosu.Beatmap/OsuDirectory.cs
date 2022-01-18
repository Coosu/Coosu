using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Coosu.Beatmap.Management
{
    public class OsuDirectory
    {
        public List<OsuFile> FileList { get; } = new List<OsuFile>();

        public OsuDirectory()
        {

        }

        public async Task LoadFromDirectoryAsync(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.osu");
            foreach (var file in files)
                FileList.Add(await OsuFile.ReadFromFileAsync(file.FullName));
        }

        public async Task LoadFromFileAsync(string path)
        {
            FileList.Add(await OsuFile.ReadFromFileAsync(path));
        }
    }
}
