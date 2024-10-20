using Photon.Pun;
using UnityEngine;

public class TankController : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 150f;
    public float maxHealth = 100;
    private float currentHealth;
    public Vector2 minBounds;   // Limite mínimo para X e Y
    public Vector2 maxBounds;   // Limite máximo para X e Y

    public GameObject bulletPrefab; // Prefab da bala
    public Transform firePoint;     // Ponto de disparo

    void ClampPosition()
    {
        // Limita a posição do jogador dentro dos bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        transform.position = clampedPosition;
    }

    void Awake()
    {
        Camera cam = Camera.main;
        Vector2 screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));

        minBounds = new Vector2(-screenBounds.x, -screenBounds.y);
        maxBounds = new Vector2(screenBounds.x, screenBounds.y);
    }

    void Start()
    {
        currentHealth = maxHealth;

        // Apenas o jogador local controla o tanque
        if (!photonView.IsMine)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = true; // Torna o Rigidbody2D kinematic
            }
        }
    }

    void Update()
    {
        ClampPosition();
        if (photonView.IsMine)
        {
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire();
            }
        }
    }

    void HandleMovement()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;

        transform.Translate(Vector3.up * move);
        transform.Rotate(Vector3.forward, -rotate);
    }

    public void Fire()
    {
        if (bulletPrefab && firePoint)
        {
            // Instancia a bala na rede usando PhotonNetwork.Instantiate
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation, 0);

            // Configura o PhotonView do jogador que disparou a bala na bala instanciada
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Initialize(photonView); // Passa o PhotonView do jogador que disparou
            }
        }
    }

    [PunRPC]
    public void ApplyDamage(float damage)
    {
        maxHealth -= damage;
        Debug.Log("Dano recebido: " + damage + ", Saúde restante: " + maxHealth);
        if (maxHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.instance.UpdateScore(PhotonNetwork.NickName);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            Invoke("Respawn", 3f); // Renasce após 3 segundos
        }
    }

    void Respawn()
    {
        Vector3 spawnPosition = GameManager.instance.GetRandomSpawnPosition();
        PhotonNetwork.Instantiate(gameObject.name, spawnPosition, Quaternion.identity);
    }
}
