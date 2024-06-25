using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedController : MonoBehaviour
{
    // Start is called before the first frame update
    private bool speed = false;
    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TaskOnClick()
    {
        if (!speed)
        {
            Time.timeScale = 50;
            speed = true;
        }
        else
        {
            Time.timeScale = 1;
            speed = false;
        }
        
    }
}
