using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] Tilemap platformTilemap = null;
    [SerializeField] Tilemap hazardTilemap = null;

    [SerializeField] Tile platformTile = null;
    [SerializeField] Tile upSpikeTile = null;
    [SerializeField] Tile downSpikeTile = null;
    [SerializeField] Tile leftSpikeTile = null;
    [SerializeField] Tile rightSpikeTile = null;
    [SerializeField] GameObject checkpointBlock = null;
    [SerializeField] GameObject endBlock = null;

    [SerializeField] Image currentBlockSprite;

    int currentBlock = 0;

    Dictionary<Vector3, GameObject> objTiles;

    private void Start()
    {
        objTiles = new Dictionary<Vector3, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        SwitchBlocks();

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3Int tilePos = platformTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(mousePos));
            PlaceBlock(tilePos);
        }
        else if(Input.GetMouseButton(1))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3Int tilePos = platformTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(mousePos));
            RemoveBlock(tilePos);
        }
    }

    private void PlaceBlock(Vector3Int tilePos)
    {
        GridLayout gridLayout = FindObjectOfType<GridLayout>();
        var halfGrid = gridLayout.cellSize.x / 2;
        Vector3 cellPosition = new Vector3(tilePos.x + halfGrid, tilePos.y + halfGrid, 0);

        if (objTiles.ContainsKey(cellPosition))
        {
            Destroy(objTiles[cellPosition].gameObject);
            objTiles.Remove(cellPosition);
        }

        platformTilemap.SetTile(tilePos, null);
        hazardTilemap.SetTile(tilePos, null);

        switch (currentBlock)
        {
            case 0:
                {
                    platformTilemap.SetTile(tilePos, platformTile);
                    break;
                }
            case 1:
                {
                    hazardTilemap.SetTile(tilePos, upSpikeTile);
                    break;
                }
            case 2:
                {
                    hazardTilemap.SetTile(tilePos, downSpikeTile);
                    break;
                }
            case 3:
                {
                    hazardTilemap.SetTile(tilePos, leftSpikeTile);
                    break;
                }
            case 4:
                {
                    hazardTilemap.SetTile(tilePos, rightSpikeTile);
                    break;
                }
            case 5:
                {
                    GameObject checkpointObj = GameObject.Instantiate(checkpointBlock, cellPosition, Quaternion.identity);

                    objTiles.Add(cellPosition, checkpointObj);
                    break;
                }
            case 6:
                {
                    GameObject endObj = GameObject.Instantiate(endBlock, cellPosition, Quaternion.identity);

                    objTiles.Add(cellPosition, endObj);
                    break;
                }
        }
    }

    private void RemoveBlock(Vector3Int tilePos)
    {
        GridLayout gridLayout = FindObjectOfType<GridLayout>();
        var halfGrid = gridLayout.cellSize.x / 2;
        Vector3 cellPosition = new Vector3(tilePos.x + halfGrid, tilePos.y + halfGrid, 0);

        if (objTiles.ContainsKey(cellPosition))
        {
            Destroy(objTiles[cellPosition].gameObject);
            objTiles.Remove(cellPosition);
        }

        platformTilemap.SetTile(tilePos, null);
        hazardTilemap.SetTile(tilePos, null);
    }

    private void SwitchBlocks()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBlock = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBlock = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBlock = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBlock = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBlock = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBlock = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentBlock = 6;
        }


        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll > 0f)
        {
            currentBlock++;

            if (currentBlock > 6) currentBlock = 0;
        }
        else if(scroll < 0f)
        {
            currentBlock--;

            if (currentBlock < 0) currentBlock = 6;
        }

        SetToolSprite();

    }

    void SetToolSprite()
    {
        switch(currentBlock)
        {
            case 0:
                {
                    currentBlockSprite.sprite = platformTile.sprite;
                    break;
                }
            case 1:
                {
                    currentBlockSprite.sprite = upSpikeTile.sprite;
                    break;
                }
            case 2:
                {
                    currentBlockSprite.sprite = downSpikeTile.sprite;
                    break;
                }
            case 3:
                {
                    currentBlockSprite.sprite = leftSpikeTile.sprite;
                    break;
                }
            case 4:
                {
                    currentBlockSprite.sprite = rightSpikeTile.sprite;
                    break;
                }
            case 5:
                {
                    currentBlockSprite.sprite = checkpointBlock.GetComponent<SpriteRenderer>().sprite;
                    break;
                }
            case 6:
                {
                    currentBlockSprite.sprite = endBlock.GetComponent<SpriteRenderer>().sprite;
                    break;
                }
        }
    }
}
