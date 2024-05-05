using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CapsuleCasting : MonoBehaviour
{
    public float capsuleWidth = 0.2f;  // Larghezza della capsula
    public float capsuleHeight = 0.25f;  // Altezza della capsula
    public float maxRaycastDistance = 1.5f;  // Lunghezza massima del raycast
    public float x = 0.93f;
    public float y = 0.7f;
    bool collided = false;
    public float rightRayDistance;
    public float leftRayDistance;
    public TMP_Text rightText, leftText, prevRight_Right, prevRight_Left, prevLeft_Right, prevLeft_Left;
    public float prevRight_R, prevRight_L, prevLeft_R, prevLeft_L;
    float rotation = 2f;

    private void Start()
    {

    }
    void Update()
    {
        //print(capsuleHeight + x);
        // Calcola le posizioni degli estremi della capsula in base alla rotazione
        Vector2 leftEnd1 = transform.TransformPoint(new Vector2(capsuleWidth / y, x));
        Vector2 leftEnd2 = transform.TransformPoint(new Vector2(-capsuleWidth / y, x));
        //Vector2 topEnd = transform.TransformPoint(new Vector2(0f, capsuleHeight / 2f));
        //Vector2 bottomEnd = transform.TransformPoint(new Vector2(0f, capsuleHeight / 2f));

        Quaternion rightRotation = Quaternion.Euler(0, 0, -rotation);
        Quaternion leftRotation = Quaternion.Euler(0, 0, rotation);

        Vector2 rightPrevisionR = (Vector2)(rightRotation * (leftEnd1 - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 rightPrevisionL = (Vector2)(leftRotation * (leftEnd1 - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 leftPrevisionR = (Vector2)(rightRotation * (leftEnd2 - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 leftPrevisionL = (Vector2)(leftRotation * (leftEnd2 - (Vector2)transform.position)) + (Vector2)transform.position;



        // Raggi frontali (sull'asse x)
        DrawRay(leftEnd1, transform.TransformDirection(Vector2.up), true, 1); //il primo bool indica raycast reale (true) o di previsione (false), il secondo indica destra (true) o sinistra (false)
        DrawRay(leftEnd2, transform.TransformDirection(Vector2.up), false, 1);
        DrawRay(rightPrevisionR, rightRotation * transform.TransformDirection(Vector2.up), true, 2);
        DrawRay(rightPrevisionL, leftRotation * transform.TransformDirection(Vector2.up), true, 0);
        DrawRay(leftPrevisionR, rightRotation * transform.TransformDirection(Vector2.up), false, 2);
        DrawRay(leftPrevisionL, leftRotation * transform.TransformDirection(Vector2.up), false, 0);

        // Raggi laterali (sull'asse y)
        //DrawRay(topEnd, transform.TransformDirection(Vector2.left));
        //DrawRay(bottomEnd, transform.TransformDirection(Vector2.right));

        rightText.text = $"Dist dx: {rightRayDistance}";
        leftText.text = $"Dist sx: {leftRayDistance}";
        prevRight_Right.text = $"Prev dxdx: {prevRight_R}";
        prevRight_Left.text = $"Prev dxsx: {prevRight_L}";
        prevLeft_Right.text = $"Prev sxdx: {prevLeft_R}";
        prevLeft_Left.text = $"Prev sxdx: {prevLeft_L}";

    }
    //coordinate punto di inizio - distanza attuale - distanza successiva
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Bound"))
        {
            
            collided = true;
            //print($"D: {rightRayDistance}, {leftRayDistance}");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Bound"))
        {
            collided = false;
        }
    }

    void DrawRay(Vector2 start, Vector2 direction, bool side, int rayPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, direction, maxRaycastDistance);
        
        if (hit.collider != null)
        {
            Color color;
            if (rayPosition == 1)
            {
                color = Color.red;
            }
            else
            {
                color = Color.blue;
            }
            Debug.DrawLine(start, hit.point, color);
            if (rayPosition == 1)
            {
                if (side)
                {
                    rightRayDistance = hit.distance;
                   
                }
                else
                {
                    leftRayDistance = hit.distance;
                }

                if (Input.GetKeyDown(KeyCode.B))
                {
                    print(hit.distance);
                }
            }

            if (side)
            {
                if (rayPosition == 0)
                {
                    prevRight_L = hit.distance;
                }

                if (rayPosition == 2)
                {
                    prevRight_R = hit.distance;
                }
            }
            else
            {
                if (rayPosition == 0)
                {
                    prevLeft_L = hit.distance;
                }

                if (rayPosition == 2)
                {
                    prevLeft_R = hit.distance;
                }
            }
        }

        else
        {
            Debug.DrawRay(start, direction * maxRaycastDistance, Color.green);
            if (rayPosition == 1)
            {
                if (side)
                {
                    rightRayDistance = float.NaN;
                }
                else
                {
                    leftRayDistance = float.NaN;
                }
                
            }

            if (side)
            {
                if (rayPosition == 0)
                {
                    prevRight_L = float.NaN;
                }

                if (rayPosition == 2)
                {
                    prevRight_R = float.NaN;
                }
            }
            else
            {
                if (rayPosition == 0)
                {
                    prevLeft_L = float.NaN;
                }

                if (rayPosition == 2)
                {
                    prevLeft_R = float.NaN;
                }
            }

        }
        
    }
}