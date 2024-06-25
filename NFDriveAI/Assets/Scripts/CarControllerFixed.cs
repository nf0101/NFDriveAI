using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CarControllerFixed : MonoBehaviour  
{
    public float carSpeed = 0.75f;
    bool canDrive = true;
    Rigidbody2D m_Rigidbody;

    //public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        
        //gameObject.transform.forward = new Vector3(1, 0, 0);
        

    }
    private void FixedUpdate()
    {
        if (canDrive)
        {
            //m_Rigidbody.MovePosition(transform.position + carSpeed * Time.fixedDeltaTime * gameObject.transform.up);
            Vector2 position = m_Rigidbody.position;
            Vector2 forward = transform.up;
            //position += forward * carSpeed * Time.fixedDeltaTime;
            //m_Rigidbody.MovePosition(position);
            //m_Rigidbody.AddForce(forward * carSpeed);

            m_Rigidbody.velocity = forward;
        }   
    }

    public void TurnOff()
    {
        canDrive = false;
        m_Rigidbody.velocity = Vector2.zero;
    }

    public void SteerRight()
    {
        //gameObject.transform.Rotate(Vector3.back * 2f);
        //m_Rigidbody.MoveRotation(m_Rigidbody.rotation - 400f / 2 * Time.fixedDeltaTime);
        //m_Rigidbody.SetRotation(m_Rigidbody.rotation - 200f/2 * Time.fixedDeltaTime);
        m_Rigidbody.SetRotation(m_Rigidbody.rotation - 400f / 2 * Time.fixedDeltaTime);

    }

    public void SteerLeft()
    {
        //gameObject.transform.Rotate(Vector3.forward * 2f);
        //m_Rigidbody.MoveRotation(m_Rigidbody.rotation * Quaternion.Euler(Vector3.forward * 2f * Time.fixedDeltaTime));
        m_Rigidbody.SetRotation(m_Rigidbody.rotation + 400f/2 * Time.fixedDeltaTime);
    }

    public void Drive()
    {
        //gameObject.transform.Translate(Vector3.up * Time.deltaTime * carSpeed);
        //m_Rigidbody.MovePosition(transform.position + carSpeed * Time.deltaTime * gameObject.transform.up);

    }

    // Update is called once per frame
    void Update()
    {
        //text.text = "Speed: " + carSpeed;
        if (carSpeed < 4)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                carSpeed += 0.25f;
            }
        }

        if(carSpeed > 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                carSpeed -= 0.25f;
            }
        }


        //if (Input.GetKey(KeyCode.W))
        //{
        //    gameObject.transform.Translate(Vector3.up * Time.deltaTime * carSpeed);
        //}

        if (Input.GetKeyDown(KeyCode.A))
        {
            SteerLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SteerRight();
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    gameObject.transform.Translate(Vector3.up * Time.deltaTime * 2f);
        //}



    }
}
