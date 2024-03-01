using UnityEngine;
using UnityEngine.XR;

public class WindParticles : MonoBehaviour
{
    public float speedThreshold = 1.0f; // Limite de velocidade para ativar as partículas de vento

    private Vector3 previousPosition;
    private ParticleSystem particleSystem;

    private void Start()
    {
        // Obtém o componente ParticleSystem do objeto "ParticleOrigin"
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // Obtém a posição local atual da câmera
        Vector3 currentPosition = InputTracking.GetLocalPosition(XRNode.CenterEye);

        // Calcula a velocidade da câmera
        float cameraSpeed = Vector3.Distance(currentPosition, previousPosition) / Time.deltaTime;

        // Ativa ou desativa o sistema de partículas com base na velocidade da câmera
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

        // Armazena a posição atual como a posição anterior para a próxima atualização
        previousPosition = currentPosition;
    }
}
