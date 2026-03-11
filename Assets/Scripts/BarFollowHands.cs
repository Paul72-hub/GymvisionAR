using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarFollowHands : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;

    [Header("Offsets")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    void LateUpdate()
    {
        if (leftHand == null || rightHand == null) return;

        // Centre entre les deux mains
        Vector3 center = (leftHand.position + rightHand.position) * 0.5f;
        transform.position = center + positionOffset;

        // Direction entre les deux mains
        Vector3 handDirection = rightHand.position - leftHand.position;
        if (handDirection.sqrMagnitude < 0.0001f) return;

        // Oriente la barre selon la ligne entre les mains
        Quaternion baseRotation = Quaternion.LookRotation(handDirection.normalized, Vector3.up);

        // Corrige selon l'orientation réelle de ton mesh
        transform.rotation = baseRotation * Quaternion.Euler(rotationOffset);
    }
}