using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public delegate void  MoveDelegate();

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected AnimatorController animatorController;
    [SerializeField] protected Grid grid;

    protected int currentHealth;

    protected bool isAlive;

    public event MoveDelegate MoveFinished;

    private void Init()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    public void RelativeMoveTo(Vector3Int relativeGridPosition)
    {
        Vector3Int currentGridPos = grid.WorldToCell(transform.position);
        Vector3Int targetGridPos = currentGridPos + relativeGridPosition;
        Vector3 targetPos = grid.CellToWorld(targetGridPos);

        StartCoroutine(MoveCoroutine(targetPos));
        
    }

    public IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float speed = Vector3.Distance(startPos, targetPos) / GameController.CharacterMoveTime;
        while(Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, speed);
            transform.position = newPos;
            yield return null;
        }

        transform.position = targetPos;

        MoveFinished?.Invoke();
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
