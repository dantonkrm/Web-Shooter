using UnityEngine;
using UnityEngine.XR;

public class WindParticles : MonoBehaviour
{
    public float speedThreshold = 1.0f; // Limite de velocidade para ativar as part�culas de vento

    private Vector3 previousPosition;
    private ParticleSystem particleSystem;

    private void Start()
    {
        // Obt�m o componente ParticleSystem do objeto "ParticleOrigin"
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // Obt�m a posi��o local atual da c�mera
        Vector3 currentPosition = InputTracking.GetLocalPosition(XRNode.CenterEye);

        // Calcula a velocidade da c�mera
        float cameraSpeed = Vector3.Distance(currentPosition, previousPosition) / Time.deltaTime;

        // Ativa ou desativa o sistema de part�culas com base na velocidade da c�mera
        if (cameraSpeed >= speedThreshold)
        {
            if (!particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
        }
        else
        {
            if (particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }
        }

        // Armazena a posi��o atual como a posi��o anterior para a pr�xima atualiza��o
        previousPosition = currentPosition;
    }
}
