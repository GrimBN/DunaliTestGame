using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    private bool canMove;
    void Start()
    {
        Init();
        MoveFinished += OnMoveFinished;
        canMove = true;
    }

    
    void Update()
    {
        if( canMove && (Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0))
        {
            Vector3Int direction = new Vector3Int(Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")), Mathf.RoundToInt(Input.GetAxisRaw("Vertical")), 0);
            //Vector3Int directionInt = new Vector3Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
            RelativeMoveTo(direction);
            canMove = false;
        }
    }

    private void OnMoveFinished()
    {
        canMove = true;
    }
}
