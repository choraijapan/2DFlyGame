using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform followTarget;

    GameManager manager;
    [Range(0, 1)]
    public float smooth = 1f;
    Vector3 offset;
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        offset = transform.position - followTarget.position;
        offset.y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.gameIsStart)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(followTarget.position.x, 0, 0) + offset, smooth);
        }
    }
}
