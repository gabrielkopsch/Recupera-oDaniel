using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float bulletSpeed = 30f; // Velocidade da bala
    public float damageAmount = 20f; // Dano causado pela bala
    private PhotonView shooterPhotonView; // PhotonView do jogador que disparou a bala

    void Start()
    {
        // Configura o movimento da bala imediatamente após a instância
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.up * bulletSpeed; // Aplica a velocidade na direção correta
        }
        else
        {
            Debug.LogError("Rigidbody2D não encontrado na bala!"); // Log de erro se não houver Rigidbody2D
        }
    }

    public void Initialize(PhotonView shooter)
    {
        shooterPhotonView = shooter; // Configura o PhotonView do jogador que disparou
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colidiu com: " + collision.gameObject.name); // Verifica qual objeto a bala está colidindo

        // Ignora colisão com o jogador que disparou a bala
        if (collision.gameObject.GetComponent<PhotonView>() == shooterPhotonView)
        {
            Debug.Log("Ignorando colisão com o jogador que disparou.");
            return; // Ignora a colisão
        }

        // Verifique se o jogador inimigo foi atingido
        TankController tankController = collision.GetComponent<TankController>();
        if (tankController != null)
        {
            Debug.Log("Bala colidiu com um TankController!");
            tankController.ApplyDamage(damageAmount); // Aplica dano ao jogador
            PhotonNetwork.Destroy(gameObject); // Destrói a bala após a colisão
        }
        else
        {
            Debug.Log("Colisão não com um TankController!"); // Log de depuração
        }
    }
}
