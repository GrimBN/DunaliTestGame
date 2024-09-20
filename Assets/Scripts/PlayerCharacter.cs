using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: zombie ai, game controller tracking turns, start location and goal;
public class PlayerCharacter : CharacterBase
{
    [SerializeField] private Transform weaponParentRest;
    [SerializeField] private Transform weaponParentAttack;
    [SerializeField] private AnimationClip attackAnimationClip;
    
    private Weapon equippedWeapon;

    protected AnimationEvent AttackAnimationFinished;

    void Start()
    {
        InitBase();
        MoveFinished += OnMoveFinished;
        //AttackFinished += OnAttackFinished;
        canAct = true;
        GameController.Instance.RegisterPlayer(this);

        AttackAnimationFinished = new AnimationEvent();
        AttackAnimationFinished.time = attackAnimationClip.length;
        AttackAnimationFinished.functionName = "OnAttackFinished";
        attackAnimationClip.AddEvent(AttackAnimationFinished);
    }

    
    void Update()
    {
        if (isAlive && canAct && (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0)) //Movement
        {
            HandleMovement();
        }
        else if(isAlive && canAct && Input.GetButtonDown("Interact"))  //Checks for weapon at current cell
        {
            HandleWeaponPickup();
        }
        else if(isAlive && canAct && Input.GetButtonDown("Fire")) //Attack
        {
            HandleAttack();
        }
    }

    private void HandleMovement()
    {
        OnActionInitiated();
        Vector3Int direction = Vector3Int.RoundToInt(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0));
        Vector3Int targetGridPos = GetGridPosFromRelativeCellPos(direction);

        Turn(direction);

        LayerMask layerMask = LayerMask.GetMask("Hurdle");
        if (!CheckTargetCellForObjects(targetGridPos, layerMask, out Collider hitCollider))
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

    private void HandleWeaponPickup()
    {
        OnActionInitiated();
        Vector3Int targetGridPos = grid.LocalToCell(transform.localPosition);

        LayerMask layerMask = LayerMask.GetMask("Weapon");
        if (CheckTargetCellForObjects(targetGridPos, layerMask, out Collider hitCollider))
        {
            HandleWeaponEquip(hitCollider);
        }
        OnActionFinished();
    }

    private void HandleAttack()
    {
        OnActionInitiated();
        Vector3Int attackTargetCell = grid.WorldToCell(transform.position + transform.forward);

        if (equippedWeapon)
        {
            equippedWeapon.transform.SetParent(weaponParentAttack);
            equippedWeapon.transform.SetLocalPositionAndRotation(Vector3.zero + equippedWeapon.WeaponAttackOffset, Quaternion.identity);
            LayerMask layerMask = LayerMask.GetMask("Enemy");
            Attack(equippedWeapon.Damage, equippedWeapon.DamageType, attackTargetCell, layerMask);
        }

        OnActionFinished();
    }

    private void HandleWeaponEquip(Collider hitCollider)
    {
        Weapon weapon = hitCollider.gameObject.GetComponent<Weapon>();
        if (weapon)
        {
            if(equippedWeapon)
            {
                equippedWeapon.transform.SetParent(weapon.transform.parent);
                equippedWeapon.transform.SetLocalPositionAndRotation(weapon.transform.localPosition, weapon.transform.localRotation);
                equippedWeapon.GetComponent<BoxCollider>().enabled = true; //Enable dropped weapon's collider to allow picking it back up
            }
            equippedWeapon = weapon;
            equippedWeapon.transform.SetParent(weaponParentRest);
            equippedWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            hitCollider.enabled = false;    //Disable the equipped weapons collider to prevent interference with other weapon pickups
        }
    }

    private void OnMoveFinished()
    {
        OnActionFinished();
    }

    private void OnAttackFinished()
    {
        if(equippedWeapon)
        {
            equippedWeapon.transform.SetParent(weaponParentRest);
            equippedWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
