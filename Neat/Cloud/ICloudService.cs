using Neat.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Neat.Cloud
{
    public interface ICloudService : IVaryingService
    {
        Task<Exception?> Connect();
        void Disconnect();

        Task<FileInfo?> Fetch(string root, string fileName, string destination);
        Task<string?> UploadFile(string destination, FileInfo file);
        Task<IEnumerable<string>> List(string root, string prefix, int maxFilesAmount);
    }
}
