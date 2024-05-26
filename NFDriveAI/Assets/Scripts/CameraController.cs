using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    public GameObject cam1, cam2;
    public TMP_Text timer;
    public float x, y;
    private float zoom = 0;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.OnTimerStart();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //timer.text = EventManager.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cam1.SetActive(true);
            cam2.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cam1.SetActive(false);
            cam2.SetActive(true);
        }

        cam2.transform.position = player.transform.position + new Vector3(0, 0, -10f);
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

    private void OnApplicationQuit()
    {
        EventManager.OnTimerStop();
    }
}
