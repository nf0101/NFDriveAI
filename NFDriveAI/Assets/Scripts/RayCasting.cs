using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleCasting : MonoBehaviour
{
    public float capsuleWidth = 0.2f;  // Larghezza della capsula
    public float capsuleHeight = 0.5f;  // Altezza della capsula

    void Update()
    {
        // Calcola le posizioni degli estremi della capsula in base alla rotazione
        //Vector2 leftEnd1 = transform.TransformPoint(new Vector2(-capsuleWidth / 2f, capsuleHeight / 2f));
        //Vector2 leftEnd2 = transform.TransformPoint(new Vector2(-capsuleWidth / 2f, -capsuleHeight / 2f));
        //Vector2 topEnd = transform.TransformPoint(new Vector2(0f, capsuleHeight / 2f));
        //Vector2 bottomEnd = transform.TransformPoint(new Vector2(0f, -capsuleHeight / 2f));

        Vector2 leftEnd1 = transform.TransformPoint(new Vector2(capsuleWidth / 1.1f, -capsuleHeight / 2f));
        Vector2 leftEnd2 = transform.TransformPoint(new Vector2(-capsuleWidth / 1.1f, -capsuleHeight / 2f));
        Vector2 topEnd = transform.TransformPoint(new Vector2(0f, capsuleHeight / 2f));
        Vector2 bottomEnd = transform.TransformPoint(new Vector2(0f, capsuleHeight / 2f));

        // Raggi frontali (sull'asse x)
        DrawRay(leftEnd1, transform.TransformDirection(Vector2.up));
        DrawRay(leftEnd2, transform.TransformDirection(Vector2.up));

        // Raggi laterali (sull'asse y)
        DrawRay(topEnd, transform.TransformDirection(Vector2.left));
        DrawRay(bottomEnd, transform.TransformDirection(Vector2.right));
    }

    void DrawRay(Vector2 start, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, direction);

        if (hit.collider != null)
        {
            Debug.DrawLine(start, hit.point, Color.red);
            //print(hit.collider.name);
        }
        else
        {
            Debug.DrawRay(start, direction * 100f, Color.green);
        }
    }
}