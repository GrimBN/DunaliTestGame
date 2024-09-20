using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    [Tooltip("These are the cell locations relative to the enemy's <b>STARTING</b> position on the grid." +
        "\nThe enemy automatically traces its path back upon reaching the end and then loops (including the starting position).")]
    [SerializeField] private Vector3Int[] waypoints;
    private Vector3Int[] modifiedWaypoints;

    private Vector3Int startGridPos;

    void Start()
    {
        InitBase();
        Init();
        StartCoroutine(Patrol());
    }

    private void Init()
    {
        startGridPos = grid.LocalToCell(transform.localPosition);

        modifiedWaypoints = new Vector3Int[waypoints.Length + 1];
        waypoints.CopyTo(modifiedWaypoints, 1); //Adds the starting position to the waypoints

        GameController.Instance.RegisterEnemy(this);

        MoveFinished += OnMoveFinished;
        //canAct = true;
    }

    private IEnumerator Patrol()
    {
        int loopDirection = 1;
        for(int i = 1; i < modifiedWaypoints.Length && i >=0; i+= loopDirection) //Start at index 1 to skip start position
        {
            while (!canAct) //yield condition that makes enemy wait until next available turn
            {
                yield return null;
            }

            OnActionInitiated();
            Vector3Int currentGridPos = grid.LocalToCell(transform.localPosition);
            Vector3Int targetGridPos = startGridPos + modifiedWaypoints[i];

            /* if (animator)
            {
                animator.SetBool("Moving", true);
            } */

            Vector3Int relativeGridPos = targetGridPos - currentGridPos;
            Turn(relativeGridPos);

            LayerMask layerMask = LayerMask.GetMask("Player");

            if (!CheckTargetCellForObjects(targetGridPos, layerMask))
            {
                MoveTo(targetGridPos);
                //canAct = false;
            }
            else
            {
                Attack();           //WIP
                OnActionFinished();
            }

            

            if(i == modifiedWaypoints.Length - 1)
            {
                loopDirection = -1;
            }
            else if(i == 0)
            {
                loopDirection = 1;
                
            }
        }
    }

    private void OnMoveFinished()
    {
        //canAct = true;
        OnActionFinished();

        /* if(!animator) { return; }

        animator.SetBool("Moving", false); */
        

    }
}
