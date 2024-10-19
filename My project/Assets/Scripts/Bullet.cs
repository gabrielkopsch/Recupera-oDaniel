using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun, IWeapon
{
    public GameObject bulletPrefab;       // Prefab do proj�til
    public Transform firePoint;           // Ponto de onde o proj�til ser� disparado
    public float reloadTime = 1f;         // Tempo de recarga entre disparos
    private float nextFireTime = 0f;      // Controla o tempo at� o pr�ximo disparo
    public float bulletSpeed = 30f;       // Aumente a velocidade da bala para um valor maior
    public float damageAmount = 20f;      // Dano causado pela bala
    private PhotonView shooterPhotonView; // PhotonView do jogador que disparou a bala
    public float bulletLifeTime = 40f;
    public float ReloadTime
    {
        get { return reloadTime; }
    }

    void Update()
    {
        // Apenas o dono do objeto pode disparar
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    [PunRPC]
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignora colis�o com o jogador que disparou a bala
        if (collision.gameObject.GetComponent<PhotonView>() == shooterPhotonView)
        {
            return;  // Evita que a bala colida com o jogador que a disparou
        }

        PhotonView pv = PhotonView.Get(this);

        TankController tankController = collision.gameObject.GetComponent<TankController>();
        
        if(tankController != null && !tankController.photonView.IsMine)
        {
            Debug.Log("Colidiu com um tanque ou outro jogador!");

            // Desativa o proj�til em todos os clientes
            pv.RPC("DisableGameObject", RpcTarget.All);
        }
    }

    [PunRPC]
    void DisableGameObject()
    {
        gameObject.SetActive(false);  // Desativa o objeto (a bala) ao colidir
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            // Instancia a bala na rede usando PhotonNetwork.Instantiate
            GameObject bullet = PhotonNetwork.Instantiate("bulletPrefab", firePoint.position, firePoint.rotation);

            // Referencia o PhotonView do jogador que disparou a bala
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.shooterPhotonView = this.photonView;

            // Aplica movimento � bala imediatamente ap�s a inst�ncia
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Aplique a velocidade na dire��o do firePoint
                rb.velocity = firePoint.up * bulletSpeed; // Use up para a dire��o correta
            }

            // Define o tempo para o pr�ximo disparo
            nextFireTime = Time.time + reloadTime;
        }
    }
}
