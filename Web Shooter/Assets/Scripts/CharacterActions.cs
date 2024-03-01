using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterActions : MonoBehaviour
{
    public float velocidadeCorrida = 5f;
    public float aceleracao = 10f;
    public float desaceleracao = 10f;
    public InputActionReference thumbstickClickedAction;
    public InputActionReference jumpActionReference;
    public float jumpForce = 5f;

    private bool correndo = false;
    private Transform cameraTransform;
    private bool noSolo = false;
    private Vector3 velocidade;
    private Rigidbody rb;
    private bool canJump = true;
    private float tempoDePressao;
    private float tempoDePressaoMaximo = 2.0f; // Tempo máximo de pressão para o pulo
    public float multiplicadorDeForca = 2f; // Ajuste conforme necessário

    [SerializeField]
    private LayerMask groundLayer;

    private void Start()
    {
        if (thumbstickClickedAction == null || jumpActionReference == null)
        {
            Debug.LogError("InputActionReference não atribuído!");
            enabled = false;
            return;
        }

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

        thumbstickClickedAction.action.started += OnButtonPress;
        thumbstickClickedAction.action.canceled += OnButtonRelease;
        jumpActionReference.action.started += ctx => Jump();

        rb = GetComponent<Rigidbody>();

        thumbstickClickedAction.action.Enable();
        jumpActionReference.action.Enable();
    }

    private void OnDestroy()
    {
        thumbstickClickedAction.action.started -= OnButtonPress;
        thumbstickClickedAction.action.canceled -= OnButtonRelease;
        jumpActionReference.action.started -= ctx => Jump();
    }

    private void Update()
    {
        float targetSpeed = correndo ? velocidadeCorrida : 0f;
        velocidade.x = Mathf.MoveTowards(velocidade.x, targetSpeed, aceleracao * Time.deltaTime);

        if (correndo && noSolo)
        {
            LimitVelocity(); // Adiciona a limitação de velocidade.
            Vector3 direcao = cameraTransform.forward * velocidade.x * Time.deltaTime;
            transform.position += direcao;
        }

        // Atualiza o tempo de pressão do botão de pular
        if (canJump && Input.GetButtonDown("Jump"))
        {
            tempoDePressao = 0f;
        }
        else if (Input.GetButton("Jump"))
        {
            tempoDePressao += Time.deltaTime;
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

    private void Jump()
    {
        if (canJump)
        {
            tempoDePressao = Mathf.Clamp(tempoDePressao, 0f, tempoDePressaoMaximo);

            // Calcula a força do pulo com base na duração do pressionamento do botão
            float forcaFinalDoPulo = jumpForce * (1 + (tempoDePressao / tempoDePressaoMaximo) * multiplicadorDeForca);

            // Aplica a força para o salto
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Zera a componente vertical da velocidade antes de aplicar o impulso
            rb.AddForce(Vector3.up * forcaFinalDoPulo, ForceMode.Impulse);

            canJump = false;
            tempoDePressao = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            noSolo = true;
            canJump = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            noSolo = false;
        }
    }

    private void LimitVelocity()
    {
        float maxAirborneSpeed = 3f; // Limite de velocidade no ar
        float maxSwingSpeed = 20f;   // Limite de velocidade enquanto se balança nas teias

        Vector3 currentVelocity = rb.velocity;

        // Limite de velocidade no eixo horizontal (X e Z).
        float horizontalSpeed = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
        if (horizontalSpeed > maxAirborneSpeed && !IsSwingingOnWeb())
        {
            float ratio = maxAirborneSpeed / horizontalSpeed;
            rb.velocity *= ratio;
        }

        // Limite de velocidade no eixo vertical (Y).
        if (currentVelocity.y > maxSwingSpeed)
        {
            rb.velocity = new Vector3(currentVelocity.x, maxSwingSpeed, currentVelocity.z);
        }
    }

    private bool IsSwingingOnWeb()
    {
        // Implemente a lógica para verificar se o personagem está se balançando nas teias.
        // Isso depende da implementação específica da mecânica de teias em seu jogo.
        // Retorne true se estiver se balançando nas teias; caso contrário, retorne false.
        return false;
    }
}
