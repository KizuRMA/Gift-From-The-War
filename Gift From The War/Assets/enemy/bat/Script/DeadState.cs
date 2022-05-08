using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeadState : BaseState
{
    private NavMeshAgent agent;
    [SerializeField] private GameObject BoxPrefab;

    // Start is called before the first frame update

    private void Awake()
    {
        myController = GetComponent<BatController>();
        agent = GetComponent<NavMeshAgent>();
        //ナビメッシュによるオブジェクトの回転を更新しない
        agent.isStopped = true;
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.updatePosition = false;
    }

    public override void Start()
    {
       
      
    }

    // Update is called once per frame
    public override void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + myController.hight, transform.position.z);
        Instantiate(BoxPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
