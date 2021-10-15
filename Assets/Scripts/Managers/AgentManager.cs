using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField]
    private int n_agents;
    [SerializeField]
    private Vector3 startPos;
    [SerializeField]
    private GameObject carPrefab;
    [SerializeField]
    private int[] layers = new int[] {4, 6, 3, 2};
    [SerializeField]
    [Range(0.0001f, 1f)] private float mutationChance = 0.1f;
    [SerializeField]
    [Range(0.0001f, 1f)] private float mutationStrength = 0.5f;
    [SerializeField]
    private int generations = 100;

    public static AgentManager Instance { get; private set; } // static singleton

    public List<CarController> agentList;
    [HideInInspector]
    public int currGeneration = 0;

    private bool sorted = false;

    private void Update()
    {
        if (currGeneration < generations)
        {
            if (agentList.TrueForAll(a => a.gameObject.activeInHierarchy == false) && sorted == false)
            {
                SortAgents();
                MutateAgents();
                ResetAgents();
                sorted = false;
                currGeneration++;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        // Make n_agents even
        if (n_agents % 2 != 0)
        {
            n_agents = n_agents + 1;
        }

        for (int i = 0; i < n_agents; i++)
        {
            GameObject car = GameObject.Instantiate(carPrefab, startPos, Quaternion.identity);
            NeuralNetwork network = new NeuralNetwork(layers);
            car.GetComponent<CarController>().id = i;
            car.GetComponent<CarController>().network = network;

            agentList.Add(car.GetComponent<CarController>());
        }
    }

    private void SortAgents()
    {
        agentList.Sort();
        sorted = true;
    }

    private void MutateAgents()
    {
        for (int i = 0; i < n_agents / 2; i++)
        {
            // Copy top half of list, to bottom half
            agentList[i].network = agentList[i + n_agents / 2].network.copy(new NeuralNetwork(layers));
            // Mutate bottom half copies
            agentList[i].network.SimpleMutate(mutationChance, mutationStrength);
        }
    }

    private void ResetAgents()
    {
        for (int i = 0; i < n_agents; i++)
        {
            agentList[i].gameObject.transform.position = startPos;
            agentList[i].gameObject.transform.rotation = Quaternion.identity;
            agentList[i].previousCheckTime = Time.time;
            agentList[i].ResetFitness();
            agentList[i].gameObject.SetActive(true);
        }
    }
}
