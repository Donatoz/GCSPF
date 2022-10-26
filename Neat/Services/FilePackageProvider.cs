using Neat.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Neat.Services
{
    public class FilePackageProvider : IPackageProvider
    {
        public Package? FetchPackage(string path)
        {
            var bytes = File.ReadAllBytes(path);
            Package? package = null;

            try
            {
                package = JsonSerializer.Deserialize<Package>(bytes);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

            return package;
        }

        public bool SavePackage(Package package, string path)
        {
            try
            {
                var json = JsonSerializer.Serialize(package, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
