using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinArea : MonoBehaviour
{
    public event ActionDelegate PlayerEnteredWinArea;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.TryGetComponent(out PlayerCharacter player))
        {
            PlayerEnteredWinArea?.Invoke(player);
        }
    }
}
