using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bossMove : MonoBehaviour
{
    private CharacterController playerCC;
    private CharacterController wolfCC;
    private NavMeshAgent _agent;
    [SerializeField] bool moveFlg;

    // Start is called before the first frame update
    void Start()
    {
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();
        wolfCC = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveFlg)
        {
            _agent.destination = playerCC.transform.position;
        }
    }
}
