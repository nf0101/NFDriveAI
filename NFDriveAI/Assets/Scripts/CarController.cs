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

        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Rotate(Vector3.forward * 100.0f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Rotate(Vector3.back * 100.0f * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameObject.transform.Translate(Vector3.up * Time.deltaTime * 2f);
        }

    }
}
