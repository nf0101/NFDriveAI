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
    public TMP_Text rightText, leftText, test, prev;
    public float prevDis;

    private void Start()
    {

    }
    void Update()
    {
        //print(capsuleHeight + x);
        // Calcola le posizioni degli estremi della capsula in base alla rotazione
        Vector2 leftEnd1 = transform.TransformPoint(new Vector2(capsuleWidth / y, x));
        Vector2 leftEnd2 = transform.TransformPoint(new Vector2(-capsuleWidth / y, x));
        Vector2 topEnd = transform.TransformPoint(new Vector2(0f, capsuleHeight / 2f));
        Vector2 bottomEnd = transform.TransformPoint(new Vector2(0f, capsuleHeight / 2f));
        Vector2 nextStart = Quaternion.Euler(Vector2.down * 1.5f) * leftEnd1; //correggere punto start
        // Raggi frontali (sull'asse x)
        DrawRay(leftEnd1, transform.TransformDirection(Vector2.up), new bool[] {true, true}); //il primo bool indica raycast reale (true) o di previsione (false), il secondo indica destra (true) o sinistra (false)
        DrawRay(leftEnd2, transform.TransformDirection(Vector2.up), new bool[] {true, false});
        DrawRay(nextStart, Quaternion.Euler(0, 0, -1.5f) * transform.TransformDirection(Vector2.up), new bool[] {false, true});


        // Raggi laterali (sull'asse y)
        //DrawRay(topEnd, transform.TransformDirection(Vector2.left));
        //DrawRay(bottomEnd, transform.TransformDirection(Vector2.right));

        rightText.text = rightRayDistance.ToString();
        leftText.text = leftRayDistance.ToString();
        prev.text = prevDis.ToString();

    }
    //coordinate punto di inizio - distanza attuale - distanza successiva
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Finish"))
        {
            
            collided = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Finish"))
        {
            collided = false;
        }
    }

    void DrawRay(Vector2 start, Vector2 direction, bool[] specs)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, direction, maxRaycastDistance);
        
        if (hit.collider != null)
        {
            //if (specs[0])
            //{
            //   // test.text = Vector2.Angle(gameObject.transform.position, hit.point).ToString();
            //}
            
            Debug.DrawLine(start, hit.point, Color.red);
            if (specs[0])
            {
                if (specs[1])
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
            else
            {
                if (specs[1])
                {
                    prevDis = hit.distance;
                }
            }
            //test.text = Vector2.Distance(Quaternion.Euler(Vector2.down * 0.5f) * start, hit.point).ToString();
        }

        else
        {
            Debug.DrawRay(start, direction * maxRaycastDistance, Color.green);
            if (specs[0])
            {
                if (specs[1])
                {
                    rightRayDistance = float.NaN;
                }
                else
                {
                    leftRayDistance = float.NaN;
                }
                
            }
            else
            {
               
            }
        }
        
    }
}