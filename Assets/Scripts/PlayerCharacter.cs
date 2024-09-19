using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    private bool canAct;
    void Start()
    {
        Init();
        MoveFinished += OnMoveFinished;
        canAct = true;
    }

    
    void Update()
    {
        if (canAct && PlayerInput())
        {
            Vector3Int direction = Vector3Int.RoundToInt(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0));
            Vector3Int currentGridPos = grid.LocalToCell(transform.localPosition);
            Vector3Int targetGridPos = currentGridPos + direction;

            Turn(direction);
            if (animator)
            {
                animator.SetBool("Moving", true);
            }
            
            if (CheckTargetCellEmpty(targetGridPos))
            {

                
                
                RelativeMoveTo(direction);
                canAct = false;
            }
        }
        else if (canAct && animator.GetBool("Moving"))
        {
            animator.SetBool("Moving", false);
            //animMarkedForDisable = false;
        }
    }

    private static bool PlayerInput()
    {
        return Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0;
    }

    private void OnMoveFinished()
    {
        canAct = true;
        if(!animator) { return; }

        if(!PlayerInput())
        {
            animator.SetBool("Moving", false);
        }
    }
}
