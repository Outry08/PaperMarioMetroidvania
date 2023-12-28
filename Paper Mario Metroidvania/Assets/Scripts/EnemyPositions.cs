using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPositions : MonoBehaviour
{

    private Transform[] transforms;
    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        transforms = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 1; i < transforms.Length; i++)
        {
            Debug.Log(Mathf.Abs(transforms[i].position.x - cam.position.x));
            if (!transforms[i].gameObject.activeSelf && transforms[i].gameObject.GetComponent<Enemy>().getHealth() > 0 && Mathf.Abs(transforms[i].position.x - cam.position.x) < 20 && Mathf.Abs(transforms[i].position.y - cam.position.y) < 9)
                transforms[i].gameObject.SetActive(true);
            else if (transforms[i].gameObject.activeSelf && (Mathf.Abs(transforms[i].position.x - cam.position.x) >= 20 || Mathf.Abs(transforms[i].position.y - cam.position.y) >= 9))
                transforms[i].gameObject.SetActive(false);
        }
    }
}
