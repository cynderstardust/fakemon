using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Tilemap groundTilemap;
    public MapManager mapManager;

    public bool moving = false;
    public float moveSpeed = 1f;
    Vector3Int movementDestination;
    string moveDirection;
    int playerZPosition = -1;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving)
        {
            ProcessInput();
        }
        else
        {
            MoveCharacter();
        }        
    }

    void ProcessInput()
    {
        Vector3Int? newPosition = null;
        int xPos = (int)gameObject.transform.position.x;
        int yPos = (int)gameObject.transform.position.y;

        //Move Right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition = new Vector3Int(xPos + 1, yPos, playerZPosition);
            moveDirection = "right";
        }

        //Move Left
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition = new Vector3Int(xPos - 1, yPos, playerZPosition);
            moveDirection = "left";
        }

        //Move Up
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition = new Vector3Int(xPos, yPos + 1, playerZPosition);
            moveDirection = "up";
        }

        //Move Down
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition = new Vector3Int(xPos, yPos - 1, playerZPosition);
            moveDirection = "down";
        }

        if (newPosition == null) return;        
        Debug.Log($"New pos: {newPosition}");

        string destinationName = GetDestinationTileName((Vector3Int)newPosition);
        if (destinationName.Equals("water")) return;

        movementDestination = (Vector3Int)newPosition;
        moving = true;                 
    }

    void MoveCharacter()
    {
        Vector2 moveVector = movementDestination - gameObject.transform.position;
        Vector2 scaledMoveVector = moveVector.normalized * moveSpeed * Time.deltaTime;

        gameObject.transform.position += new Vector3(scaledMoveVector.x, scaledMoveVector.y, 0);

        //Check if arrived at new tile
        bool moveComplete = false;
        switch (moveDirection)
        {
            case "right":
                if (gameObject.transform.position.x > movementDestination.x) moveComplete = true;                
                break;

            case "left":
                if (gameObject.transform.position.x < movementDestination.x) moveComplete = true;
                break;
                
            case "up":
                if (gameObject.transform.position.y > movementDestination.y) moveComplete = true;
                break;

            case "down":
                if (gameObject.transform.position.y < movementDestination.y) moveComplete = true;
                break;
        }

        if (moveComplete)
        {
            gameObject.transform.position = movementDestination;
            moving = false;
            OnMoveComplete();
        }
    }

    void OnMoveComplete()
    {
        LogTile();
    }

    void LogTile()
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position);

        TileBase clickedTile = groundTilemap.GetTile(gridPosition);

        Debug.Log($"Moved to at {(Vector2Int)gridPosition}, tile: {clickedTile.name}");
    }

    string GetDestinationTileName(Vector3Int destination)
    {
        return groundTilemap.GetTile(new Vector3Int(destination.x, destination.y, 0)).name;
    }
}