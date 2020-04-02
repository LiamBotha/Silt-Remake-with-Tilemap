using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelConverter: MonoBehaviour
{
    [SerializeField] Tilemap backgroundTilemap = null;
    [SerializeField] Tilemap platformTilemap = null;
    [SerializeField] Tilemap hazardTilemap = null;

    [SerializeField] Tile backgroundTile = null;

    [SerializeField] GameObject checkpointPrefab = null;
    [SerializeField] GameObject endpointPrefab = null;

    [SerializeField] GameObject loadingCanvas = null;

    Dictionary<string, Tile> tiles;

    bool dataLoaded = false;

    private void Start()
    {
        LoadAllTiles();
    }

    public void LoadLevelAndPlay()
    {
        dataLoaded = false;

        LoadFromFile();

        if(dataLoaded)
        {
            loadingCanvas.SetActive(false);

            StartCoroutine(LoadPlayerAsync());
        }
    }

    public void ReloadLevelAndPlay()
    {
        dataLoaded = false;

        LoadLastFile();

        if (dataLoaded)
        {
            loadingCanvas.SetActive(false);

            StartCoroutine(LoadPlayerAsync());
        }
    }

    private IEnumerator LoadPlayerAsync()
    {
        AsyncOperation asyncPlayerLoad = SceneManager.LoadSceneAsync("PlayerScene",LoadSceneMode.Additive);
        AsyncOperation asyncUILoad = SceneManager.LoadSceneAsync("UIScene",LoadSceneMode.Additive);

        while(!asyncPlayerLoad.isDone && !asyncUILoad.isDone)
        {
            yield return null;
        }

        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("PlayerScene"));

        GameObject.FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D = backgroundTilemap.GetComponent<CompositeCollider2D>();
    }

    public void LoadAllTiles()
    {
        tiles = new Dictionary<string, Tile>();

        var loadedTiles = Resources.LoadAll<Tile>("Tileset");

        int i = 0;
        foreach(Tile tile in loadedTiles)
        {
            ++i;
            tiles.Add(i.ToString(), tile);
            Debug.Log(tile.name + " tile loaded");
        }
    }

    public string ConvertTilemap(Tilemap tilemap)
    {
        tilemap.CompressBounds();
        var bounds = tilemap.cellBounds;
        var allTiles = tilemap.GetTilesBlock(bounds);

        var width = bounds.xMax - bounds.xMin;
        var height = bounds.yMax - bounds.yMin;

        string output = "";

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                var tile = allTiles[x + width * y];
                if (tile != null)
                {
                    var tileID = tiles.FirstOrDefault(t => t.Value == tile);

                    output += tileID.Key + ":" + (bounds.xMin + x) + "," + (bounds.yMin + y) + " ";
                }
            }
            output += Environment.NewLine;
        }

        return output;
    }

    public string ConvertGameobjects()
    {
        string output = "";

        var checkpoints = FindObjectsOfType<Checkpoint>();
        var endPoints = FindObjectsOfType<LevelEnd>();

        foreach (Checkpoint c in checkpoints)
        {
            output += "C"
                + ":" + c.transform.position.x.ToString("0.0", CultureInfo.InvariantCulture)
                + "," + c.transform.position.y.ToString("0.0", CultureInfo.InvariantCulture) + " ";
        }

        foreach (LevelEnd end in endPoints)
        {
            output += "E"
                + ":" + end.transform.position.x.ToString("0.0", CultureInfo.InvariantCulture)
                + "," + end.transform.position.y.ToString("0.0", CultureInfo.InvariantCulture) + " ";
        }

        return output;
    }

    public void SaveToFile()
    {
        string output = ConvertTilemap(platformTilemap);
        output += "|" + Environment.NewLine;
        output += ConvertTilemap(hazardTilemap);
        output += "|" + Environment.NewLine;
        output += ConvertGameobjects();

        string filePath = FileEditor.SetFile();

        if(filePath != null)
        {
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.Write(output);
            writer.Flush();
            writer.Close();

            Debug.Log("Save Completed");
        }
    }

    public void LoadLastFile()
    {
        List<string[]> platformInput = new List<string[]>();
        List<string[]> hazardInput = new List<string[]>();
        List<string[]> checkpointInput = new List<string[]>();

        dataLoaded = GetLastData(platformInput, hazardInput, checkpointInput);

        if (dataLoaded)
        {
            platformTilemap.ClearAllTiles();
            hazardTilemap.ClearAllTiles();

            var checkpoints = FindObjectsOfType<Checkpoint>();
            foreach (var c in checkpoints)
            {
                Destroy(c.gameObject);
            }

            LoadInTiles(platformInput, hazardInput, checkpointInput);

            Debug.Log("Load Completed");
        }
    }

    public void LoadFromFile() // TODO - build out method to get all tiletypes procedurally
    {
        List<string[]> platformInput = new List<string[]>();
        List<string[]> hazardInput = new List<string[]>();
        List<string[]> checkpointInput = new List<string[]>();

        dataLoaded = GetDataFromFile(platformInput, hazardInput, checkpointInput);

        if(dataLoaded)
        {
            platformTilemap.ClearAllTiles();
            hazardTilemap.ClearAllTiles();

            var checkpoints = FindObjectsOfType<Checkpoint>();
            foreach (var c in checkpoints)
            {
                Destroy(c.gameObject);
            }

            LoadInTiles(platformInput, hazardInput, checkpointInput);

            Debug.Log("Load Completed");
        }
    }

    private static bool GetLastData(List<string[]> platformInput, List<string[]> hazardInput, List<string[]> checkpointInput)
    {
        string filePath = FileEditor.LastPath;

        if (filePath != null)
        {
            StreamReader reader = new StreamReader(filePath);// TODO - Let Player choose file using fileEditor script

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

    private static bool GetDataFromFile(List<string[]> platformInput, List<string[]> hazardInput, List<string[]> checkpointInput)
    {
        string filePath = FileEditor.GetFile();

        if (filePath != null)
        {
            StreamReader reader = new StreamReader(filePath);// TODO - Let Player choose file using fileEditor script

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

    private void LoadInTiles(List<string[]> platformInput, List<string[]> hazardInput, List<string[]> checkpointInput)
    {
        for (int y = 0; y < platformInput.Count; ++y)
        {
            for (int x = 0; x < platformInput[y].Length; x++)
            {
                if (platformInput[y][x] != "0" && platformInput[y][x] != "")
                {
                    var tileData = platformInput[y][x].Split(':');
                    var position = tileData[1].Split(',');

                    Debug.Log(platformInput[y][x]);
                    var tile = tiles[tileData[0]];
                    platformTilemap.SetTile(new Vector3Int(int.Parse(position[0]), int.Parse(position[1]), 0), tile);
                }
            }
        }

        for (int y = 0; y < hazardInput.Count; ++y)
        {
            for (int x = 0; x < hazardInput[y].Length; x++)
            {
                if (hazardInput[y][x] != "0" && hazardInput[y][x] != "")
                {
                    var tileData = hazardInput[y][x].Split(':');
                    var position = tileData[1].Split(',');

                    var tile = tiles[tileData[0]];
                    hazardTilemap.SetTile(new Vector3Int(int.Parse(position[0]), int.Parse(position[1]), 0), tile);
                }
            }
        }

        for (int y = 0; y < checkpointInput.Count; ++y)
        {
            for (int x = 0; x < checkpointInput[y].Length; x++)
            {
                if (checkpointInput[y][x] != "0" && checkpointInput[y][x] != "")
                {
                    var tileData = checkpointInput[y][x].Split(':');
                    var position = tileData[1].Split(',');

                    Vector3 cellPosition = new Vector3(float.Parse(position[0], CultureInfo.InvariantCulture), float.Parse(position[1], CultureInfo.InvariantCulture), 0);
                    if (tileData[0].Contains("C"))
                        GameObject.Instantiate(checkpointPrefab, cellPosition, Quaternion.identity);
                    else if (tileData[0].Contains("E"))
                        GameObject.Instantiate(endpointPrefab, cellPosition, Quaternion.identity);
                }
            }
        }

        platformTilemap.CompressBounds();
        var bounds = platformTilemap.cellBounds;

        var width = bounds.xMax - bounds.xMin;
        var height = bounds.yMax - bounds.yMin;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                backgroundTilemap.SetTile(new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0), backgroundTile);
            }
        }
    }

}
