using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public static UI Instance { get; private set; } // static singleton

    private GameObject canvas;

    private Text write;
    private Text gen;
    private string output;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        write = GameObject.Find("CarText").GetComponent<Text>();
        gen = GameObject.Find("GenerationText").GetComponent<Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        output = "";

        for (int i = 0; i < AgentManager.Instance.agentList.Count; i++)
        {
            CarController car = AgentManager.Instance.agentList[i];

            string carStr = "Car " + car.id + ": ";
            string vals = string.Join(", ", car.inputs.Select(x => x.ToString("N2")));
            string dist = "    Dist: " + car.distance;
            string time = " Time: " + car.time.ToString("N1");
            string fitness = " Fitness: " + (car.network.fitness);

            output += carStr + vals + dist + time + fitness + System.Environment.NewLine;
        }

        write.text = output;
        gen.text = "Generation: " + AgentManager.Instance.currGeneration;
    }
}
