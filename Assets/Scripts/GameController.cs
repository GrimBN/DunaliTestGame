using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]private float characterMoveTime = 0.5f;
    public float CharacterMoveTime { get => characterMoveTime;}

    public static GameController Instance;

    private int enemiesLeftToTakeAction;

    private PlayerCharacter player;
    private List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    private void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() 
    {
        TurnStart();
    }
    private void TurnStart()
    {
        enemiesLeftToTakeAction = enemies.Count;
        player.CanAct = true;

        foreach(EnemyCharacter enemy in enemies)
        {
            enemy.CanAct = false;
        }
    }

    public void RegisterEnemy(EnemyCharacter enemy)
    {
        enemies.Add(enemy);
        enemy.ActionInitiated += CharacterTookAction;
        enemy.ActionFinished += CharacterFinishedAction;
    }

    public void RegisterPlayer(PlayerCharacter playerCharacter)
    {
        player = playerCharacter;
        player.ActionInitiated += CharacterTookAction;
        player.ActionFinished += CharacterFinishedAction;
    }

    private void CharacterTookAction(CharacterBase character)
    {
        /* if(character == player)
        {
            AllowOtherCharacterActions();
        }
        else
        {
            enemiesLeftToTakeAction--;
        } */
        character.CanAct = false;
    }

    private void CharacterFinishedAction(CharacterBase character)
    {
        if(character == player)
        {
            AllowOtherCharacterActions();
        }
        else
        {
            enemiesLeftToTakeAction--;
            if(enemiesLeftToTakeAction <= 0)
            {
                ProcessEndTurn();
            }
        }
    }

    private void ProcessEndTurn()
    {
        TurnStart();
    }

    private void AllowOtherCharacterActions()
    {
        foreach(EnemyCharacter enemy in enemies)
        {
            enemy.CanAct = true;
        }
    }
}
