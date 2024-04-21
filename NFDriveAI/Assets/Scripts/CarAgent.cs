using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CarAgent : MonoBehaviour
{
    private float[,] qtable = new float[60,5];
    private float learningRate = 0.5f;
    private float discountFactor = 0.9f;
    private float explorationStart = 0.1f;
    private float explorationEnd = 0.01f;
    private int finishLineReward = 1000;
    private int goodDrivingReward = 1;
    private int badDrivingPenalty = -1;
    private int wallPenalty = -1000;
    private float decayRate = 0.05f;
    private CapsuleCasting raycastScript;
    private CarController carControllerScript;
    private float[] features = new float[3];

    // Start is called before the first frame update
    void Start()
    {
        //rint(GetExplorationRate(1));
        raycastScript = gameObject.GetComponent<CapsuleCasting>();
        carControllerScript = gameObject.GetComponent<CarController>();
    }

    // Update is called once per frame
    void Update()
    {
        features[0] = raycastScript.rightRayDistance;
        features[1] = raycastScript.leftRayDistance;
        features[2] = carControllerScript.carSpeed;
       // string concatenated = string.Join(",", features.Select(x => x.ToString()).ToArray());
        //print(concatenated);
        //previsione stato con coordinate hit.point
    }

    float GetExplorationRate(int currentEpisode)
    {
        float explorationRate = explorationStart * Mathf.Pow((explorationEnd / explorationStart), currentEpisode * decayRate);
        return explorationRate;
    }

    int GetAction(int state, int currentEpisode)
    {
        float explorationRate = GetExplorationRate(currentEpisode);
        float maxQValue = float.MinValue;
        int bestAction = 0;

        if (Random.Range(0.1f, 1.1f) > explorationRate)
        {
            return Random.Range(0, 6);
        }
        else
        {
            for (int action = 0; action < qtable.GetLength(1); action++)
            {
                if (qtable[state, action] > maxQValue)
                {
                    maxQValue= qtable[state, action];
                    bestAction = action;
                }
            }
            return bestAction;
        }
    }

    float GetMaxQValue(int state)
    {
        float maxQValue = float.MinValue;
        for (int action = 0; action < qtable.GetLength(1); action++)
        {
            if (qtable[state, action] > maxQValue)
            {
                maxQValue = qtable[state, action];
            }
        }

        return maxQValue;
    }

    public void UpdateQTable(int state, int action, float reward, int nextState)
    {
        float maxNextQValue = GetMaxQValue(nextState);
        float currentQValue = qtable[state, action];
        float newQValue = currentQValue + learningRate * (reward + discountFactor * maxNextQValue - currentQValue);
        qtable[state, action] = newQValue;
    }

    public void FinishEpisode(int currentEpisode, bool train = true)
    {
        int currentState = 0; 
    }
}
