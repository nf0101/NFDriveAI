
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

public class CarAgent : MonoBehaviour
{
    private float[,,,,] qtable = new float[31, 31, 11, 11, 3];
    private float learningRate = 0.5f;
    private float discountFactor = 0.9f;
    private float explorationStart = 0.1f;
    private float explorationEnd = 0.01f;
    private int finishLineReward = 1000;
    private int goodDrivingReward = 1;
    private int badDrivingPenalty = -1;
    private int wallPenalty = -5;
    private float decayRate = 0.05f;
    private CapsuleCasting raycastScript;
    private CarController carControllerScript;
    private float[] raycastDistances = new float[12];
    private float[] features = new float[3];
    public TMP_Text actionToPerform, currentStateText, dxStateText, sxStateText;
    private bool collided = false;
    public int totalReward = 0;
    private string filePath => Path.Combine(Application.dataPath + "\\Learning\\", "example.json");

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
            qtable = JsonConvert.DeserializeObject<float[,,,,]>(json);
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
        raycastDistances[6] = raycastScript.rightLatDistance;
        raycastDistances[7] = raycastScript.leftLatDistance;
        raycastDistances[8] = raycastScript.prevLatRight_R;
        raycastDistances[9] = raycastScript.prevLatRight_L;
        raycastDistances[10] = raycastScript.prevLatLeft_R;
        raycastDistances[11] = raycastScript.prevLatLeft_L;
        features[0] = raycastScript.rightRayDistance;
        features[1] = raycastScript.leftRayDistance;
        features[2] = carControllerScript.carSpeed;
        print(totalReward);


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

