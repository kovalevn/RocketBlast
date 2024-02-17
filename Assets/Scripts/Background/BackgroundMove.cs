using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    private GameController gameController;
    private float moveSpeed = -0.15f;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameController.RocketDestroyed)
        {
            transform.position += new Vector3(0, moveSpeed, 0) * Time.deltaTime;
            if (transform.position.y <= -32.4)
            {
                transform.position = new Vector3(0, 32.4f, 0);
            }
        }
    }
}
