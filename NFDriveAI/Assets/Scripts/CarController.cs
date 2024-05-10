using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CarController : MonoBehaviour  
{
    public float carSpeed = 1;
    public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SteerRight()
    {
        gameObject.transform.Rotate(Vector3.back * 2f);
    }

    public void SteerLeft()
    {
        gameObject.transform.Rotate(Vector3.forward * 2f);
    }

    public void Drive()
    {
        gameObject.transform.Translate(Vector3.up * Time.deltaTime * carSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Speed: " + carSpeed;
        if (carSpeed < 4)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                carSpeed += 0.5f;
            }
        }

        if(carSpeed > 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                carSpeed -= 0.5f;
            }
        }
        

        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.Translate(Vector3.up * Time.deltaTime * carSpeed);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            gameObject.transform.Rotate(Vector3.forward * 2f);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            gameObject.transform.Rotate(Vector3.back * 2f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameObject.transform.Translate(Vector3.up * Time.deltaTime * 2f);
        }



    }
}
