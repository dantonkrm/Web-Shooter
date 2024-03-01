using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WindSound : MonoBehaviour
{
    public float minSpeed = 2f; // Velocidade mínima para começar a reproduzir o som
    public float maxVolume = 0.5f; // Volume máximo do som de vento
    public float maxSpeed = 10f; // Velocidade máxima para alcançar o volume máximo
    public float fadeInDuration = 1f; // Duração do fade in em segundos
    public float fadeOutDuration = 1f; // Duração do fade out em segundos

    private Rigidbody playerRigidbody;
    private AudioSource audioSource;
    private float targetVolume;
    private float currentVolume;
    private bool isFadingIn;
    private bool isFadingOut;

    private void Start()
    {
        playerRigidbody = GetComponentInParent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        targetVolume = 0f;
        currentVolume = 0f;
        isFadingIn = false;
        isFadingOut = false;
    }

    private void Update()
    {
        float playerSpeed = playerRigidbody.velocity.magnitude;

        // Verifica se deve iniciar o fade in
        if (playerSpeed >= minSpeed && !isFadingIn && !audioSource.isPlaying)
        {
            isFadingIn = true;
            StartCoroutine(FadeIn());
        }

        // Verifica se deve iniciar o fade out
        if (playerSpeed < minSpeed && !isFadingOut && audioSource.isPlaying)
        {
            isFadingOut = true;
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float timer = 0f;
        float initialVolume = audioSource.volume;
        audioSource.Play();

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            currentVolume = Mathf.Lerp(initialVolume, maxVolume, timer / fadeInDuration);
            audioSource.volume = currentVolume;
            yield return null;
        }

        isFadingIn = false;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float timer = 0f;
        float initialVolume = audioSource.volume;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            currentVolume = Mathf.Lerp(initialVolume, 0f, timer / fadeOutDuration);
            audioSource.volume = currentVolume;
            yield return null;
        }

        audioSource.Stop();
        isFadingOut = false;
    }
}
