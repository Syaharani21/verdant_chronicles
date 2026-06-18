using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource source;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        source = GetComponent<AudioSource>();
    }

    // Ubah dari private ke public
    public void PlaySound(AudioClip _sound)
    {
        if (_sound == null) return;
        source.PlayOneShot(_sound);
    }
}
