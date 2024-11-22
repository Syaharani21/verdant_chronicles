using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
   [SerializeField]private float attackCooldown;
   [SerializeField]private Transform firePoint;
   [SerializeField] private GameObject[] fireballs;
   private Animator anim;
   private PlayerMovement playerMovement;
   private float cooldownTimer;

   private void Awake(){
    anim = GetComponent<Animator>();
    playerMovement = GetComponent<PlayerMovement>();
   }
   
   private void Update(){
    if(Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
    Attack();

    cooldownTimer += Time.deltaTime;
   }

   private void Attack(){
      anim.SetTrigger("attack");
    cooldownTimer = 0;

    fireballs[0].transform.position = firePoint.position;
    fireballs[0].GetComponent<projektive>().SetDirection(Mathf.Sign(transform.localScale.x));
   }
}
