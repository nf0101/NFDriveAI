using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleCasting : MonoBehaviour
{
    public float capsuleWidth = 0.2f;  // Larghezza della capsula
    public float capsuleHeight = 0.25f;  // Altezza della capsula
    public float maxRaycastDistance = 1.5f;  // Lunghezza massima del raycast
    public float x = 0.93f;
    public float y = 0.7f;
    bool collided = false;
    static float rightRayDistance;
    static float leftRayDistance;

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

        // Raggi frontali (sull'asse x)
        DrawRay(leftEnd1, transform.TransformDirection(Vector2.up), true);
        DrawRay(leftEnd2, transform.TransformDirection(Vector2.up), false);

        // Raggi laterali (sull'asse y)
        //DrawRay(topEnd, transform.TransformDirection(Vector2.left));
        //DrawRay(bottomEnd, transform.TransformDirection(Vector2.right));
        
    }

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

    void DrawRay(Vector2 start, Vector2 direction, bool isRightRay)
    {
        RaycastHit2D hit = Physics2D.Raycast(start, direction, maxRaycastDistance);

        if (hit.collider != null)
        {
            Debug.DrawLine(start, hit.point, Color.red);
            if (collided)
            {
                print($"{hit.distance}");
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                print(hit.distance);
            }
            //print(hit.collider.name);
        }
        else
        {
            Debug.DrawRay(start, direction * maxRaycastDistance, Color.green);
        }
        
    }
}