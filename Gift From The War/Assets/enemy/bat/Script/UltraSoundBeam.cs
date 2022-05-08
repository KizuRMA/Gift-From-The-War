using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltraSoundBeam : MonoBehaviour
{
    private CharacterController playerCC;
    [SerializeField] private float longestLength;
    [SerializeField] private float duration;
    [SerializeField] private float length;
    [SerializeField] private float velocity;
    private float defaultVelocity;
    private float defaultDuration;
    private float defaultLength;
    private float defaultLongestLength;
    private float nowTime;
    private bool aliveFlg;

    private void Awake()
    {
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();

        defaultVelocity = velocity;
        defaultLength = length;
        defaultDuration = duration;
        defaultLongestLength = longestLength;
        aliveFlg = true;
    }

    public void Init()
    {
        velocity = defaultVelocity;
        length = defaultLength;
        duration = defaultDuration;
        longestLength = defaultLongestLength;
        aliveFlg = true;
    }

    public bool IsAlive()
    {
        return aliveFlg;
    }

    // Update is called once per frame
    public void Update()
    {
        //初めて更新関数が実行される時
        if(length <= defaultLength)
        {
            nowTime = Time.time;
        }

        //超音波ビームを長くする
        length += velocity;
        length = Mathf.Min(length,longestLength);

        //当たり判定
        Vector3 _firePos = transform.position + (transform.up * 0.3f);
        Vector3 _targetVec = playerCC.transform.position - _firePos;

        float dot = Vector3.Dot(transform.forward.normalized,_targetVec.normalized);

        //Debug.Log(Mathf.Acos(dot) * Mathf.Rad2Deg);
        if (Mathf.Acos(dot) * Mathf.Rad2Deg <= 20.0f)
        {
            //プレイヤーの方向に対する超音波の射程距離を出す。
            float withinRange = length / dot;

            //超音波の範囲ないにプレイヤーがいるか確認する
            if (_targetVec.magnitude - withinRange < 0)
            {
                Ray _ray = new Ray(_firePos, _targetVec);
                RaycastHit _raycastHit;

                //プレイヤーに向かってレイを発射
                bool hit = Physics.Raycast(_ray, out _raycastHit, withinRange);

                //Debug.Log(_raycastHit.collider.gameObject);
                //Debug.Log("超音波が当たった！！");
            }
        }

        //デバッグ用の線を描画
        var lineRenderer = gameObject.GetComponent<LineRenderer>();
        //lineRenderer.hideFlags = HideFlags.None;

        var positions = new Vector3[]
        {
            _firePos,
            _firePos + (transform.forward * length),
        };

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.SetPositions(positions);

        //持続時間を計算
        if (duration + nowTime <= Time.time)
        {
            aliveFlg = false;
            lineRenderer.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}
