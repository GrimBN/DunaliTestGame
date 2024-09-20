using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;

    private void Start() 
    {
        GameController.Instance.WinEvent += WinUI;
        GameController.Instance.LoseEvent += LoseUI;

        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);

    }

    private void WinUI()
    {
        winText.gameObject.SetActive(true);
    }

    private void LoseUI()
    {
        loseText.gameObject.SetActive(true);
    }
}
