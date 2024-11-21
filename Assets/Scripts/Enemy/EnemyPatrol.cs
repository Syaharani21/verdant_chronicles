using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge; 
    [SerializeField] private Transform rightEdge; 

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    private Vector3 initScale; 
    private bool movingLeft; 

    private void Awake()
    {
        // Simpan skala awal enemy
        initScale = enemy.localScale;
    }

    private void Update()
    {
        
        if (movingLeft)
        {
            // Jika belum mencapai ujung kiri, terus bergerak
            if (enemy.position.x > leftEdge.position.x)
                MoveInDirection(-1);
            else
                movingLeft = false; 
        }
        else
        {
            // Jika belum mencapai ujung kanan, terus bergerak
            if (enemy.position.x < rightEdge.position.x)
                MoveInDirection(1);
            else
                movingLeft = true; 
        }
    }

    private void MoveInDirection(int direction)
    {
        // Ubah orientasi enemy berdasarkan arah
        enemy.localScale = new Vector3(
            Mathf.Abs(initScale.x) * (direction == -1 ? 1 : -1), // Kiri: skala positif, Kanan: skala negati
            initScale.y,
            initScale.z
        );

        // Pindahkan enemy ke arah yang ditentukan
        enemy.position = new Vector3(
            enemy.position.x + Time.deltaTime * direction * speed,
            enemy.position.y, 
            enemy.position.z  
        );
    }
}