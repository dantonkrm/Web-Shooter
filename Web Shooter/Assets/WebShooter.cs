using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WebShooter : MonoBehaviour
{
    [Header("Core Variables")]
    public Transform shooterTip;
    public Rigidbody player;
    public GameObject webEnd;
    public LineRenderer lineRenderer;
    public ActionBasedController controller;

    [Header("Web Settings")]
    public float webStrenght = 8.5f;
    public float webDamper = 7f;
    public float webMassScale = 4.5f;
    public float webZipStrenght = 5f;
    public float maxDistance;
    public LayerMask webLayers;

    public AudioSource webReleaseAudioSource;

    private SpringJoint joint;
    private FixedJoint endJoint;
    private Vector3 webPoint;
    private float distanceFromPoint;
    private bool webShot;
    private Vector3 initialControllerPosition;
    private bool isPulling = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        webEnd.transform.parent = null;
    }

    private void Update()
    {
        HandleInput();

        if (webShot && joint)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, shooterTip.position);
            lineRenderer.SetPosition(1, webEnd.transform.position);
        }

        if (isPulling)
        {
            Vector3 currentControllerPosition = controller.transform.position;
            float pullDistance = Vector3.Distance(initialControllerPosition, currentControllerPosition);

            // limite de distância para ativar o impulso.
            float pullThreshold = 0.1f; 

            if (pullDistance > pullThreshold)
            {
                ApplyWebImpulse(currentControllerPosition - initialControllerPosition);
                isPulling = false;
            }
        }
    }

    private void HandleInput()
    {
        float isPressed = controller.activateAction.action.ReadValue<float>();

        if (isPressed > 0 && !webShot)
        {
            webShot = true;
            isPulling = true;
            initialControllerPosition = controller.transform.position; // Salve a posição inicial.
            ShootWeb();

            if (webReleaseAudioSource != null)
            {
                webReleaseAudioSource.Play();
            }
        }
        else if (isPressed == 0 && webShot)
        {
            webShot = false;
            StopWeb();
        }
    }

    private void ShootWeb()
    {
        Vector3 webDirection = shooterTip.forward;
        float webDistance = maxDistance;
        Quaternion webRotation = Quaternion.LookRotation(webDirection);

        RaycastHit[] hits = player.SweepTestAll(webDirection, webDistance, QueryTriggerInteraction.Ignore);

        if (hits.Length > 0)
        {
            float closestDistance = float.MaxValue;
            RaycastHit closestHit = new RaycastHit();

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < closestDistance)
                {
                    closestDistance = hits[i].distance;
                    closestHit = hits[i];
                }
            }

            webPoint = closestHit.point;
            webEnd.transform.position = webPoint;

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = webPoint;

            if (closestHit.transform.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                if (rigidbody)
                {
                    joint.connectedAnchor = Vector3.zero;

                    webEnd.GetComponent<Rigidbody>().isKinematic = false;
                    endJoint = webEnd.AddComponent<FixedJoint>();
                    endJoint.connectedBody = rigidbody;

                    joint.connectedBody = webEnd.GetComponent<Rigidbody>();
                }
            }

            if (!rigidbody)
            {
                webEnd.GetComponent<Rigidbody>().isKinematic = true;
            }

            distanceFromPoint = Vector3.Distance(player.transform.position, webPoint) * 0.75f;
            joint.minDistance = 0;
            joint.maxDistance = distanceFromPoint;

            joint.spring = webStrenght;
            joint.damper = webDamper;
            joint.massScale = webMassScale;
        }
    }

    private void ApplyWebImpulse(Vector3 pullDirection)
    {
        // Aqui você pode aplicar um impulso com base na direção do puxão.
        float impulseStrength = 30.0f; // Ajuste conforme necessário.
        player.AddForce(pullDirection.normalized * impulseStrength, ForceMode.Impulse);
    }

    private void StopWeb()
    {
        Destroy(joint);

        if (endJoint) Destroy(endJoint);
        lineRenderer.positionCount = 0;
    }
}
