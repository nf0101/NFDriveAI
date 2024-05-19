using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CameraController : MonoBehaviour
{
    public GameObject player, player2;
    public GameObject cam1, cam2, cam3;
    public float x, y;
    private float zoom = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
            cam3.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cam1.SetActive(false);
            cam2.SetActive(true);
            cam3.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            cam1.SetActive(false);
            cam2.SetActive(false);
            cam3.SetActive(true);
        }
        cam2.transform.position = player.transform.position + new Vector3(0, 0, -10f);
        cam3.transform.position = player2.transform.position + new Vector3(0, 0, -10f);
        x = Input.mouseScrollDelta.x;
        y = Input.mouseScrollDelta.y;
        zoom = Input.mouseScrollDelta.y;
        if (cam2.GetComponent<Camera>().orthographicSize >= 1)
        {
            cam2.GetComponent<Camera>().orthographicSize -= (zoom * 0.2f);
        }
        else
        {
            cam2.GetComponent<Camera>().orthographicSize = 1;
        }
        
    }
}
