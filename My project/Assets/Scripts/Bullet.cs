using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float bulletSpeed = 30f; // Velocidade da bala
    public float damageAmount = 20f; // Dano causado pela bala
    private PhotonView shooterPhotonView; // PhotonView do jogador que disparou a bala

    void Start()
    {
        // Configura o movimento da bala imediatamente ap�s a inst�ncia
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.up * bulletSpeed; // Aplica a velocidade na dire��o correta
        }
        else
        {
            Debug.LogError("Rigidbody2D n�o encontrado na bala!"); // Log de erro se n�o houver Rigidbody2D
        }
    }

    public void Initialize(PhotonView shooter)
    {
        shooterPhotonView = shooter; // Configura o PhotonView do jogador que disparou
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colidiu com: " + collision.gameObject.name); // Verifica qual objeto a bala est� colidindo

        // Ignora colis�o com o jogador que disparou a bala
        if (collision.gameObject.GetComponent<PhotonView>() == shooterPhotonView)
        {
            Debug.Log("Ignorando colis�o com o jogador que disparou.");
            return; // Ignora a colis�o
        }

        // Verifique se o jogador inimigo foi atingido
        TankController tankController = collision.GetComponent<TankController>();
        if (tankController != null)
        {
            Debug.Log("Bala colidiu com um TankController!");
            tankController.ApplyDamage(damageAmount); // Aplica dano ao jogador
            PhotonNetwork.Destroy(gameObject); // Destr�i a bala ap�s a colis�o
        }
        else
        {
            Debug.Log("Colis�o n�o com um TankController!"); // Log de depura��o
        }
    }
}
