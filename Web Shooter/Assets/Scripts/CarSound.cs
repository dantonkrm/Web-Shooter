using UnityEngine;

public class CarSound : MonoBehaviour
{
    public AudioSource audioSource;
    public float maxVolume = 1.0f;
    public float fadeSpeed = 1.0f;

    private bool isMoving = false;
    private float targetVolume = 0.0f;

    void Update()
    {
        // Verifica se o carro está em movimento (você pode ajustar essa lógica de acordo com a sua necessidade)
        if (GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            if (!isMoving)
            {
                // Inicia o fade in
                isMoving = true;
                targetVolume = maxVolume;
            }
        }
        else
        {
            if (isMoving)
            {
                // Inicia o fade out
                isMoving = false;
                targetVolume = 0.0f;
            }
        }

        // Aplica o fade in ou fade out
        if (Mathf.Abs(audioSource.volume - targetVolume) > 0.01f)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
        }
    }
}
