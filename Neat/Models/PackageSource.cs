using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace Neat.Models
{
    public class PackageSource
    {
        public PackageSourceType Type { get; set; }
        public string Path { get; set; }
        [JsonIgnore]
        public bool Removable { get; }
        [JsonIgnore]
        public bool Editable { get; }

        [JsonConstructor]
        public PackageSource(PackageSourceType type, string path)
        {
            Type = type;
            Path = path;
        }

        public PackageSource(PackageSourceType type, string path, bool removable = true, bool editable = true) : this(type, path)
        {
            Removable = removable;
            Editable = editable;
        }

        public IEnumerable<FileInfo> Fetch()
        {
            if (Type == PackageSourceType.LocalDirectory)
            {
                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                    yield break;
                }

                var rawFiles = Directory.GetFiles(Path);

                foreach (var filePath in rawFiles)
                {
                    yield return new FileInfo(filePath);
                }
            }
        }
    }
}
