
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class OldCarAgent : MonoBehaviour
{
    private float[,,] qtable = new float[16, 16, 5];
    private float learningRate = 0.5f;
    private float discountFactor = 0.9f;
    private float explorationStart = 0.1f;
    private float explorationEnd = 0.01f;
    private int finishLineReward = 1000;
    private int goodDrivingReward = 50;
    private int badDrivingPenalty = -50;
    private int wallPenalty = -1000;
    private float decayRate = 0.05f;
    private CapsuleCasting raycastScript;
    private CarController carControllerScript;
    private float[] raycastDistances = new float[6];
    private float[] features = new float[3];
    public TMP_Text actionToPerform;
    private bool collided = false;
    private string filePath => Path.Combine(Application.persistentDataPath, "example.json");

    private void Save()
    {
        var json = JsonConvert.SerializeObject(qtable);

        File.WriteAllText(filePath, json);
        print($"File saved at path: {filePath}");
    }
    private void Load()
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            qtable = JsonConvert.DeserializeObject<float[,,]>(json);
            print($"Loaded file from {filePath}");
        }

    }

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
        raycastDistances[0] = raycastScript.rightRayDistance;
        raycastDistances[1] = raycastScript.leftRayDistance;
        raycastDistances[2] = raycastScript.prevRight_R;
        raycastDistances[3] = raycastScript.prevRight_L;
        raycastDistances[4] = raycastScript.prevLeft_R;
        raycastDistances[5] = raycastScript.prevLeft_L;
        features[0] = raycastScript.rightRayDistance;
        features[1] = raycastScript.leftRayDistance;
        features[2] = carControllerScript.carSpeed;


        //string concatenated = string.Join(";", raycastDistances.Select(x => (DiscretizeDistance(x)).ToString()).ToArray());
        //print(GetStateIndex((DiscretizeDistance(raycastDistances[0]), DiscretizeDistance(raycastDistances[1]))));
        //GetStateIndex((DiscretizeDistance(raycastDistances[0]), DiscretizeDistance(raycastDistances[1])));
        FinishEpisode(1);

        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }

    }

    (int, int) GetStateIndex((float, float) distances)
    {
        //print(distances);
        return ((int)(distances.Item1 / 0.05), (int)(distances.Item2 / 0.05));
    }

    float DiscretizeDistance(float distance)
    {

        if (float.IsNaN(distance))
        {
            return 0.75f;
        }

        if (float.IsNegativeInfinity(distance))
        {
            return 0.0f;
        }
        return (float)(Math.Round(distance * 20, MidpointRounding.AwayFromZero) / 20);
    }

    float GetExplorationRate(int currentEpisode)
    {
        float explorationRate = explorationStart * Mathf.Pow((explorationEnd / explorationStart), currentEpisode * decayRate);
        return explorationRate;
    }

    int GetAction((int, int) state, int currentEpisode)
    {
        float explorationRate = GetExplorationRate(currentEpisode);
        float maxQValue = float.MinValue;
        int bestAction = 0;

        //if (UnityEngine.Random.Range(0.1f, 1.1f) > explorationRate)
        //{
        //    return UnityEngine.Random.Range(0, 3);
        //}
        //else
        //{
        for (int action = 0; action < qtable.GetLength(2); action++)
        {
            if (qtable[state.Item1, state.Item2, action] > maxQValue)
            {
                maxQValue = qtable[state.Item1, state.Item2, action];
                bestAction = action;
            }
        }
        return bestAction;
        //}
    }

    float GetMaxQValue((int, int) state)
    {
        float maxQValue = float.MinValue;
        for (int action = 0; action < qtable.GetLength(2); action++)
        {
            if (qtable[state.Item1, state.Item2, action] > maxQValue)
            {
                maxQValue = qtable[state.Item1, state.Item2, action];
            }
        }

        return maxQValue;
    }

    public void UpdateQTable((int, int) state, int action, float reward, (int, int) nextState)
    {
        float maxNextQValue = GetMaxQValue(nextState);
        float currentQValue = qtable[state.Item1, state.Item2, action];
        float newQValue = currentQValue + learningRate * (reward + discountFactor * maxNextQValue - currentQValue);
        qtable[state.Item1, state.Item2, action] = newQValue;
    }

    public void FinishEpisode(int currentEpisode, bool train = true)
    {
        (int, int) currentState = GetStateIndex((DiscretizeDistance(raycastDistances[0]), DiscretizeDistance(raycastDistances[1])));
        bool isDone = false;
        int episodeReward = 0;
        int episodeStep = 0;
        int reward = 0;

        if (!isDone)
        {
            carControllerScript.Drive();
            int action = GetAction(currentState, currentEpisode);
            //print(action);
            (int, int) nextState = (0, 0);

            if (action == 0) //Non fare nulla
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[0]), DiscretizeDistance(raycastDistances[1])));
                actionToPerform.text = "Nothing";

            }

            if (action == 1) //Gira a destra
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[2]), DiscretizeDistance(raycastDistances[4])));
                actionToPerform.text = "Right";
                carControllerScript.SteerRight();
            }

            else if (action == 2) //Gira a sx
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[3]), DiscretizeDistance(raycastDistances[5])));
                actionToPerform.text = "Left";
                carControllerScript.SteerLeft();
            }
            //print($"{currentState.Item2};{nextState.Item2}");
            if (collided)
            {
                print("Collisione");
                reward = wallPenalty;
            }

            else if (nextState.Item1 < currentState.Item1 && nextState.Item1 < 15 || nextState.Item2 < currentState.Item2 && nextState.Item1 < 15)
            {
                reward = badDrivingPenalty;
            }
            else if (nextState.Item1 > currentState.Item1 || nextState.Item2 > currentState.Item2)
            {
                reward = goodDrivingReward;
            }

            //print(reward);
            episodeReward += reward;

            if (train)
            {
                UpdateQTable(currentState, action, reward, nextState);
                //currentState = nextState;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Bound"))
        {

            collided = true;
            //Save();
            //SceneManager.LoadScene(0);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Bound"))
        {
            collided = false;
        }
    }

}