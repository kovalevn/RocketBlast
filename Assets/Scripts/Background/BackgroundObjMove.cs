using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjMove : MonoBehaviour
{
    private GameController gameController;
    private float moveSpeed;
    private float scaleSpeed;
    private float rotationSpeed;
    public bool expand =  true;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        moveSpeed = Random.Range(-1f, -0.2f);
        scaleSpeed = Random.Range(0.05f, 0.15f);
        rotationSpeed = Random.Range(0.05f, 0.15f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameController.RocketDestroyed)
        {
            transform.position += new Vector3(0, moveSpeed, 0) * Time.deltaTime;
            //if (expand) transform.localScale += new Vector3(scaleSpeed, scaleSpeed, 0) * Time.deltaTime;
            transform.Rotate(0, 0, rotationSpeed);
            if (transform.position.y < -7)
            {
                transform.position = -transform.position;
                //transform.localScale = new Vector3(1, 1, 0);
            }
        }
    }
}
