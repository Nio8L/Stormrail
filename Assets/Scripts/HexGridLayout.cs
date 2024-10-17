using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rule{
    public Vector2Int coordinates;
    public HexTile.Type type;
}

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize;
    public GameObject hexPrefab;

    private List<GameObject> tileObjects = new();
    public HexTile[,] tiles;

    public List<Material> materials;
    public List<Rule> rules;

    private void Awake() {
        tiles = new HexTile[gridSize.x, gridSize.y];
    }

    private void OnValidate() {
        if(Application.isPlaying){
            LayoutGrid();
        }
    }

    private void OnEnable() {
        LayoutGrid();
    }

    public void LayoutGrid(){
        foreach (GameObject tile in tileObjects)
        {
            Destroy(tile);
        }
        
        tileObjects.Clear();
        tiles = new HexTile[gridSize.x, gridSize.y];
        
        for(int x = 0; x < gridSize.x; x++){
            for(int y = 0; y < gridSize.y; y++){
                GameObject hex = Instantiate(hexPrefab, transform.position, Quaternion.identity);
                hex.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));
                hex.transform.rotation = Quaternion.Euler(-90, -90, 0);
                tileObjects.Add(hex);
                
                tiles[x, y] = hex.GetComponent<HexTile>();
                tiles[x, y].Initialize(new Vector2Int(x, y));
                

                hex.transform.SetParent(transform, true);
            }
        }

        foreach(Rule rule in rules){
            tiles[rule.coordinates.x, rule.coordinates.y].SetType(rule.type, materials[(int) rule.type]);
        }
    }

    private Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinates)
    {
        int column = coordinates.x;
        int row = coordinates.y;
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;

        
        shouldOffset = column % 2 == 0;
        width = 2f;
        height = Mathf.Sqrt(3);
        horizontalDistance = width * (3f / 4f);
        verticalDistance = height;
        offset = shouldOffset ? height / 2 : 0;
        xPosition = column * horizontalDistance;
        yPosition = row * verticalDistance - offset;
        
        
        return new Vector3(xPosition, 0, yPosition);
    }
}
