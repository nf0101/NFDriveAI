using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CapsuleCastingFixed : MonoBehaviour
{
    public float capsuleWidth = 0.2f;  // Larghezza della capsula
    public float capsuleHeight = 0.25f;  // Altezza della capsula
    public float maxRaycastDistance = 0.75f, latDistance = 0.25f;  // Lunghezza massima del raycast
    public float x = 0.93f;
    public float y = 0.7f;
    bool collided = false;
    public float rightRayDistance, leftRayDistance, rightLatDistance, leftLatDistance;
    public TMP_Text rightText, leftText, prevRight_Right, prevRight_Left, prevLeft_Right, prevLeft_Left, rightLatText, leftLatText, prevRightLat_Right, prevRightLat_Left, prevLeftLat_Right, prevLeftLat_Left;
    public float prevRight_R, prevRight_L, prevLeft_R, prevLeft_L, prevLatLeft_R, prevLatLeft_L, prevLatRight_R, prevLatRight_L;
    float rotation = 4f;

    private void Start()
    {

    }
    void FixedUpdate()
    {
        //print(capsuleHeight + x);
        // Calcola le posizioni degli estremi della capsula in base alla rotazione
        Vector2 leftEnd1 = transform.TransformPoint(new Vector2(capsuleWidth / y, x));
        Vector2 leftEnd2 = transform.TransformPoint(new Vector2(-capsuleWidth / y, x));
        Vector2 rightEnd = transform.TransformPoint(new Vector2(capsuleWidth / y, x));
        Vector2 leftDown = transform.TransformPoint(new Vector2(-capsuleWidth / y, x));

        Quaternion rightRotation = Quaternion.Euler(0, 0, -rotation);
        Quaternion leftRotation = Quaternion.Euler(0, 0, rotation);

        //Quaternion rightLatRotation = Quaternion.Euler(0, 0, rotation);
        //Quaternion leftLatRotation = Quaternion.Euler(rotation, 0, 0);

        Vector2 rightPrevisionR = (Vector2)(rightRotation * (leftEnd1 - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 rightPrevisionL = (Vector2)(leftRotation * (leftEnd1 - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 leftPrevisionR = (Vector2)(rightRotation * (leftEnd2 - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 leftPrevisionL = (Vector2)(leftRotation * (leftEnd2 - (Vector2)transform.position)) + (Vector2)transform.position;

        Vector2 rightLatPrevisionR = (Vector2)(rightRotation * (rightEnd - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 rightLatPrevisionL = (Vector2)(leftRotation * (rightEnd - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 leftLatPrevisionL = (Vector2)(leftRotation * (leftDown - (Vector2)transform.position)) + (Vector2)transform.position;
        Vector2 leftLatPrevisionR = (Vector2)(rightRotation * (leftDown - (Vector2)transform.position)) + (Vector2)transform.position;



        // Raggi frontali (sull'asse x)
        DrawRay(leftEnd1, transform.TransformDirection(Vector2.up), true, 1, maxRaycastDistance); //il bool indica il lato (destra, sinistra); l'int indica [0, previsione sx], [1, raggio reale], [2, previsione dx]
        DrawRay(leftEnd2, transform.TransformDirection(Vector2.up), false, 1, maxRaycastDistance);
        DrawRay(rightPrevisionR, rightRotation * transform.TransformDirection(Vector2.up), true, 2, maxRaycastDistance);
        DrawRay(rightPrevisionL, leftRotation * transform.TransformDirection(Vector2.up), true, 0, maxRaycastDistance);
        DrawRay(leftPrevisionR, rightRotation * transform.TransformDirection(Vector2.up), false, 2, maxRaycastDistance);
        DrawRay(leftPrevisionL, leftRotation * transform.TransformDirection(Vector2.up), false, 0, maxRaycastDistance); 
        
        DrawRay(rightEnd, Quaternion.Euler(0, 0, -15 * rotation / 1.4f) * transform.TransformDirection(Vector2.up), true, 4, maxRaycastDistance);
        DrawRay(leftDown, Quaternion.Euler(0, 0, 15 * rotation / 1.4f) * transform.TransformDirection(Vector2.up), false, 4, maxRaycastDistance);

        DrawRay(rightLatPrevisionR, Quaternion.Euler(0, 0, -15 * rotation / 1.4f) * rightRotation * transform.TransformDirection(Vector2.up), true, 5, maxRaycastDistance);
        DrawRay(rightLatPrevisionL, Quaternion.Euler(0, 0, -15 * rotation / 1.4f) * leftRotation * transform.TransformDirection(Vector2.up), true, 3, maxRaycastDistance);

        DrawRay(leftLatPrevisionL, Quaternion.Euler(0, 0, 15 * rotation / 1.4f) * leftRotation * transform.TransformDirection(Vector2.up), false, 3, maxRaycastDistance);
        DrawRay(leftLatPrevisionR, Quaternion.Euler(0, 0, 15 * rotation / 1.4f) * rightRotation * transform.TransformDirection(Vector2.up), false, 5, maxRaycastDistance);

        rightText.text = $"Dist dx: {rightRayDistance}";
        leftText.text = $"Dist sx: {leftRayDistance}";

        rightLatText.text = $"Dist lat dx: {rightLatDistance}";
        leftLatText.text = $"Dist lat sx: {leftLatDistance}";

        prevRight_Right.text = $"Prev dxdx: {prevRight_R}";
        prevRight_Left.text = $"Prev dxsx: {prevRight_L}";
        prevLeft_Right.text = $"Prev sxdx: {prevLeft_R}";
        prevLeft_Left.text = $"Prev sxdx: {prevLeft_L}";

        prevRightLat_Right.text = $"Prev lat dxdx: {prevLatRight_R}";
        prevRightLat_Left.text = $"Prev lat dxsx: {prevLatRight_L}";
        prevLeftLat_Right.text = $"Prev lat sxdx: {prevLatLeft_R}";
        prevLeftLat_Left.text = $"Prev lat sxsx: {prevLatLeft_L}";

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

    void DrawRay(Vector2 start, Vector2 direction, bool side, int rayPosition, float rayLength)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, direction, rayLength);
        
        if (hit.collider != null)
        {
            Color color;
            if (rayPosition == 1 || rayPosition == 4)
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

                if (rayPosition == 3)
                {
                    prevLatRight_L = hit.distance;
                }

                if (rayPosition == 4)
                {
                    rightLatDistance = hit.distance; 
                }

                if (rayPosition == 5)
                {
                    prevLatRight_R = hit.distance;
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

                if (rayPosition == 3)
                {
                    prevLatLeft_L = hit.distance;
                }

                if (rayPosition == 4)
                {
                    leftLatDistance = hit.distance;
                }
                    
                if (rayPosition == 5)
                {
                    prevLatLeft_R = hit.distance;
                }
            }
        }

        else
        {
            Debug.DrawRay(start, direction * rayLength, Color.green);
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

            if (rayPosition == 4)
            {
                if (side)
                {
                    rightLatDistance = float.NaN;
                }
                else
                {
                    leftLatDistance = float.NaN;
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

            if (rayPosition == 3)
            {
                if (side)
                {
                    prevLatRight_L = float.NaN;
                }
                else
                {
                    prevLatLeft_L = float.NaN;
                }
            }

            if (rayPosition == 5)
            {
                if (side)
                {
                    prevLatRight_R = float.NaN;
                }
                else
                {
                    prevLatLeft_R = float.NaN;
                }
            }
        }
        
    }
}