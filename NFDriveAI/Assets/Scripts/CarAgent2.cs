
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class CarAgent2 : MonoBehaviour
{
    private float[,,,,] qtable = new float[31, 31, 31, 31, 3];
    private float learningRate = 0.5f;
    private float discountFactor = 0.9f;
    private float explorationStart = 0.1f;
    private float explorationEnd = 0.01f;
    private int goodDrivingReward = 1;
    private int badDrivingPenalty = -1;
    private int wallPenalty = -10;
    private float decayRate = 0.05f;
    private CapsuleCasting raycastScript;
    private CarController carControllerScript;
    private float[] raycastDistances = new float[12];
    private float[] features = new float[3];
    //public TMP_Text actionToPerform, currentStateText, dxStateText, sxStateText, rewardText;
    private bool collided = false;
    public int totalReward = 0, collisions = 0;
    private string filePath => Path.Combine(Application.dataPath + "\\Learning\\Agent1\\", "Agent1_0.json");

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
        //Load();
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

        FinishEpisode(1, train: false);

        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }

        // rewardText.text = $"Reward: {totalReward}";

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
        (int, int, int, int) currentState = GetStateIndex((DiscretizeDistance(raycastDistances[0], 0.75f), DiscretizeDistance(raycastDistances[1], 0.75f), DiscretizeDistance(raycastDistances[6], 0.75f), DiscretizeDistance(raycastDistances[7], 0.75f)));
        bool isDone = false;
        int episodeReward = 0;
        int episodeStep = 0;
        int reward = 0;
        int differenceDistance = 0;

        if (!isDone)
        {

            carControllerScript.Drive();
            int action = GetAction(currentState, currentEpisode);
            (int, int, int, int) nextState = currentState;

            //currentStateText.text = currentState.ToString();
            //dxStateText.text = GetStateIndex((DiscretizeDistance(raycastDistances[2], 0.75f), DiscretizeDistance(raycastDistances[4], 0.75f), DiscretizeDistance(raycastDistances[8], 0.75f), DiscretizeDistance(raycastDistances[10], 0.75f))).ToString();
            //sxStateText.text = GetStateIndex((DiscretizeDistance(raycastDistances[3], 0.75f), DiscretizeDistance(raycastDistances[5], 0.75f), DiscretizeDistance(raycastDistances[9], 0.75f), DiscretizeDistance(raycastDistances[11], 0.75f))).ToString();
            if (action == 0) //Non fare nulla
            {
                nextState = currentState;//GetStateIndex((DiscretizeDistance(raycastDistances[0], 0.75f), DiscretizeDistance(raycastDistances[1], 0.75f), DiscretizeDistance(raycastDistances[6], 0.25f), DiscretizeDistance(raycastDistances[7], 0.25f)));
                                         //actionToPerform.text = "Nothing";

            }

            else if (action == 1) //Gira a destra
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[2], 0.75f), DiscretizeDistance(raycastDistances[4], 0.75f), DiscretizeDistance(raycastDistances[8], 0.75f), DiscretizeDistance(raycastDistances[10], 0.75f)));
                //actionToPerform.text = "Right";
                carControllerScript.SteerRight();
            }

            else if (action == 2) //Gira a sinistra
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[3], 0.75f), DiscretizeDistance(raycastDistances[5], 0.75f), DiscretizeDistance(raycastDistances[9], 0.75f), DiscretizeDistance(raycastDistances[11], 0.75f)));
                //actionToPerform.text = "Left";
                carControllerScript.SteerLeft();
            }

            differenceDistance = maxDistance(nextState.Item3, nextState.Item4);

            if (collided)
            {
                //print("Collisione");
                reward = wallPenalty - collisions;
            }
            //TODO: rivedere sistema reward
            //TODO: rivedere sistema reward
            else if (((nextState.Item1 < currentState.Item1) && (nextState.Item2 < currentState.Item2)) || ((Mathf.Min(nextState.Item3, nextState.Item4) < differenceDistance - 1)) || (Mathf.Max(nextState.Item3, nextState.Item4) > differenceDistance + 1) || nextState.Item3 <= 7 || nextState.Item4 <= 7)
            {
                reward = badDrivingPenalty;
                //rewardText.color = Color.red;
                //print($"Bad state, {currentState.Item1}, {nextState.Item1}; {currentState.Item2}, {nextState.Item2}");
            }

            else if (!collided && ((nextState.Item1 >= currentState.Item1 && nextState.Item2 >= currentState.Item2) || (nextState.Item1 > 15 && nextState.Item1 > 15)) && ((Mathf.Min(nextState.Item3, nextState.Item4) >= differenceDistance - 1 && Mathf.Max(nextState.Item3, nextState.Item4) <= differenceDistance + 1) || Mathf.Abs(nextState.Item3 - nextState.Item4) <= Mathf.Abs(currentState.Item3 - currentState.Item4)))
            {
                reward = goodDrivingReward;
                //rewardText.color = Color.green;
            }




            else
            {
                //rewardText.color = Color.white;
            }

            episodeReward += reward;
            totalReward += reward;

            if (train)
            {
                UpdateQTable(currentState, action, reward, nextState);
                
            }
            currentState = nextState;
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
            collisions++;
        }
    }

    private int maxDistance(int right, int left)
    {
        int maxDistance = 0;
        maxDistance = (Math.Max(right, left) - Math.Min(right, left)) / 2 + Math.Min(right, left);

        return maxDistance;
    }

}
