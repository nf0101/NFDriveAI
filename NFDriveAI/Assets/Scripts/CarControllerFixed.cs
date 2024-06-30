using TMPro;
using UnityEngine;
public class CarControllerFixed : MonoBehaviour  
{
    public float carSpeed = 0.75f;
    bool canDrive = true;
    Rigidbody2D m_Rigidbody;
    public TMP_Text speedText;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();

    }
    private void FixedUpdate()
    {
        if (canDrive)
        {
            Vector2 position = m_Rigidbody.position;
            Vector2 forward = transform.up;
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
        m_Rigidbody.SetRotation(m_Rigidbody.rotation - 400f / 2 * Time.fixedDeltaTime);
    }

    public void SteerLeft()
    {
        m_Rigidbody.SetRotation(m_Rigidbody.rotation + 400f/2 * Time.fixedDeltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = "Speed: " + carSpeed;
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

        if (Input.GetKeyDown(KeyCode.A))
        {
            SteerLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SteerRight();
        }
    }
}
