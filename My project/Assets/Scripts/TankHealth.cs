using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TankHealth : MonoBehaviourPun
{
    public float maxHealth = 100f, currentHealth;
    public Image healthBarForeground;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        // Chama o m�todo RPC para aplicar dano
        photonView.RPC("RPCTakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPCTakeDamage(float damageAmount) // Altere para 'float' se estiver usando 'float'
    {
        currentHealth -= damageAmount;
        Debug.Log($"Dano recebido: {damageAmount}, Sa�de atual: {currentHealth}");

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Debug.Log("Jogador morreu.");
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarForeground != null)
        {
            healthBarForeground.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        // Aqui voc� pode adicionar l�gica de morte, como anima��es ou efeitos
        PhotonNetwork.Destroy(gameObject); // Destroi o tanque na rede
    }
}
