using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource source;

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();

        // Jika memakai lagu yang sama di stage selanjutnya(Hapus jika menggunakan sound yang berbeda)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        // Destroy duplicate game objects
        else if (instance != null && instance != this)
            Destroy(gameObject);

    }

    // Ubah dari private ke public
    public void PlaySound(AudioClip _sound)
    {
        source.PlayOneShot(_sound);
    }
}
