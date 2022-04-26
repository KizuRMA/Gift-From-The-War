using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class batMove : MonoBehaviour
{
    private CharacterController playerCC;
    private CharacterController batCC;
    private NavMeshAgent _agent;
    Transform rantanTransform;
    [SerializeField] bool moveFlg;
    [SerializeField] float hight;

    // Start is called before the first frame update
    void Start()
    {
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();
        batCC = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
        rantanTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveFlg)
        {
            _agent.destination = playerCC.transform.position;
        }

        rantanTransform.position += new Vector3(0, hight, 0);
    }
}
