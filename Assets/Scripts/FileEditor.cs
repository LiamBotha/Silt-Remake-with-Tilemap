using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class FileEditor
{

#if UNITY_EDITOR
    static string startPath = Application.dataPath.Replace("/Assets", "/Build/Levels");
#else
    static string startPath = Application.dataPath.Replace("/Silt - Remake with Tilemap_Data", "/Levels");
#endif

    public static string LastPath { get; private set; } = "";

    public static string GetFile()
    {
        //string filePath = EditorUtility.OpenFilePanel("Override with .tilemap", startPath, "Tilemap");

        string filePath = StandaloneFileBrowser.OpenFilePanel("Override with .tilemap", startPath, "Tilemap",false).FirstOrDefault();

        if (filePath.Length != 0)
        {
            LastPath = filePath;
            return filePath;
        }

        return null;
    }

    public static string SetFile()
    {
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save as .tilemap", startPath, "level.tilemap","Tilemap");

        if(filePath.Length != 0)
        {
            LastPath = filePath;
            return filePath;
        }

        return null;
    }

    public static bool GetDataFromFile(out List<string[]> platformInput, out List<string[]> hazardInput, out List<string[]> checkpointInput)
    {
        platformInput = new List<string[]>();
        hazardInput = new List<string[]>();
        checkpointInput = new List<string[]>();

        string filePath = FileEditor.GetFile();

        if (filePath != null)
        {
            StreamReader reader = new StreamReader(filePath);

            string line = "";

            while (!(line = reader.ReadLine()).Contains("|"))
            {
                platformInput.Add(line.Split(' '));
            }

            while (!(line = reader.ReadLine()).Contains("|"))
            {
                hazardInput.Add(line.Split(' '));
            }

            while ((line = reader.ReadLine()) != null)
            {
                checkpointInput.Add(line.Split(' '));
            }

            return true;
        }

        return false;
    }

    public static bool GetLastData(out List<string[]> platformInput, out List<string[]> hazardInput, out List<string[]> checkpointInput)
    {
        platformInput = new List<string[]>();
        hazardInput = new List<string[]>();
        checkpointInput = new List<string[]>();

        string filePath = LastPath;

        if (filePath != null) // Maybe turn into seperate method as it is getting reused
        {
            StreamReader reader = new StreamReader(filePath);

            string line = "";

            while (!(line = reader.ReadLine()).Contains("|"))
            {
                platformInput.Add(line.Split(' '));
            }

            while (!(line = reader.ReadLine()).Contains("|"))
            {
                hazardInput.Add(line.Split(' '));
            }

            while ((line = reader.ReadLine()) != null)
            {
                checkpointInput.Add(line.Split(' '));
            }

            return true;
        }

        return false;
    }

    public static void SaveString(string output)
    {
        string filePath = SetFile();

        if (filePath != null)
        {
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.Write(output);
            writer.Flush();
            writer.Close();

            Debug.Log("Save Completed");
        }
    }
}
