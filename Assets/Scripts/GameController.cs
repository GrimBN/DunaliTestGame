using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField] private float characterMoveTime = 0.5f;
    [SerializeField] private Transform startPosition;
    [SerializeField] private WinArea winArea;
    
    public float CharacterMoveTime { get => characterMoveTime;}

    public static GameController Instance;

    private int enemiesLeftToTakeAction;

    private bool playerHasWon;

    private PlayerCharacter player;
    private List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    public event BasicDelegate WinEvent;
    public event BasicDelegate LoseEvent;

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
        winArea.PlayerEnteredWinArea += ProcessWin;
        //TurnStart();
    }
    
    public void TurnStart()
    {
        enemiesLeftToTakeAction = enemies.Count;
        player.CanAct = true;
        player.TurnTaken = false;

        foreach(EnemyCharacter enemy in enemies)
        {
            enemy.CanAct = false;
            enemy.TurnTaken = false;
        }
    }

    public void RegisterEnemy(EnemyCharacter enemy)
    {
        enemies.Add(enemy);
        enemy.ActionInitiated += CharacterTookAction;
        enemy.ActionFinished += CharacterFinishedAction;
        enemy.CharacterDied += CharacterDeathOccured;
        enemy.CanAct = false;
        enemy.TurnTaken = false;
    }

    public void RegisterPlayer(PlayerCharacter playerCharacter)
    {
        player = playerCharacter;
        player.ActionInitiated += CharacterTookAction;
        player.ActionFinished += CharacterFinishedAction;
        player.CharacterDied += CharacterDeathOccured;
        player.CanAct = false;
        player.TurnTaken = false;
    }

    private void CharacterTookAction(CharacterBase character)
    {
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
        }

        if(enemiesLeftToTakeAction <= 0)
        {
            ProcessEndTurn();
        }
    }

    private void CharacterDeathOccured(CharacterBase character)
    {
        if(character == player)
        {
            Debug.Log(character);
            LoseEvent?.Invoke();
        }
        else
        {
            foreach(EnemyCharacter enemy in enemies)
            {
                if(enemy == character)
                {
                    enemies.Remove(enemy);
                    if(!enemy.TurnTaken)
                    {
                        enemiesLeftToTakeAction--;
                        if (enemiesLeftToTakeAction <= 0)
                        {
                            ProcessEndTurn();
                        }
                    }
                    break;
                }
            }
        }
    }

    private void ProcessEndTurn()
    {
        if(!playerHasWon)
        {
            TurnStart();
        }
    }

    private void AllowOtherCharacterActions()
    {
        foreach(EnemyCharacter enemy in enemies)
        {
            enemy.CanAct = true;
        }
    }

    private void ProcessWin(CharacterBase character)
    {
        playerHasWon = true;
        WinEvent?.Invoke();
    }
}
