using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: zombie ai, game controller tracking turns, start location and goal;
public class PlayerCharacter : CharacterBase
{
    void Start()
    {
        InitBase();
        MoveFinished += OnMoveFinished;
        canAct = true;
        GameController.Instance.RegisterPlayer(this);
        
    }

    
    void Update()
    {
        if (canAct && PlayerInput())
        {
            OnActionInitiated();
            Vector3Int direction = Vector3Int.RoundToInt(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0));
            Vector3Int targetGridPos = GetGridPosFromRelativeCellPos(direction);

            Turn(direction);
            
            LayerMask layerMask = LayerMask.GetMask("Hurdle");
            if (!CheckTargetCellForObjects(targetGridPos, layerMask))
            {
                
                MoveTo(targetGridPos);
                //canAct = false;
            }
            else
            {
                animator.SetTrigger("Walk");
                OnActionFinished();
            }

            
        }
        /* else if (canAct && animator.GetBool("Moving"))
        {
            animator.SetBool("Moving", false);
            //animMarkedForDisable = false;
        } */
    }

    private static bool PlayerInput()
    {
        return Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0;
    }

    private void OnMoveFinished()
    {
        //canAct = true;
        OnActionFinished();

        /* if(!animator) { return; }

        animator.SetBool("Moving", false); */


    }
}
