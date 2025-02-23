using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Kibo.Data
{
    public class FileDataService : IDataService<GameData>
    {
        private static string SaveFolder => Path.Combine(Application.persistentDataPath, "Saves");
        private const string FileExtension = ".json";

        public IEnumerable<string> Saves => Directory.EnumerateFiles(SaveFolder, "?*" + FileExtension).Select(path => Path.GetFileNameWithoutExtension(path));

        private readonly ISerializer serializer;

        public FileDataService(ISerializer serializer)
        {
            this.serializer = serializer;

            if (!Directory.Exists(SaveFolder)) Directory.CreateDirectory(SaveFolder);
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(SaveFolder, fileName + FileExtension);
        }

        public void Save(GameData gameData, bool overwrite = true)
        {
            string filePath = GetFilePath(gameData.Name);

            if (!overwrite && File.Exists(filePath)) Debug.LogError($"{nameof(GameData)} already exists at '{filePath}'");

            File.WriteAllText(filePath, serializer.Serialize(gameData));
        }

        public GameData Load(string name)
        {
            string filePath = GetFilePath(name);

            if (!File.Exists(filePath)) Debug.LogError($"No {nameof(GameData)} at '{filePath}'");

            return serializer.Deserialize<GameData>(File.ReadAllText(filePath));
        }

        public void Delete(string name)
        {
            string filePath = GetFilePath(name);

            if (!File.Exists(filePath)) Debug.LogError($"No {nameof(GameData)} at '{filePath}'");

            File.Delete(filePath);
        }

        public void DeleteAll()
        {
            foreach (string filePath in Directory.EnumerateFiles(SaveFolder, "?*" + FileExtension))
            {
                File.Delete(filePath);
            }
        }
    }
}