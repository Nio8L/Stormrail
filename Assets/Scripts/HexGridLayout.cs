using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize;
    public GameObject hexPrefab;

    private List<GameObject> tiles = new();

    private void OnValidate() {
        if(Application.isPlaying){
            LayoutGrid();
        }
    }

    private void OnEnable() {
        LayoutGrid();
    }

    public void LayoutGrid(){
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }
        
        tiles.Clear();
        
        for(int x = 0; x < gridSize.x; x++){
            for(int y = 0; y < gridSize.y; y++){
                GameObject hex = Instantiate(hexPrefab, transform.position, Quaternion.identity);
                hex.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));
                hex.transform.rotation = Quaternion.Euler(-90, -90, 0);
                hex.GetComponent<HexTile>().Initialize(new Vector2Int(x, y));
                tiles.Add(hex);

                hex.transform.SetParent(transform, true);
            }
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
