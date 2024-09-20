using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: zombie ai, game controller tracking turns, start location and goal;
public class PlayerCharacter : CharacterBase
{
    [SerializeField] private Transform weaponParentRest;
    [SerializeField] private Transform weaponParentAttack;
    
    private Weapon equippedWeapon;

    void Start()
    {
        InitBase();
        MoveFinished += OnMoveFinished;
        canAct = true;
        GameController.Instance.RegisterPlayer(this);
        
    }

    
    void Update()
    {
        if (canAct && PlayerInput()) //Movement
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
        else if(canAct && Input.GetButtonDown("Interact"))  //Check for weapon at current cell
        {
            OnActionInitiated();
            Vector3Int targetGridPos = grid.LocalToCell(transform.localPosition);

            LayerMask layerMask = LayerMask.GetMask("Weapon");
            if(CheckTargetCellForObjects(targetGridPos, layerMask, out Collider hitCollider))
            {
                HandleWeaponEquip(hitCollider);
            }
        }
        else if(canAct && Input.GetButtonDown("Fire")) //Attack
        {
            OnActionInitiated();
            Vector3Int attackTargetCell = grid.WorldToCell(transform.position + transform.forward);
            
            equippedWeapon.transform.SetParent(weaponParentAttack);
            equippedWeapon.transform.SetLocalPositionAndRotation(Vector3.zero + equippedWeapon.WeaponAttackOffset, Quaternion.identity);

            LayerMask layerMask = LayerMask.GetMask("Enemy");
            Attack(equippedWeapon.Damage, equippedWeapon.DamageType, attackTargetCell, layerMask);

        }
        /* else if (canAct && animator.GetBool("Moving"))
        {
            animator.SetBool("Moving", false);
            //animMarkedForDisable = false;
        } */
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
            }
            equippedWeapon = weapon;
            equippedWeapon.transform.SetParent(weaponParentRest);
            equippedWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
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
