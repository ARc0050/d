using UnityEngine;

public class CamControl : MonoBehaviour
{

    public bool IsInteracte = true;

    public float moveSpeed = 10.0f;

    public float minSize = 2.5f;
    public float maxSize = 12;
    public float zoomSpeed = 150f;

    public float maxZ = 50.0f;
    public float minZ = -50.0f;
    public float maxX = 50.0f;
    public float minX = -50.0f;

    private float mouseWheel;

    private float mouseOffset = 0.01f;



    private bool IsDrag;
    private Vector3 moveEndPos;
    private Vector3 curPos;

    private Camera cam;



    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if(IsInteracte)
        {
            if (!IsDrag)
            {
                mouseInScreenBound();
            }
            mouseMoveCam();
            mouseScrollWheelChangeView();
            restrictCameraPosition();
        }
    }


    

    //鼠标在屏幕边缘控制摄像机位移
    private void mouseInScreenBound()
    {
        Vector3 v1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if(v1.y >= 1 - mouseOffset)
        {
            cam.transform.Translate( Vector3.forward * moveSpeed * Time.deltaTime , Space.World);
        }
        if (v1.y <= mouseOffset)
        {
            cam.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime , Space.World);
        }
        if (v1.x <= mouseOffset)
        {
            cam.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime );
        }
        if (v1.x >= 1 - mouseOffset)
        {
            cam.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime );
        }

    }

    //鼠标滚轮控制放大缩小
    private void mouseScrollWheelChangeView()
    {
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(cam.orthographicSize);
        if (mouseWheel > 0 && cam.transform.position.y >= minSize)
        {
            cam.transform.Translate(Vector3.down * zoomSpeed * Time.deltaTime, Space.World);
        }
        else if (mouseWheel < 0 && cam.transform.position.y <= maxSize)
        {
            cam.transform.Translate(Vector3.up * zoomSpeed * Time.deltaTime, Space.World);
        }
    }

    //限制摄像机范围
    private void restrictCameraPosition()
    {

        

        if(cam.transform.position.z > maxZ)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, maxZ);
        }
        if (cam.transform.position.z < minZ)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, minZ);
        }
        if(cam.transform.position.x < minX)
        {
            cam.transform.position = new Vector3(minX, cam.transform.position.y, cam.transform.position.z);
        }
        if (cam.transform.position.x > maxX)
        {
            cam.transform.position = new Vector3(maxX, cam.transform.position.y, cam.transform.position.z);
        }


    }

    //鼠标滚轮控制位移
    private void mouseMoveCam()
    {
        if (Input.GetMouseButtonDown(2) && !IsDrag)
        {
            moveEndPos.x = Input.mousePosition.x;
            moveEndPos.z = Input.mousePosition.y;

            IsDrag = true;
        }
        if(Input.GetMouseButton(2) && IsDrag)
        {
            curPos.x = Input.mousePosition.x;
            curPos.z = Input.mousePosition.y;
            Vector3 movePos = (curPos - moveEndPos) * moveSpeed * Time.deltaTime;
            cam.transform.position -= movePos;
            moveEndPos = curPos;
        }

        if(Input.GetMouseButtonUp(2) && IsDrag)
        {
            IsDrag = false;
        }
    }

}
