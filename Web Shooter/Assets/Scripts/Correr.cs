using UnityEngine;
using UnityEngine.InputSystem;

public class Correr : MonoBehaviour
{
    public float velocidadeCorrida = 5f; // Velocidade de corrida do personagem
    public float aceleracao = 10f; // Acelera��o ao iniciar a corrida
    public float desaceleracao = 10f; // Desacelera��o ao parar a corrida
    public InputActionReference thumbstickClickedAction; // Refer�ncia � a��o "thumbstickClicked"

    private bool correndo = false; // Verificar se o personagem est� correndo ou n�o
    private Transform cameraTransform; // Transform da c�mera ou objeto que indica a dire��o do movimento
    private bool noSolo = false; // Verificar se o personagem est� no solo
    private Vector3 velocidade; // Velocidade atual do personagem

    [SerializeField]
    private LayerMask groundLayer; // Layer do solo

    private void Start()
    {
        // Certifique-se de atribuir corretamente a InputActionReference ao script atrav�s do Inspector
        if (thumbstickClickedAction == null)
        {
            Debug.LogError("InputActionReference n�o atribu�do!");
            enabled = false;
            return;
        }

        // Obt�m o Transform da c�mera principal
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
        }
        else
        {
            Debug.LogError("C�mera principal n�o encontrada!");
            enabled = false;
            return;
        }

        // Registra os callbacks para o pressionamento e libera��o do bot�o
        thumbstickClickedAction.action.started += OnButtonPress;
        thumbstickClickedAction.action.canceled += OnButtonRelease;
    }

    private void OnDestroy()
    {
        // Remove os callbacks quando o objeto for destru�do para evitar vazamentos de mem�ria
        thumbstickClickedAction.action.started -= OnButtonPress;
        thumbstickClickedAction.action.canceled -= OnButtonRelease;
    }

    private void Update()
    {
        // Atualiza a velocidade gradualmente ao iniciar e parar a corrida
        float targetSpeed = correndo ? velocidadeCorrida : 0f;
        velocidade.x = Mathf.MoveTowards(velocidade.x, targetSpeed, aceleracao * Time.deltaTime);

        // Verifica se o personagem est� correndo e no solo, e atualiza a posi��o
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
