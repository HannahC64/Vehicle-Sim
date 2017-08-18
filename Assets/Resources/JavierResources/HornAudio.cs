using UnityEngine;

public class HornAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip carHorn3;
    void Start ()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("LeftRed"))
        {
            
            audioSource.PlayOneShot(carHorn3,0.8f);
        }
    }
}
