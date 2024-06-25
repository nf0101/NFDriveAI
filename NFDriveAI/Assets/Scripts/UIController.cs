using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    CarAgentFixed carAgent;
    // Start is called before the first frame update
    void Start()
    {
        carAgent = GameObject.FindGameObjectWithTag("Player").GetComponent<CarAgentFixed>();
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TaskOnClick()
    {
        carAgent.SaveResults();

    }
}
