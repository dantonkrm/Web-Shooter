using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class CapsuleColliderDriver : MonoBehaviour
{
    public XROrigin m_XROrigin;
    public float m_MinHeight;
    public float m_MaxHeight;

    public CapsuleCollider m_collider;
    void Update()
    {
        var height = Mathf.Clamp(m_XROrigin.CameraInOriginSpaceHeight, m_MinHeight, m_MaxHeight);

        Vector3 center = m_XROrigin.CameraInOriginSpacePos;
        center.y = height / 2f + m_collider.radius;

        m_collider.height = height;
        m_collider.center = center;
    }
}
