using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EzYuzu
{
    public static class DirectoryUtilities
    {
        // https://stackoverflow.com/a/49570235
        public static void Copy(string fromFolder, string toFolder, bool overwrite = false)
        {
            Directory
                .EnumerateFiles(fromFolder, "*.*", SearchOption.AllDirectories)
                .Where(file => (File.GetAttributes(file) & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                .AsParallel()
                .ForAll(from =>
                {
                    var to = from.Replace(fromFolder, toFolder);

                    // Create directories if required
                    var toSubFolder = Path.GetDirectoryName(to);
                    if (!string.IsNullOrWhiteSpace(toSubFolder))
                    {
                        Directory.CreateDirectory(toSubFolder);
                    }

                    File.Copy(from, to, overwrite);
                });
        }
    }
}
