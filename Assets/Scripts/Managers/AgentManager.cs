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

    public static AgentManager Instance { get; private set; } // static singleton
    public List<GameObject> agentList;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        for (int i = 0; i < n_agents; i++)
        {
            agentList.Add(GameObject.Instantiate(carPrefab, startPos, Quaternion.identity));
        }
    }
}
