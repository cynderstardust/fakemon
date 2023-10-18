using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Tilemap groundTilemap, objectsTilemap;
    public MapManager mapManager;

    public Animator animator;
    public GameObject InteractableObjectContainer;

    public bool moving = false;
    public float moveSpeed = 1f;
    Vector3Int movementDestination;
    string moveDirection;
    int playerZPosition = -1;

    void Start()
    {
        animator.speed = 0;
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
        // Debug.Log($"New pos: {newPosition}");

        //Check for an interactable object, and if there is one, trigger the interaction then return
        bool haveInteracted = CheckForInteractableObject((Vector3Int)newPosition);
        if (haveInteracted) return;

        //Check that the ground tile is passable
        string destinationName = GetDestinationTileName((Vector3Int)newPosition, groundTilemap);
        if (destinationName.Equals("water")) return;

        destinationName = GetDestinationTileName((Vector3Int)newPosition, objectsTilemap);
        if (destinationName.Length > 0)
        {
            Debug.Log($"Moving to item type: {destinationName}");
            return;
        }



        movementDestination = (Vector3Int)newPosition;
        moving = true;                 
    }

    bool CheckForInteractableObject(Vector3Int newPosition)
    {
        foreach (Transform child in InteractableObjectContainer.transform)
        {
            //Check the coordinates and see if the player is going to move into this cell
            if ((int)child.position.x != newPosition.x || (int)child.position.y != newPosition.y) continue;

            //Make sure there is an interactable script on the object
            InteractableObject interactable = child.GetComponent<InteractableObject>();
            if (interactable == null) continue;

            interactable.Interact();
            return true;           
        }

        return false;
    }

    void MoveCharacter()
    {
        Vector2 moveVector = movementDestination - gameObject.transform.position;
        Vector2 scaledMoveVector = moveVector.normalized * moveSpeed * Time.deltaTime;

        gameObject.transform.position += new Vector3(scaledMoveVector.x, scaledMoveVector.y, 0);

        //Check if arrived at new tile
        bool moveComplete = false;

        animator.speed = 1;
        switch (moveDirection)
        {          
            case "right":
                animator.SetInteger("State", 3);                
                if (gameObject.transform.position.x > movementDestination.x) moveComplete = true;                
                break;

            case "left":
                animator.SetInteger("State", 1);
                if (gameObject.transform.position.x < movementDestination.x) moveComplete = true;
                break;
                
            case "up":
                animator.SetInteger("State", 2);
                if (gameObject.transform.position.y > movementDestination.y) moveComplete = true;
                break;

            case "down":
                animator.SetInteger("State", 0);
                if (gameObject.transform.position.y < movementDestination.y) moveComplete = true;
                break;
        }

        if (moveComplete)
        {
            gameObject.transform.position = movementDestination;
            moving = false;
            animator.speed = 0;
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

    string GetDestinationTileName(Vector3Int destination, Tilemap tilemap)
    {
        var tile = tilemap.GetTile(new Vector3Int(destination.x, destination.y, 0));
        if (tile == null) return "";
        return tile.name;
    }
}
