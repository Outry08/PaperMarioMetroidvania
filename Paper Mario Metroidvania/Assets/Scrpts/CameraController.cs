using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Camera cam;
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //double leftEdge = cam.gameObject.transform.position.x - cam.pixelWidth / 2;
        //double rightEdge = cam.gameObject.transform.position.x + cam.pixelWidth / 2;
        //double topEdge = cam.gameObject.transform.position.y + cam.pixelHeight / 2;
        //double bottomEdge = cam.gameObject.transform.position.y - cam.pixelHeight / 2;

        cam.gameObject.transform.SetPositionAndRotation(new Vector3(player.transform.position.x, player.transform.position.y, cam.gameObject.transform.position.z), cam.gameObject.transform.rotation);
    }
}
