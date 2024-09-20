using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    [Tooltip("These are the cell locations relative to the enemy's <b>current</b> position on the grid")]
    [SerializeField] private Vector3Int[] relativeWaypoints;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        MoveFinished += OnMoveFinished;
        canAct = true;
        StartCoroutine(Patrol());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Patrol()
    {
        int loopDirection = 1;
        for(int i=0; i < relativeWaypoints.Length && i >=0; i+= loopDirection)
        {
            Vector3Int targetGridPos = GetGridPosFromRelativeCellPos(relativeWaypoints[i]);

            if (animator)
            {
                animator.SetBool("Moving", true);
            }

            Turn(relativeWaypoints[i]);
            LayerMask layerMask = LayerMask.GetMask("Player");

            if (!CheckTargetCellForObjects(targetGridPos, layerMask))
            {
                RelativeMoveTo(relativeWaypoints[i]);
                canAct = false;
                
            }
            else
            {
                Attack();
                canAct = false;

            }
            while (!canAct)
            {
                yield return null;
            }

            if(i == relativeWaypoints.Length - 1)
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
        Debug.Log("Zombie move finished");
        canAct = true;
        if(!animator) { return; }

        animator.SetBool("Moving", false);

    }
}
