using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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

    public bool CanAct { get=>canAct; set=>canAct = value;}

    public event BasicDelegate MoveFinished;
    public event ActionDelegate ActionInitiated;
    public event ActionDelegate ActionFinished;

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
        //Vector3 posCheck = grid.LocalToWorld(GetLocalPosFromCellPos(cellPosition));
        //Vector3 direction = posCheck - boxCastCenter;

        Vector3 boxCenter = bounds.center + Vector3.one * 0.5f;
        Collider[] colliders = Physics.OverlapBox(boxCenter, bounds.extents * 0.8f, Quaternion.identity, layerMask);
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

        while ( timer < GameController.Instance.CharacterMoveTime)// && Vector3.Distance(transform.localPosition, targetPos) > 0.01f)
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

    public void Attack(int damage, DamageTypes damageType, Vector3Int targetGridPos, LayerMask attackLayer)
    {
        if(animator)
        {
            animator.SetTrigger("Attack");
        }

        if(CheckTargetCellForObjects(targetGridPos, attackLayer, out Collider hitCollider))
        {
            if(hitCollider.TryGetComponent<CharacterBase>(out CharacterBase hitCharacter))
            {
                hitCharacter.HandleHit(damage, damageType);
            }
        }
    }
    public void HandleHit( int damage, DamageTypes damageType = DamageTypes.Slash)
    {
        float resistanceTouse;

        switch(damageType)
        {
            case DamageTypes.Slash:
                resistanceTouse = slashResistance;
                break;
            case DamageTypes.Blunt:
                resistanceTouse = bluntResistance;
                break;
            default:
                resistanceTouse = 100f;
                break;
        }

        float modifedDamage = damage * resistanceTouse / 100f;
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
        ActionFinished?.Invoke(this);
    }

}
