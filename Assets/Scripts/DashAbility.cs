using UnityEngine;  // Pastikan ini ada di bagian atas

public class DashAbility : MonoBehaviour 
{
    public DashState dashState;
    public float dashTimer;
    public float maxDash = 20f;
    public Vector2 savedVelocity;

    private Rigidbody2D playerRigidbody;  // Renamed variable

    void Awake() 
    {
        playerRigidbody = GetComponent<Rigidbody2D>();  // Mengambil komponen Rigidbody2D dari Player
    }

    void Update() 
    {
        switch (dashState) 
        {
            case DashState.Ready:
                // Memeriksa apakah tombol Left Shift ditekan bersamaan dengan tombol arah atau WASD
                if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
                                                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
                                                        Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || 
                                                        Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow)))
                {
                    savedVelocity = playerRigidbody.velocity;  // Menggunakan playerRigidbody untuk mengakses velocity

                    // Tentukan arah dash berdasarkan input horizontal dan vertikal
                    float dashDirectionX = Input.GetAxisRaw("Horizontal");  // -1 untuk kiri, 1 untuk kanan
                    float dashDirectionY = Input.GetAxisRaw("Vertical");    // -1 untuk bawah, 1 untuk atas

                    // Atur kecepatan dash dengan mengalikan arah dash
                    playerRigidbody.velocity = new Vector2(dashDirectionX * 3f, dashDirectionY * 3f);
                    dashState = DashState.Dashing;
                }
                break;

            case DashState.Dashing:
                dashTimer += Time.deltaTime * 3;
                if (dashTimer >= maxDash)
                {
                    dashTimer = maxDash;
                    playerRigidbody.velocity = savedVelocity;  // Mengembalikan kecepatan awal setelah dash
                    dashState = DashState.Cooldown;
                }
                break;

            case DashState.Cooldown:
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = DashState.Ready;
                }
                break;
        }
    }
}

public enum DashState 
{
    Ready,
    Dashing,
    Cooldown
}
