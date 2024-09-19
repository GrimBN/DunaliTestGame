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

    protected void Init()
    {
        currentHealth = maxHealth;
        isAlive = true;
        grid = GetComponentInParent<Grid>();
        Vector3Int currentGridPos = grid.WorldToCell(transform.localPosition);

    }

    private void Start() 
    {
        Init();
    }

    public void RelativeMoveTo(Vector3Int relativeGridPosition)
    {
        Vector3Int currentGridPos = grid.LocalToCell(transform.localPosition);
        Vector3Int targetGridPos = currentGridPos + relativeGridPosition;
        Vector3 targetPos = new Vector3(grid.GetCellCenterLocal(targetGridPos).x, 0f, grid.GetCellCenterLocal(targetGridPos).z) ;

         Debug.Log("Current grid pos:" + currentGridPos + "\nTarget grid pos:" + targetGridPos + "\nTarget Local pos:" + targetPos);

        StartCoroutine(MoveCoroutine(targetPos));
        
    }

    public IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        Vector3 startPos = transform.localPosition;
        float speed = Vector3.Distance(startPos, targetPos) / GameController.Instance.CharacterMoveTime;
        float timer = 0f;
        while( timer < GameController.Instance.CharacterMoveTime && Vector3.Distance(transform.localPosition, targetPos) > 0.01f)
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
