using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDrag : MonoBehaviour
{
    //Rayの長さを指定
    [SerializeField]
    private float rayLength = 10;
    public GameObject cube = null;
    private float depth;
    // 飛ばしたRayにあたったオブジェクトのCollider情報を格納
    public RaycastHit hit;
    private Vector3 offset;

    bool rag = false;

    Vector3 mousePos = Vector3.zero;
    private Vector3 moveTo;
    private Vector3 obPos;

    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, rayLength))
            {
                if(!rag)
                {
                    rag = true;
                    depth = Camera.main.transform.InverseTransformPoint(hit.point).z;
                    obPos = hit.collider.gameObject.transform.position;
                    Debug.Log(obPos);
                    mousePos = hit.point;
                    //mousePos.z = obPos.z;

                    offset = mousePos - cube.transform.position;
                    //offset.z = depth;
                    Debug.Log(string.Format("offset[{0}] = mousePos[{1}] - cube[{2}]",offset,mousePos,cube.transform.position));

                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            rag = false;
        }

        if(rag)
        {
            mousePos = Input.mousePosition;
            mousePos.z = depth;


            moveTo = Camera.main.ScreenToWorldPoint(mousePos);
            Debug.Log(moveTo);

            cube.transform.position = moveTo - offset;
        }
    }
}
