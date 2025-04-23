using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CollectionManager.Models
{
    public class FileService
    {
        private readonly string folderPath = FileSystem.AppDataDirectory;

        public FileService()
        {
            Debug.WriteLine($"[DEBUG] App data path: {folderPath}");
        }

        public async Task SaveCollection(Collection collection)
        {
            string path = Path.Combine(folderPath, collection.Name + ".txt");
            var lines = collection.Items.Select(e => e.ToString());
            await File.WriteAllLinesAsync(path, lines);
        }

        public async Task<Collection?> LoadCollection(string name)
        {
            string path = Path.Combine(folderPath, name + ".txt");
            if (!File.Exists(path)) return null;

            var lines = await File.ReadAllLinesAsync(path);
            return new Collection
            {
                Name = name,
                Items = lines.Select(CollectionItem.FromString).ToList()
            };
        }

        public List<string> ListFiles()
        {
            return Directory.GetFiles(folderPath, "*.txt")
                            .Select(Path.GetFileNameWithoutExtension)
                            .ToList();
        }
    }
}
