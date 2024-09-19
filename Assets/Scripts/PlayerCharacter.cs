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
            Vector3Int direction = new Vector3Int(Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")), Mathf.RoundToInt(Input.GetAxisRaw("Vertical")), 0);
            
            if(CheckTargetCellEmpty(grid.LocalToCell(transform.localPosition) + direction))
            {
                RelativeMoveTo(direction);
                Turn(direction);
            }
            canAct = false;


        }
        else if (canAct && animator.GetBool("Moving"))
        {
            animator.SetBool("Moving", false);
        }
    }

    private static bool PlayerInput()
    {
        return Mathf.Abs(Input.GetAxis("Horizontal")) > 0 || Mathf.Abs(Input.GetAxis("Vertical")) > 0;
    }

    private void OnMoveFinished()
    {
        canAct = true;
        if(animator && !PlayerInput())
        {
            animator.SetBool("Moving", false);
        }
    }
}
