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
    private string output;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        write = GameObject.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        output = "";

        for (int i = 0; i < AgentManager.Instance.agentList.Count; i++)
        {
            CarController car = AgentManager.Instance.agentList[i].GetComponent<CarController>();

            string carStr = "Car " + i + ": ";
            string vals = string.Join(", ", car.outputs.Select(x => x.ToString("N2")));
            string dist = "    Dist: " + car.distance;
            string time = " Time: " + car.time.ToString("N1");

            output += carStr + vals + dist + time + System.Environment.NewLine;
        }

        write.text = output;
    }
}