        //print($"{DiscretizeDistance(raycastDistances[6], 0.25f)}, {DiscretizeDistance(raycastDistances[7], 0.25f)}");
        //(int, int, int, int) testState = GetStateIndex((DiscretizeDistance(raycastDistances[0], 0.75f), DiscretizeDistance(raycastDistances[1], 0.75f), DiscretizeDistance(raycastDistances[6], 0.25f), DiscretizeDistance(raycastDistances[7], 0.25f)));
        //print(DiscretizeDistance(raycastDistances[0], 0.75f));
        
    }

    (int, int, int, int) GetStateIndex((float, float, float, float) distances)
    {
        //print(distances);
        return ((int)(distances.Item1 / 0.025), (int)(distances.Item2 / 0.025), (int)(distances.Item3 / 0.025), (int)(distances.Item4 / 0.025));
    }

    float DiscretizeDistance(float distance, float maxDistance)
    {
        
        if (float.IsNaN(distance))
        {
            return maxDistance;
        }

        if (float.IsNegativeInfinity(distance))
        {
            return 0.0f;
        }
        return (float)(Math.Round(distance * 40, MidpointRounding.AwayFromZero) / 40);
    }

    float GetExplorationRate(int currentEpisode)
    {
        float explorationRate = explorationStart * Mathf.Pow((explorationEnd / explorationStart), currentEpisode * decayRate);
        return explorationRate;
    }

    int GetAction((int, int, int, int) state, int currentEpisode)
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
        for (int action = 0; action < qtable.GetLength(4); action++)
            {

            if (qtable[state.Item1, state.Item2, state.Item3, state.Item4, action] > maxQValue)
                {
                    maxQValue = qtable[state.Item1, state.Item2, state.Item3, state.Item4, action];
                    bestAction = action;
                }
            }
        //print(bestAction);
            return bestAction;
        //}
    }

    float GetMaxQValue((int, int, int, int) state)
    {
        float maxQValue = float.MinValue;
        for (int action = 0; action < qtable.GetLength(4); action++)
        {
            if (qtable[state.Item1, state.Item2, state.Item3, state.Item4, action] > maxQValue)
            {
                maxQValue = qtable[state.Item1, state.Item2, state.Item3, state.Item4, action];
            }
        }

        return maxQValue;
    }

    public void UpdateQTable((int, int, int, int) state, int action, float reward, (int, int, int, int) nextState)
    {
        float maxNextQValue = GetMaxQValue(nextState);
        float currentQValue = qtable[state.Item1, state.Item2, state.Item3, state.Item4, action];
        float newQValue = currentQValue + learningRate * (reward + discountFactor * maxNextQValue - currentQValue);
        qtable[state.Item1, state.Item2, state.Item3, state.Item4, action] = newQValue;
    }

    public void FinishEpisode(int currentEpisode, bool train = true)
    {
        (int, int, int, int) currentState = GetStateIndex((DiscretizeDistance(raycastDistances[0], 0.75f), DiscretizeDistance(raycastDistances[1], 0.75f), DiscretizeDistance(raycastDistances[6], 0.25f), DiscretizeDistance(raycastDistances[7], 0.25f)));
        bool isDone = false;
        int episodeReward = 0;
        int episodeStep = 0;
        int reward = 0;

        if (!isDone)
        {
            carControllerScript.Drive();
            int action = GetAction(currentState, currentEpisode);
            //print(action);
            (int, int, int, int) nextState = currentState;
            currentStateText.text = currentState.ToString();
            dxStateText.text = GetStateIndex((DiscretizeDistance(raycastDistances[2], 0.75f), DiscretizeDistance(raycastDistances[4], 0.75f), DiscretizeDistance(raycastDistances[8], 0.25f), DiscretizeDistance(raycastDistances[10], 0.25f))).ToString();
            sxStateText.text = GetStateIndex((DiscretizeDistance(raycastDistances[3], 0.75f), DiscretizeDistance(raycastDistances[5], 0.75f), DiscretizeDistance(raycastDistances[9], 0.25f), DiscretizeDistance(raycastDistances[11], 0.25f))).ToString();
            if (action == 0) //Non fare nulla
            {
                nextState = currentState;//GetStateIndex((DiscretizeDistance(raycastDistances[0], 0.75f), DiscretizeDistance(raycastDistances[1], 0.75f), DiscretizeDistance(raycastDistances[6], 0.25f), DiscretizeDistance(raycastDistances[7], 0.25f)));
                actionToPerform.text = "Nothing";
                
            }

            if (action == 1) //Gira a destra
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[2], 0.75f), DiscretizeDistance(raycastDistances[4], 0.75f), DiscretizeDistance(raycastDistances[8], 0.25f), DiscretizeDistance(raycastDistances[10], 0.25f)));
                actionToPerform.text = "Right";
                carControllerScript.SteerRight();
            }

            else if (action == 2) //Gira a sinistra
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[3], 0.75f), DiscretizeDistance(raycastDistances[5], 0.75f), DiscretizeDistance(raycastDistances[9], 0.25f), DiscretizeDistance(raycastDistances[11], 0.25f)));
                actionToPerform.text = "Left";
                carControllerScript.SteerLeft();
            }

            //print($"{currentState.Item2};{nextState.Item2}");
            //print($"{nextState.Item1}, {currentState.Item1}) || ({nextState.Item2}, {currentState.Item2}) || ({nextState.Item3} < 3) || ({nextState.Item4} < 3)");
            if (collided)
            {
                //print("Collisione");
                reward = wallPenalty;
            }
            //TODO: rivedere sistema reward
            else if ((nextState.Item1 < currentState.Item1) || (nextState.Item2 < currentState.Item2))
            {
                reward = badDrivingPenalty;
                print($"Bad state, {currentState.Item1}, {nextState.Item1}; {currentState.Item2}, {nextState.Item2}");
            }

            //else if ((nextState.Item3 < 5) || (nextState.Item4 < 5))
            //{
            //    reward = badDrivingPenalty;
            //}

            else if ((nextState.Item1 >= currentState.Item1 && nextState.Item2 >= currentState.Item2 ) && (Math.Abs(nextState.Item3 - nextState.Item4) >= 0 && Math.Abs(nextState.Item3 - nextState.Item4) <= 4))
            {
                reward = goodDrivingReward;
            }

            //else if (true)
            //{

            //}

            //print(reward);
            episodeReward += reward;
            totalReward += reward;
           //print(nextState.Item3 < currentState.Item3 || (nextState.Item3 < currentState.Item3));

            if (train)
            {
                UpdateQTable(currentState, action, reward, nextState);
                currentState = nextState;
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
