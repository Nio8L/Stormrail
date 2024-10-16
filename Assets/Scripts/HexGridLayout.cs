using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize;

    [Header("Hex Settings")]
    public Material material;

    public float innerSize = 0;
    public float outerSize = 1;
    public float height = 0.25f;
    public bool isFlatTopped;

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
                GameObject hex = new GameObject($"Hex {x},{y}", typeof(HexRenderer));
                hex.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));
                tiles.Add(hex);

                HexRenderer hexRenderer = hex.GetComponent<HexRenderer>();
                hexRenderer.isFlatTopped = isFlatTopped;
                hexRenderer.outerSize = outerSize;
                hexRenderer.innerSize = innerSize;
                hexRenderer.height = height;
                hexRenderer.SetMaterial(material);
                hexRenderer.DrawMesh();

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
        float size = outerSize;

        if(!isFlatTopped){
            shouldOffset = row % 2 == 0;
            width = Mathf.Sqrt(3) * size;
            height = 2f * size;

            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);

            offset = shouldOffset ? width / 2 : 0;

            xPosition = column * horizontalDistance + offset;
            yPosition = row * verticalDistance;
        }else{
            shouldOffset = column % 2 == 0;
            width = 2f * size;
            height = Mathf.Sqrt(3) * size;

            horizontalDistance = width * (3f / 4f);
            verticalDistance = height;

            offset = shouldOffset ? height / 2 : 0;
            xPosition = column * horizontalDistance;
            yPosition = row * verticalDistance - offset;
        }
        
        return new Vector3(xPosition, 0, -yPosition);
    }
}
