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
}
