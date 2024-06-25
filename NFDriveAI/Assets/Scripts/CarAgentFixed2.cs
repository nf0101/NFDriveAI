
using Newtonsoft.Json;
using System;
using System.IO;
using TMPro;
using UnityEngine;
public class CarAgentFixed2 : MonoBehaviour
{
    private float[,,,,] qtable = new float[31, 31, 31, 31, 3];
    public float learningRateStart = 0.9f;
    public float learningRateEnd = 0.1f;
    public float decayLearning = 0.001f;
    public float discountFactor = 0.9f;
    private float explorationStart = 1f;
    private float explorationEnd = 0.01f;
    private int goodDrivingReward = 1, badDrivingPenalty = -1, wallPenalty = -10;
    private float decayRate = 0.01f;
    private CapsuleCastingFixed raycastScript;
    private CarControllerFixed carControllerScript;
    private float[] raycastDistances = new float[12];
    public TMP_Text actionToPerform, currentStateText, dxStateText, sxStateText, rewardText;
    private bool collided = false, isStreak = true, canLap = true;
    public bool training = true;
    public int totalReward = 0, collisions = 0;
    public string FilePath = "Learning\\Agent0\\AgentF_0_S075.json";
    public TMP_Text timerText;
    private Timer timerScript;
    public double collisionsPerHour, collisionsPerMinute, collisionPerLap;
    public int laps = 0, longestStreak = 0, streak = 0, lapDirection = 1;
    private Vector2 startPosition;
    private Quaternion startRotation;

