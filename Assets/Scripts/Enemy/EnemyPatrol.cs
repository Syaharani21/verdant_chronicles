using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header ("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft;

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;

    private void Awake()
    {
        initScale = enemy.localScale;
    }

    private void Update()
{
    // Periksa apakah musuh sudah mati
    if (enemy.GetComponent<Health>().IsDead)
    {
        return; 
    }

    if (movingLeft)
    {
        if (enemy.position.x > leftEdge.position.x)
            MoveInDirection(-1);
        else
        {
            enemy.position = new Vector3(leftEdge.position.x, enemy.position.y, enemy.position.z);
            DirectionChange();
        }
    }
    else
    {
        if (enemy.position.x < rightEdge.position.x)
            MoveInDirection(1);
        else
        {
            enemy.position = new Vector3(rightEdge.position.x, enemy.position.y, enemy.position.z);
            DirectionChange();
        }
    }
}

    private void DirectionChange()
    {
        movingLeft = !movingLeft;
    }

    private void MoveInDirection(int _direction)
    {
        //Make enemy face direction
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction,
            initScale.y, initScale.z);

        //Move in that direction
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }
}