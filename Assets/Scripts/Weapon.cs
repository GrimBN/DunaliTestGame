using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageTypes {Slash, Blunt};
public class Weapon : MonoBehaviour
{
    [SerializeField] private DamageTypes damageType;
    [SerializeField] private Vector3 weaponAttackOffset;
    public Vector3 WeaponAttackOffset { get => weaponAttackOffset;}
    

    public DamageTypes DamageType { get=>damageType;}

    private int damage;

    public int Damage { get => damage;}
}
