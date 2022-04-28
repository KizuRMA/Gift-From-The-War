using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    private float moveSpeed;
    [SerializeField] private float normalSpeed = 3; // 移動速度
    [SerializeField] private float dashSpeedRaito = 3; //走る速さの倍率
    [SerializeField] private float turnSpeed = 20; //振り向く速さ
    private float turnRaito = 180; //振り向く段階
    private bool moveFlg = false;
    private bool dashFlg = false;

    private CharacterController CC;
    private Vector3 moveVelocity; // キャラの移動速度情報
    private Vector3 moveVec; // 合成用

    public GameObject cam;
    Quaternion cameraRot, characterRot;
    [SerializeField] private float Xsensityvity = 3f, Ysensityvity = 3f;

    bool cursorLock = true;

    //角度の制限用
    [SerializeField] float minX = -45f, maxX = 45f;

    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<CharacterController>(); // 毎フレームアクセスするので、負荷を下げるためにキャッシュしておく
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;

        moveSpeed = normalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (turnRaito >= turnSpeed) //180ターンを使っていなかったら
        {
            CameraMove();
        }

        cameraRot = ClampRotation(cameraRot);

        MiddleClick();

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        UpdateCursorLock();

        MoveKey();
        DashJudge();
    }

    private void FixedUpdate()
    {

        Dash();

        Move();

        CC.Move(moveVec);

    }

    //--------------------------------------------------------------------
    //自作関数
    //--------------------------------------------------------------------
    public void UpdateCursorLock()  //カーソル表示切り替え
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void MiddleClick()
    {
        if (Input.GetMouseButtonDown(2))
        {
            turnRaito = 0;
        }

        if (turnRaito < turnSpeed)
        {
            cameraRot[0] = 0;
            cameraRot[2] = 0;
            turnRaito++;
            cameraRot *= Quaternion.Euler(0, 180 / turnSpeed, 0);
        }
    }

    public void MoveKey()
    {
        moveVelocity.x = Input.GetAxis("Horizontal");
        moveVelocity.z = Input.GetAxis("Vertical");

        if(moveVelocity.x == 0 && moveVelocity.z == 0)
        {
            moveFlg = false;
        }
        else
        {
            moveFlg = true;
        }
    }

    public void Move()
    {

        moveVec = cam.transform.forward * moveVelocity.z + cam.transform.right * moveVelocity.x;
        moveVec.y = 0;

        moveVec.Normalize();
        moveVec *= moveSpeed;

        moveVec.y += Physics.gravity.y;
    }

    public void CameraMove()
    {
        //マウスから角度を計算
        float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;

        //カメラとキャラクターの回転を代入
        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);
    }

    public void DashJudge()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            dashFlg = true;
        }
        else
        {
            dashFlg = false;
        }
    }

    public void Dash()
    {
        if (dashFlg)
        {
            moveSpeed = normalSpeed * dashSpeedRaito;
        }
        else
        {
            moveSpeed = normalSpeed;
        }
    }

    //角度制限関数の作成
    public Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,zはベクトル（量と向き）：wはスカラー（座標とは無関係の量）)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

    //--------------------------------------------------------------------
    //ゲッターセッター
    //--------------------------------------------------------------------
    public bool GetMoveFlg()
    {
        return moveFlg;
    }

    public float GetAngleMin()
    {
        return minX;
    }
}
