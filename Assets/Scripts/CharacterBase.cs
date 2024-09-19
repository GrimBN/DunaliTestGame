using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public delegate void  MoveDelegate();

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Grid grid;
    [SerializeField] protected LayerMask layerMask1;

    protected int currentHealth;

    protected bool isAlive;

    private Vector3 gizmoPoint;

    public event MoveDelegate MoveFinished;

    protected void Init()
    {
        currentHealth = maxHealth;
        isAlive = true;
        grid = GetComponentInParent<Grid>();
        transform.localPosition = GetLocalPosFromCellPos(Vector3Int.zero);
        animator = GetComponent<Animator>();

    }
    private void OnDrawGizmos() 
    {
        Gizmos.DrawCube(gizmoPoint, Vector3.one * 0.8f);
    }

    private void Start() 
    {
        Init();
    }

    protected void RelativeMoveTo(Vector3Int relativeGridPosition)
    {
        Vector3 targetPos = GetLocalPosFromCellPos(relativeGridPosition);

        StartCoroutine(MoveCoroutine(targetPos));
    }

    protected Vector3 GetLocalPosFromCellPos(Vector3Int cellPosition)
    {
        Vector3Int currentGridPos = grid.LocalToCell(transform.localPosition);
        Vector3Int targetGridPos = currentGridPos + cellPosition;
        Vector3 targetPos = new Vector3(grid.GetCellCenterLocal(targetGridPos).x, 0f, grid.GetCellCenterLocal(targetGridPos).z);
        return targetPos;
    }

    protected bool CheckTargetCellEmpty(Vector3Int cellPosition)
    {
        Bounds bounds = grid.GetBoundsLocal(cellPosition) ;
        //Vector3 posCheck = grid.LocalToWorld(GetLocalPosFromCellPos(cellPosition));
        //Vector3 direction = posCheck - boxCastCenter;

        LayerMask layerMask = LayerMask.GetMask("Hurdle");
        gizmoPoint = bounds.center + Vector3.one * 0.5f;
        Collider[] colliders = Physics.OverlapBox(gizmoPoint, bounds.extents * 0.8f, Quaternion.identity, layerMask);
        if (colliders.Length > 0)
        {
            Debug.Log(colliders[0].gameObject.name);
            return false;
        }
        return true;
    }

    private IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        Vector3 startPos = transform.localPosition;
        float speed = Vector3.Distance(startPos, targetPos) / GameController.Instance.CharacterMoveTime;
        float timer = 0f;

        while ( timer < GameController.Instance.CharacterMoveTime)// && Vector3.Distance(transform.localPosition, targetPos) > 0.01f)
        {
            Vector3 step = Vector3.MoveTowards(transform.localPosition, targetPos, speed * Time.deltaTime);
            transform.localPosition = step;
            //Debug.Log(step);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPos;

        MoveFinished?.Invoke();
    }

    protected void Turn(Vector3Int relativeGridPosition)
    {
        Vector3 targetPos = GetLocalPosFromCellPos(relativeGridPosition);
        transform.LookAt(targetPos);
    }

    public void Attack()
    {

    }

    public void ApplyDamage(int damage)
    {
        if(isAlive && damage > 0)
        {
            currentHealth -= damage;

            if(currentHealth <= 0)
            {
                HandleDeath();
            }
        }
    }

    private void HandleDeath()
    {
        isAlive = true;
    }

}
