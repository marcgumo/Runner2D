using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public enum characterType
    {
        Player, Enemy, Ship
    }

    [Header("Health Settings")]
    public characterType currentCharacterType;
    public bool friendlyFire = false;
    public Image energyBar;

    [Header("Stats")]
    public int maxHealth;
    int currentHealth;

    UIController _UIController;

    void Start()
    {
        currentHealth = maxHealth;

        _UIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
    }

    public void TakeDamage(int _damage, string _tag)
    {
        if (gameObject.tag != _tag && !friendlyFire)
        {
            return;
        }
        
        currentHealth -= _damage;

        if (currentCharacterType == characterType.Player)
        {
            energyBar.fillAmount = (float)currentHealth / (float)maxHealth;

        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            switch (currentCharacterType)
            {
                case characterType.Player:
                    var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

                    if (player != null)
                    {
                        player.ResetGame();
                    }
                    break;
                case characterType.Enemy:
                    _UIController.SetScore(50);
                    Destroy(gameObject);
                    break;
                case characterType.Ship:
                    _UIController.SetScore(100);
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