    private void Save()
    {
        var json = JsonConvert.SerializeObject(qtable);

        File.WriteAllText($"{Application.dataPath}\\{FilePath}", json);
        print($"File saved at path: {Application.dataPath}\\{FilePath}");
    }
    private void Load()
    {
        if (File.Exists($"{Application.dataPath}\\{FilePath}"))
        {
            var json = File.ReadAllText($"{Application.dataPath}\\{FilePath}");
            qtable = JsonConvert.DeserializeObject<float[,,,,]>(json);
            print($"Loaded file from {Application.dataPath}\\{FilePath}");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        //rint(GetExplorationRate(1));
        //Load();
        raycastScript = gameObject.GetComponent<CapsuleCastingFixed>();
        carControllerScript = gameObject.GetComponent<CarControllerFixed>();
        timerScript = timerText.GetComponent<Timer>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
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

        AgentLoop(training);
        //rewardText.text = $"Reward: {totalReward}";
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
        if (timerScript.hoursElapsed > 0)
        {
            collisionsPerHour = collisions / timerScript.hoursElapsed;
            collisionsPerMinute = collisions / timerScript.hoursElapsed / 60;
        }


        if (streak > longestStreak)
        {
            longestStreak = streak;
        }
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

    (int, int, int, int) GetStateIndex((float, float, float, float) distances)
    {
        return ((int)(distances.Item1 / 0.025), (int)(distances.Item2 / 0.025), (int)(distances.Item3 / 0.025), (int)(distances.Item4 / 0.025));
    }

    float GetExplorationRate(int currentEpisode)
    {
        float explorationRate = explorationStart * Mathf.Pow((explorationEnd / explorationStart), currentEpisode * decayRate);
        return explorationRate;
    }

    //int GetAction((int, int, int, int) state, int currentEpisode, bool training = true)
    //{
    //    float explorationRate;
    //    if (!training)
    //    {
    //        explorationRate = 0.0f;
    //    }
    //    else
    //    {
    //        explorationRate = GetExplorationRate(currentEpisode);
    //    }
        
    //    float maxQValue = float.MinValue;
    //    int bestAction = 0;

    //    if (UnityEngine.Random.Range(0.1f, 1.1f) < explorationRate)
    //    {
    //        return UnityEngine.Random.Range(0, 3);
    //    }
    //    else
    //    {
    //        for (int action = 0; action < qtable.GetLength(4); action++)
    //    {

    //        if (qtable[state.Item1, state.Item2, state.Item3, state.Item4, action] > maxQValue)
    //        {
    //            maxQValue = qtable[state.Item1, state.Item2, state.Item3, state.Item4, action];
    //            bestAction = action;
    //        }
    //    }
    //        return bestAction;
    //    }
    //}

    int GetAction((int, int, int, int) state, int currentEpisode, bool training = true)
    {
        float explorationRate;
        explorationRate = GetExplorationRate(currentEpisode);


        float maxQValue = float.MinValue;
        int bestAction = 0;

        
        if (!training || explorationRate < UnityEngine.Random.Range(0.1f, 1.1f))
        {
            for (int action = 0; action < qtable.GetLength(4); action++)
            {

                if (qtable[state.Item1, state.Item2, state.Item3, state.Item4, action] > maxQValue)
                {
                    maxQValue = qtable[state.Item1, state.Item2, state.Item3, state.Item4, action];
                    bestAction = action;
                }
            }
            return bestAction;
        }
        else
        {
            return UnityEngine.Random.Range(0, 3);
        }
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

    public void UpdateQTable((int, int, int, int) state, int action, float reward, (int, int, int, int) nextState, int lap)
    {
        float maxNextQValue = GetMaxQValue(nextState);
        float currentQValue = qtable[state.Item1, state.Item2, state.Item3, state.Item4, action];
        float learningRate = learningRateStart * Mathf.Pow((learningRateEnd / learningRateStart), lap * decayLearning);
        float newQValue = currentQValue + Math.Max(0.01f, learningRate) * (reward + discountFactor * maxNextQValue - currentQValue);
        qtable[state.Item1, state.Item2, state.Item3, state.Item4, action] = newQValue;
    }

    public void AgentLoop(bool train = true)
    {
        (int, int, int, int) currentState = GetStateIndex((DiscretizeDistance(raycastDistances[0], 0.75f), DiscretizeDistance(raycastDistances[1], 0.75f), DiscretizeDistance(raycastDistances[6], 0.75f), DiscretizeDistance(raycastDistances[7], 0.75f)));
        bool isDone = false;
        int episodeReward = 0;
        int reward = 0;
        int differenceDistance = 0;

        if (!isDone)
        {
            int action = GetAction(currentState, laps+1, train);
            (int, int, int, int) nextState = currentState;

            currentStateText.text = currentState.ToString();
            //dxStateText.text = GetStateIndex((DiscretizeDistance(raycastDistances[2], 0.75f), DiscretizeDistance(raycastDistances[4], 0.75f), DiscretizeDistance(raycastDistances[8], 0.75f), DiscretizeDistance(raycastDistances[10], 0.75f))).ToString();
            // sxStateText.text = GetStateIndex((DiscretizeDistance(raycastDistances[3], 0.75f), DiscretizeDistance(raycastDistances[5], 0.75f), DiscretizeDistance(raycastDistances[9], 0.75f), DiscretizeDistance(raycastDistances[11], 0.75f))).ToString();
            
            if (action == 0) //Non fare nulla
            {
                nextState = currentState; //GetStateIndex((DiscretizeDistance(raycastDistances[0], 0.75f), DiscretizeDistance(raycastDistances[1], 0.75f), DiscretizeDistance(raycastDistances[6], 0.25f), DiscretizeDistance(raycastDistances[7], 0.25f)));
                actionToPerform.text = "Nothing";

            }

            else if (action == 1) //Gira a destra
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[2], 0.75f), DiscretizeDistance(raycastDistances[4], 0.75f), DiscretizeDistance(raycastDistances[8], 0.75f), DiscretizeDistance(raycastDistances[10], 0.75f)));
                actionToPerform.text = "Right";
                carControllerScript.SteerRight();
            }

            else if (action == 2) //Gira a sinistra
            {
                nextState = GetStateIndex((DiscretizeDistance(raycastDistances[3], 0.75f), DiscretizeDistance(raycastDistances[5], 0.75f), DiscretizeDistance(raycastDistances[9], 0.75f), DiscretizeDistance(raycastDistances[11], 0.75f)));
                actionToPerform.text = "Left";
                carControllerScript.SteerLeft();
            }

            differenceDistance = maxDistance(nextState.Item3, nextState.Item4);

            if (collided)
            {
                //print("Collisione");
                reward = wallPenalty;
            }
            //TODO: rivedere sistema reward
            //else if (((nextState.Item1 < currentState.Item1) && (nextState.Item2 < currentState.Item2)) || ((Mathf.Min(nextState.Item3, nextState.Item4) < differenceDistance - 1)) || (Mathf.Max(nextState.Item3, nextState.Item4) > differenceDistance + 1) || nextState.Item3 <= 5 || nextState.Item4 <= 5)
            //{
            //    reward = badDrivingPenalty;
            //    rewardText.color = Color.red;
            //    //print($"Bad state, {currentState.Item1}, {nextState.Item1}; {currentState.Item2}, {nextState.Item2}");
            //}

            else if (((nextState.Item1 < currentState.Item1) && (nextState.Item2 < currentState.Item2)) || ((nextState.Item4 < differenceDistance - 1)) || (nextState.Item3 > differenceDistance + 1) || nextState.Item3 <= 7 || nextState.Item4 <= 7)
            {
                reward = badDrivingPenalty;
                //rewardText.color = Color.red;
                //print($"Bad state, {currentState.Item1}, {nextState.Item1}; {currentState.Item2}, {nextState.Item2}");
            }

            //else if (!collided && ((nextState.Item1 >= currentState.Item1 && nextState.Item2 >= currentState.Item2) || (nextState.Item1 > 15 && nextState.Item1 > 15 )) && ((Mathf.Min(nextState.Item3, nextState.Item4) >= differenceDistance-1 && Mathf.Max(nextState.Item3, nextState.Item4) <= differenceDistance+1) || Mathf.Abs(nextState.Item3 - nextState.Item4) <= Mathf.Abs(currentState.Item3 - currentState.Item4)))
            //{
            //    reward = goodDrivingReward;
            //    rewardText.color = Color.green;
            //}

            else if (!collided && ((nextState.Item1 >= currentState.Item1 && nextState.Item2 >= currentState.Item2) || (nextState.Item1 > 15 && nextState.Item1 > 15)) && (nextState.Item4 >= differenceDistance - 1 && nextState.Item3 <= differenceDistance + 1 && nextState.Item3 > 8 && nextState.Item4 > 8))
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
                UpdateQTable(currentState, action, reward, nextState, laps);

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
            isStreak = false;
        }
    }

    private int maxDistance(int right, int left)
    {
        int maxDistance = 0;
        maxDistance = (Math.Max(right, left) - Math.Min(right, left)) / 2 + Math.Min(right, left);

        return maxDistance;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (canLap)
        {
            canLap = false;
            if (other.gameObject.tag.Equals("LapCounter"))
            {
                Vector2 dir = other.transform.position - transform.position;
                print($"{ dir.normalized.x}, {lapDirection}");
                if ((dir.normalized.x < 0) == (lapDirection < 0))
                {
                    print("Same direction");
                    laps++;
                    if (isStreak)
                    {
                        streak++;
                        print($"Streak add {streak}");
                    }
                    else
                    {
                        streak = 0;
                        isStreak = true;
                    }

                    collisionPerLap = (double)collisions / laps;

                }
                else
                {
                    print("Changed direction");
                    lapDirection *= -1;
                }
                
            }

            canLap = true;
        }
    }
}