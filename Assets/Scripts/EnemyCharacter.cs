using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    [SerializeField] private int attackDamage;

    [Tooltip("These are the cell locations relative to the enemy's <b>STARTING</b> position on the grid." +
        "\nThe enemy automatically traces its path back upon reaching the end and then loops (including the starting position).")]
    [SerializeField] private Vector3Int[] waypoints;
    private Vector3Int[] modifiedWaypoints;

    private Vector3Int startGridPos;

    private Coroutine patrolCoroutine;

    void Start()
    {
        InitBase();
        Init();
        patrolCoroutine = StartCoroutine(Patrol());
    }

    private void Init()
    {
        startGridPos = grid.LocalToCell(transform.localPosition);

        modifiedWaypoints = new Vector3Int[waypoints.Length + 1];
        waypoints.CopyTo(modifiedWaypoints, 1); //Adds the starting position to the waypoints

        GameController.Instance.RegisterEnemy(this);

        MoveFinished += OnMoveFinished;
        CharacterDied += OnDeath;
    }

    private IEnumerator Patrol()
    {
        int loopDirection = 1;
        for(int i = 1; i < modifiedWaypoints.Length && i >=0; ) //Start at index 1 to skip start position
        {
            while (!canAct) //yield condition that makes enemy wait until next available turn
            {
                yield return null;
            }

            OnActionInitiated();
            Vector3Int currentGridPos = grid.LocalToCell(transform.localPosition);
            Vector3Int targetGridPos = startGridPos + modifiedWaypoints[i];
            Vector3Int relativeGridPos = targetGridPos - currentGridPos;

            Turn(relativeGridPos);

            LayerMask layerMask = LayerMask.GetMask("Player");

            if (!CheckTargetCellForObjects(targetGridPos, layerMask, out Collider hitCollider))
            {
                MoveTo(targetGridPos);
                //canAct = false;
            }
            else
            {
                Attack(attackDamage, DamageTypes.Slash, targetGridPos, layerMask);
                OnActionFinished();
                continue; //Don't want to skip a movement if spending a turn attacking therefore skipping the iterator increment at the bottom
            }

            

            if(i == modifiedWaypoints.Length - 1)
            {
                loopDirection = -1;
            }
            else if(i == 0)
            {
                loopDirection = 1;
                
            }
            i+= loopDirection;
        }
    }

    private void OnMoveFinished()
    {
        //canAct = true;
        OnActionFinished();

        /* if(!animator) { return; }

        animator.SetBool("Moving", false); */
    }

    private void OnDeath(CharacterBase character)
    {
        StopCoroutine(patrolCoroutine);
    }
}
