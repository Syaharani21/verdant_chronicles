using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPlayer : MonoBehaviour
{
    public float speed = 5f;                 // Kecepatan gerak otomatis
    public float moveDistance = 5f;          // Jarak gerak sebelum berbalik arah
    private Vector2 startingPosition;        // Posisi awal player
    private bool movingRight = true;         // Status arah gerak awal
    private Animator animator;               // Referensi ke komponen Animator


    void Start()
    {
        startingPosition = transform.position; // Menyimpan posisi awal player
        animator = GetComponent<Animator>();   // Mengambil komponen Animator
    }

    // Update is called once per frame
    void Update()
    {
        // Menentukan arah gerak dan menggerakkan pemain
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            animator.SetBool("isWalking", true);  // Mengatur animasi ke kanan
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            animator.SetBool("isWalking", false); // Mengatur animasi ke kiri
        }

        // Mengubah arah jika mencapai batas gerak
        if (Vector2.Distance(startingPosition, transform.position) >= moveDistance)
        {
            movingRight = !movingRight;              // Berbalik arah
            startingPosition = transform.position;    // Update posisi awal
        }
    }
}
