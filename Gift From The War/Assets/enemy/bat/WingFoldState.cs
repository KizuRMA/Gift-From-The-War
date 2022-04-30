using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WingFoldState : BaseState
{
    private Vector3 rayPosition;
    private Vector3 targetPos;
    private RaycastHit hit;
    private CharacterController playerCC;
    private NavMeshAgent agent;
    private Vector3 hightManager;

    // Start is called before the first frame update
    public override void Start()
    {
        myController = GetComponent<BatController>();
        agent = GetComponent<NavMeshAgent>();
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();

        //�i�r���b�V���ɂ��I�u�W�F�N�g�̉�]���X�V���Ȃ�
        // agent.updateRotation = false;

        //�̂�O�ɌX����
        Vector3 _localAngle = transform.localEulerAngles;
        _localAngle.x = 20.0f;
        transform.localEulerAngles = _localAngle;


        rayPosition = new Vector3(transform.position.x, myController.hight, transform.position.z);
        Ray ray = new Ray(rayPosition, Vector3.up);

        //Ray����ɔ�΂�
        if (Physics.Raycast(ray, out hit))
        {
            //���C���V��ɏՓ˂��Ă���ꍇ�̓^�[�Q�b�g���W�ɐݒ肷��B
            Vector3 targetVec = Vector3.up * hit.distance;
            targetPos = rayPosition + targetVec;
        }
        else
        {
            BatController batCon = gameObject.GetComponent<BatController>();
            batCon.ChangeState(GetComponent<batMove>());
            return;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        //�̂�O�ɌX����
        Vector3 _localAngle = transform.localEulerAngles;
        _localAngle.x += myController.forwardAngle;
        transform.localEulerAngles = _localAngle;

        transform.position = new Vector3(transform.position.x, myController.hight, transform.position.z);

        //��ɏ㏸���鏈��
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.001f);
        myController.hight = transform.position.y;

        //�A�j���[�V�����؂�ւ�����
        //Animator animator = GetComponent<Animator>();

        //int trans = animator.GetInteger("trans");

        //trans++;

        //animator.SetInteger("trans", 1);
    }
}
