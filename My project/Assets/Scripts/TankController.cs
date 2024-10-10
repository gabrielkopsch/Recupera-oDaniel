using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TankController : MonoBehaviourPun, IDamageable
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 150f;
    public int maxHealth = 100;      // Vida m�xima do tanque
    private int currentHealth;

    
    public Transform firePoint;

    void Start()
    {
        // Inicia o tanque com a vida m�xima
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire();
            }
        }
    }

    // Fun��o para controlar o movimento do tanque
    void HandleMovement()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;

        transform.Translate(Vector3.up * move);
        transform.Rotate(Vector3.forward, -rotate);
    }

    // Fun��o para disparar um proj�til
    void Fire()
    {
        PhotonNetwork.Instantiate("Prefabs/bullet", firePoint.position, firePoint.rotation);
    }

    // Implementa��o da interface IDamageable para receber dano
    public void TakeDamage(int damageAmount)
    {
        // Reduz a vida atual com base no dano recebido
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Fun��o para "morrer" (destruir o tanque ou desabilitar)
    void Die()
    {
        if (photonView.IsMine)
        {
            // Destroi o tanque na rede (ou qualquer outra l�gica de morte)
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
