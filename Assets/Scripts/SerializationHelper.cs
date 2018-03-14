using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class SerializationHelper
{
    public static string StorageDirectory { get; }
    public static string SavesDirectory { get { return StorageDirectory + @"\Saves"; } }

    private static readonly string saveExtension = "sav";

    static SerializationHelper()
    {
        StorageDirectory = string.Format(@"{0}\{1}\{2}",
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            PlayerSettings.companyName,
            PlayerSettings.productName);

        CreateDirectory(SavesDirectory);
    }

    /// <summary>
    /// Zapisuje stan gry do domyślnej lokalizacji w Dokumentach
    /// </summary>
    /// <param name="status"></param>
    /// <param name="saveName">Nazwa pliku bez rozszerzenia</param>
    public static void SaveGame(GameStatus status, string saveName)
    {
        using (var fileStream = new FileStream($@"{SavesDirectory}\{saveName}.{saveExtension}", FileMode.OpenOrCreate))
        using (var streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.Write(JsonUtility.ToJson(status));
        }
    }

    /// <summary>
    /// Wczytuje stan gry z domyślnej lokalizacji w Dokumentach
    /// </summary>
    /// <param name="saveName">Nazwa pliku bez rozszerzenia</param>
    /// <returns></returns>
    public static GameStatus LoadGame(string saveName)
    {
        GameStatus gamestatus;

        using (var fileStream = new FileStream($@"{SavesDirectory}\{saveName}.{saveExtension}", FileMode.Open))
        using (var streamReader = new StreamReader(fileStream))
        {
            gamestatus = JsonUtility.FromJson<GameStatus>(streamReader.ReadToEnd());
        }

        return gamestatus;
    }

    public static string[] GetSavesList()
    {
        return Directory.GetFiles(SavesDirectory).
            Where(n => n.ToLower().
            EndsWith($".{saveExtension}")).
            Select(n => Path.GetFileNameWithoutExtension(n)).
            ToArray();
    }

    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}