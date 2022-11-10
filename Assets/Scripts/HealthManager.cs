using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public enum CharacterType
    {
        Player, Enemy, Ship
    }

    [Header("Health Settings")]
    public CharacterType currentCharacterType;
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

    public void GiveHealth(int health)
    {
        currentHealth += health;
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        energyBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    public void TakeDamage(int _damage, string _tag)
    {
        if (gameObject.tag != _tag && !friendlyFire)
        {
            return;
        }
        
        currentHealth -= _damage;

        if (currentCharacterType == CharacterType.Player)
        {
            energyBar.fillAmount = (float)currentHealth / (float)maxHealth;
            Camera.main.GetComponent<CameraController>().SetDamageEffect();
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            switch (currentCharacterType)
            {
                case CharacterType.Player:
                    var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

                    if (player != null)
                    {
                        player.ResetGame();
                    }
                    break;
                case CharacterType.Enemy:
                    _UIController.SetScore(50);
                    gameObject.GetComponent<EnemyController>().Dead();
                    Destroy(gameObject, 2f);
                    break;
                case CharacterType.Ship:
                    _UIController.SetScore(50);
                    Destroy(gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
