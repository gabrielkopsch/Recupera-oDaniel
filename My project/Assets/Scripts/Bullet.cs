using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat.UtilityScripts;

public class Bullet : MonoBehaviourPun, IWeapon 
{
    public GameObject bulletPrefab;   // Prefab do projétil
    public Transform firePoint;       // Ponto de onde o projétil será disparado
    public float reloadTime = 1f;     // Tempo de recarga entre disparos
    private float nextFireTime = 0f;  // Controla o tempo até o próximo disparo
    public float bulletSpeed = 10f;   // Velocidade da bala
<<<<<<< Updated upstream
=======
    public float damageAmount = 20f;  // Dano causado pela bala
    
>>>>>>> Stashed changes

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Conecta ao Photon
    }
    // Implementação da propriedade de recarga da interface
    public float ReloadTime
    {
        get { return reloadTime; }
    }

    void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
<<<<<<< Updated upstream
        // Aqui pode ir a lógica de dano
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // Destroi a bala na rede
        }
    }

        // Implementação do método Fire da interface IWeapon
=======
        // Verifica se o objeto colidido é um projétil
        if (collision.gameObject.CompareTag("Tank"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            TankController tankController = collision.gameObject.GetComponent<TankController>();

            // Aplica dano ao tanque, passando o dano da bala
            tankController.TakeDamage( bullet.damageAmount);
            // Destroi o projétil após a colisão
            PhotonNetwork.Destroy(gameObject);
            Debug.Log("colisao detectada");

        }
    }


>>>>>>> Stashed changes
    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            // Instancia o projétil na rede
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);

            // Aplica movimento para frente na bala
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = firePoint.up * bulletSpeed;  // Move a bala na direção do firePoint
            }

            nextFireTime = Time.time + reloadTime;
        }
    }
}