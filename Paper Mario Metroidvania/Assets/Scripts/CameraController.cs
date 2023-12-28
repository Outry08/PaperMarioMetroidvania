using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Camera cam;
    public PlayerController player;

    public Transform leftBound;
    public Transform rightBound;
    public Transform topBound;
    public Transform bottomBound;

    // Start is called before the first frame update
    void Start()
    {
        //leftBound = null;
        //rightBound = null;
        //topBound = null;
        //bottomBound = null;
    }

    // Update is called once per frame
    void Update()
    {
        float camx = cam.gameObject.transform.position.x;
        float camy = cam.gameObject.transform.position.y;

        //I wish this worked:
        //float leftEdge = camx - cam.sensorSize.x / 2 - 2;
        //float rightEdge = camx + cam.sensorSize.x / 2 - 2;
        //float topEdge = camy + cam.sensorSize.y / 2;
        //float bottomEdge = camy - cam.sensorSize.y / 2;
        float leftEdge = camx - 13.5f;
        float rightEdge = camx + 13.5f;
        float topEdge = camy + 6.7f;
        float bottomEdge = camy - 6.7f;

        float playerx = player.transform.position.x;
        float playery = player.transform.position.y;

        if (!player.isDead()) {
            if (leftBound != null && leftEdge + playerx - camx < leftBound.position.x)
                playerx = leftBound.position.x + camx - leftEdge;
            if (rightBound != null && rightEdge + playerx - camx > rightBound.position.x)
                playerx = rightBound.position.x + camx - rightEdge;

            if (topBound != null && topEdge + playery - camy > topBound.position.y)
                playery = topBound.position.y + camy - topEdge;
            if (bottomBound != null && bottomEdge + playery - camy < bottomBound.position.y)
                playery = bottomBound.position.y + camy - bottomEdge;

            cam.gameObject.transform.SetPositionAndRotation(new Vector3(playerx, playery, cam.gameObject.transform.position.z), cam.gameObject.transform.rotation);
        }
        
    }
}
