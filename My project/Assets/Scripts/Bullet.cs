using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun, IWeapon
{
    public GameObject bulletPrefab;   // Prefab do proj�til
    public Transform firePoint;       // Ponto de onde o proj�til ser� disparado
    public float reloadTime = 1f;     // Tempo de recarga entre disparos
    private float nextFireTime = 0f;  // Controla o tempo at� o pr�ximo disparo
    public float bulletSpeed = 10f;   // Velocidade da bala
    public float damageAmount = 20f;  // Dano causado pela bala

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Conecta ao Photon
    }

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
        // Verifica se o objeto colidido � um tanque
        if (collision.gameObject.CompareTag("Tank"))
        {
            // Aplica dano ao tanque atingido via RPC
            if (photonView.IsMine)
            {
                TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
                if (tankHealth != null)
                {
                    // Aplica dano ao tanque via RPC
                    tankHealth.TakeDamage(damageAmount);
                }
            }
        }

        // Destroi a bala na rede ap�s a colis�o
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            // Chama o m�todo RPC para disparar a bala, apenas para os outros jogadores
            photonView.RPC("RPCFireBullet", RpcTarget.Others, firePoint.position, firePoint.rotation);
            // Instancia o proj�til localmente sem usar o RPC para evitar instanciar duas vezes
            FireBulletLocally(firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + reloadTime;
        }
    }

    [PunRPC]
    void RPCFireBullet(Vector3 position, Quaternion rotation)
    {
        // Instancia o proj�til na rede para outros jogadores
        FireBulletLocally(position, rotation);
    }

    void FireBulletLocally(Vector3 position, Quaternion rotation)
    {
        // Instancia o proj�til localmente
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Aplica movimento � bala
        if (rb != null)
        {
            rb.velocity = firePoint.up * bulletSpeed;
        }
    }
}
