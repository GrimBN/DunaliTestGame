using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void  BasicDelegate();

public delegate void ActionDelegate(CharacterBase character);

/// <summary>
/// Base class for entities that can move and attack
/// </summary>
public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField, Range(0f, 100f)] protected float slashResistance;
    [SerializeField, Range(0f, 100f)] protected float bluntResistance;
    protected Animator animator;
    protected Grid grid;

    protected int currentHealth;
    protected bool isAlive;
    protected bool canAct;
    public bool CanAct { get => canAct; set => canAct = value; }
    private bool turnTaken;
    public bool TurnTaken { get => turnTaken; set => turnTaken = value; }

    public event BasicDelegate MoveFinished;
    public event BasicDelegate AttackFinished;
    public event ActionDelegate ActionInitiated;
    public event ActionDelegate ActionFinished;
    public event ActionDelegate CharacterDied;

    //private Vector3 gizmoCenter, gizmoSize;

    protected void InitBase()
    {
        currentHealth = maxHealth;
        isAlive = true;
        grid = GetComponentInParent<Grid>();
        transform.localPosition = GetLocalPosFromRelativeCellPos(Vector3Int.zero);
        animator = GetComponent<Animator>();

    }

    private void Start() 
    {
        InitBase();
    }
/* 
    private void OnDrawGizmos() 
    {
        Gizmos.DrawCube(gizmoCenter, gizmoSize);
    } */

/// <summary>
/// Makes the character move to a location on the grid
/// </summary>
/// <param name="targetGridPos"> The position on the grid to move to</param>
    protected void MoveTo(Vector3Int targetGridPos )
    {
        Vector3 targetPos = new Vector3(grid.GetCellCenterLocal(targetGridPos).x, 0f, grid.GetCellCenterLocal(targetGridPos).z);

        StartCoroutine(MoveCoroutine(targetPos));
    }

/// <summary>
/// Returns the local position of a cell relative to the character's current grid position
/// </summary>
/// <param name="relativeGridPosition"></param>
/// <returns>Local position of cell as a Vector3</returns>
    protected Vector3 GetLocalPosFromRelativeCellPos(Vector3Int relativeGridPosition)
    {
        Vector3Int targetGridPos = GetGridPosFromRelativeCellPos(relativeGridPosition);
        Vector3 targetPos = new Vector3(grid.GetCellCenterLocal(targetGridPos).x, 0f, grid.GetCellCenterLocal(targetGridPos).z);
        return targetPos;
    }

/// <summary>
/// Returns the grid position of a cell relative to the character's current grid position
/// </summary>
/// <param name="relativeGridPosition"></param>
/// <returns>Grid position of cell as Vector3Int</returns>
    protected Vector3Int GetGridPosFromRelativeCellPos(Vector3Int relativeGridPosition)
    {
        Vector3Int currentGridPos = grid.LocalToCell(transform.localPosition);
        Vector3Int targetGridPos = currentGridPos + relativeGridPosition;
        return targetGridPos;
    }

/// <summary>
/// Checks if the relative cell position contains an object or not defined by the layer mask. Can detect objects that are 
/// big enough to considerably spill over into this cell.
/// </summary>
/// <param name="relativeGridPosition"></param>
/// <param name="layerMask">Layer mask that defines which type of objects to check for</param>
/// <returns>Boolean value for if an object was detected: True for one or more objects, False for none</returns>
    protected bool CheckTargetCellForObjects(Vector3Int relativeGridPosition, LayerMask layerMask, out Collider hitCollider)
    {
        Bounds bounds = grid.GetBoundsLocal(relativeGridPosition) ;
        Vector3 boxCenter = bounds.center + Vector3.one * 0.5f;
        Vector3 boxSize =  new Vector3(bounds.extents.x * 0.8f, bounds.extents.y * 2f, bounds.extents.z * 0.8f);
        //gizmoCenter = boxCenter;
        //gizmoSize = boxSize;

        Collider[] colliders = Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity, layerMask);
        hitCollider = null;

        if (colliders.Length > 0)
        {
            hitCollider = colliders[0];
            Debug.Log(colliders[0].gameObject.name);
        }

        return colliders.Length > 0;
    }

    private IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        if (animator)
        {
            animator.SetBool("Moving", true);
        }
        Vector3 startPos = transform.localPosition;
        float speed = Vector3.Distance(startPos, targetPos) / GameController.Instance.CharacterMoveTime;
        float timer = 0f;

        while ( timer < GameController.Instance.CharacterMoveTime)
        {
            Vector3 step = Vector3.MoveTowards(transform.localPosition, targetPos, speed * Time.deltaTime);
            transform.localPosition = step;
            //Debug.Log(step);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPos;

        if (animator)
        {
            animator.SetBool("Moving", false);
        }

        MoveFinished?.Invoke();
    }

/// <summary>
/// Turns the character to face the relative grid position
/// </summary>
/// <param name="relativeGridPosition"></param>
    protected void Turn(Vector3Int relativeGridPosition)
    {
        Vector3 targetPos = GetLocalPosFromRelativeCellPos(relativeGridPosition);
        transform.LookAt(targetPos);
    }

/// <summary>
/// Tries to attack a particular type of entity at the given cell
/// </summary>
/// <param name="damage">Damage applied by the attack</param>
/// <param name="damageType">The type of damage; can be slashing or blunt with the recipient potentially having different resistances to each</param>
/// <param name="targetGridPos">The cell to attack</param>
/// <param name="attackLayer">The type of entity to attack defined by passing in the layer assigned to it: Player or Enemy</param>
    public void Attack(int damage, DamageTypes damageType, Vector3Int targetGridPos, LayerMask attackLayer)
    {
        if(animator)
        {
            animator.SetTrigger("Attack");
        }

        if(CheckTargetCellForObjects(targetGridPos, attackLayer, out Collider hitCollider))
        {
            if(hitCollider.gameObject.TryGetComponent(out CharacterBase hitCharacter))
            {
                hitCharacter.HandleHit(damage, damageType);
            }
        }
        
        AttackFinished?.Invoke();
    }
    public void HandleHit( int damage, DamageTypes damageType = DamageTypes.Slash)
    {
        var resistanceTouse = damageType switch //Fancy new switch expression that I'm learning about for the first time
        {
            DamageTypes.Slash => slashResistance,
            DamageTypes.Blunt => bluntResistance,
            _ => 100f,
        };

        float modifedDamage = damage * (100f - resistanceTouse) / 100f;
        ApplyDamage(Mathf.FloorToInt(modifedDamage));
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
        isAlive = false;
        animator.SetTrigger("Death");
        CharacterDied?.Invoke(this);
    }

    protected virtual void OnActionInitiated()
    {
        if (animator)
        {
            animator.speed = 1 / (GameController.Instance.CharacterMoveTime * 2.5f);
        }
        ActionInitiated?.Invoke(this);
    }

    protected virtual void OnActionFinished()
    {
        if (animator)
        {
            animator.speed = 1;
        }
        turnTaken = true;
        ActionFinished?.Invoke(this);
    }

}
