using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    private GameController gameController;
    private Rigidbody2D body;
    private AudioSource audioSource;
    public AudioClip RocketFly;
    public AudioClip RocketStop;
    public ParticleSystem RocketTrace;

    private Vector2 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = RocketFly;
        audioSource.loop = true;
        audioSource.Stop();
        initialPosition = body.position;
        RocketTrace.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10) 
        { 
            body.isKinematic = true;
            transform.position = new Vector3(0,-10,0);
            body.angularVelocity = 0;
            transform.rotation = Quaternion.identity;
        }
        if (!gameController.RocketDestroyed && gameController.isGameStarted) Fly();
    }

    public void StopRocket()
    {
        audioSource.clip = RocketStop;
        audioSource.loop = false;
        audioSource.Play();
        body.isKinematic = false;
        var fallAngle = Random.Range(-100, 100);
        body.AddForce(new Vector2(fallAngle, 0));
        body.AddTorque(50 * Mathf.Sign(-fallAngle));
        RocketTrace.Stop();
    }
    public void RestoreRocket()
    {
        audioSource.clip = RocketFly;
        audioSource.loop = true;
        body.isKinematic = true;
        body.angularVelocity = 0;
        body.velocity = Vector2.zero;
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        Debug.Log("Restored");
    }

    private void Fly()
    {
        if (!RocketTrace.isPlaying) RocketTrace.Play();
        if (!audioSource.isPlaying) audioSource.Play();
        body.rotation = Mathf.Clamp(body.rotation + 5 * Mathf.Sign(Random.Range(-1, 1)) * Time.deltaTime, -10, 10);
        body.position = new Vector2(0, Mathf.Clamp(body.position.y + 0.5f * Mathf.Sign(Random.Range(-1, 1)) * Time.deltaTime, 
            initialPosition.y - 0.5f, initialPosition.y + 0.5f));
    }
}
