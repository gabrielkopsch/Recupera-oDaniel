using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun, IWeapon
{
    public GameObject bulletPrefab;   // Prefab do proj�til
    public Transform firePoint;       // Ponto de onde o proj�til ser� disparado
    public float reloadTime = 1f;     // Tempo de recarga entre disparos
    private float nextFireTime = 0f;  // Controla o tempo at� o pr�ximo disparo
    public float bulletSpeed = 10f;    // Velocidade da bala

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Conecta ao Photon
    }

    // Implementa��o da propriedade de recarga da interface
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
        // Aqui pode ir a l�gica de dano
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // Destroi a bala na rede
        }
    }

    // Implementa��o do m�todo Fire da interface IWeapon
    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            // Chama o m�todo RPC para disparar a bala
            photonView.RPC("RPCFireBullet", RpcTarget.All, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + reloadTime;
        }
    }

    [PunRPC]
    void RPCFireBullet(Vector3 position, Quaternion rotation)
    {
        // Instancia o proj�til na rede
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, position, rotation);

        // Aplica movimento para frente na bala
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = firePoint.up * bulletSpeed;  // Move a bala na dire��o do firePoint
        }
    }
}