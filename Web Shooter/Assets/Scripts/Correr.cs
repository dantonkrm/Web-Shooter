using UnityEngine;
using UnityEngine.InputSystem;

public class Correr : MonoBehaviour
{
    public float velocidadeCorrida = 5f; // Velocidade de corrida do personagem
    public float aceleracao = 10f; // Aceleração ao iniciar a corrida
    public float desaceleracao = 10f; // Desaceleração ao parar a corrida
    public InputActionReference thumbstickClickedAction; // Referência à ação "thumbstickClicked"

    private bool correndo = false; // Verificar se o personagem está correndo ou não
    private Transform cameraTransform; // Transform da câmera ou objeto que indica a direção do movimento
    private bool noSolo = false; // Verificar se o personagem está no solo
    private Vector3 velocidade; // Velocidade atual do personagem

    [SerializeField]
    private LayerMask groundLayer; // Layer do solo

    private void Start()
    {
        // Certifique-se de atribuir corretamente a InputActionReference ao script através do Inspector
        if (thumbstickClickedAction == null)
        {
            Debug.LogError("InputActionReference não atribuído!");
            enabled = false;
            return;
        }

        // Obtém o Transform da câmera principal
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
        }
        else
        {
            Debug.LogError("Câmera principal não encontrada!");
            enabled = false;
            return;
        }

        // Registra os callbacks para o pressionamento e liberação do botão
        thumbstickClickedAction.action.started += OnButtonPress;
        thumbstickClickedAction.action.canceled += OnButtonRelease;
    }

    private void OnDestroy()
    {
        // Remove os callbacks quando o objeto for destruído para evitar vazamentos de memória
        thumbstickClickedAction.action.started -= OnButtonPress;
        thumbstickClickedAction.action.canceled -= OnButtonRelease;
    }

    private void Update()
    {
        // Atualiza a velocidade gradualmente ao iniciar e parar a corrida
        float targetSpeed = correndo ? velocidadeCorrida : 0f;
        velocidade.x = Mathf.MoveTowards(velocidade.x, targetSpeed, aceleracao * Time.deltaTime);

        // Verifica se o personagem está correndo e no solo, e atualiza a posição
        if (correndo && noSolo)
        {
            Vector3 direcao = cameraTransform.forward * velocidade.x * Time.deltaTime;
            transform.position += direcao;
        }
    }

    private void OnButtonPress(InputAction.CallbackContext context)
    {
        correndo = true;
    }

    private void OnButtonRelease(InputAction.CallbackContext context)
    {
        correndo = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            noSolo = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            noSolo = false;
        }
    }
}
