using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class CharacterJump : MonoBehaviour
{
    public InputActionReference jumpActionReference;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool canJump = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpActionReference.action.started += ctx => Jump();
        jumpActionReference.action.Enable();
    }

    void Jump()
    {
        if (canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        canJump = true;
    }
}
