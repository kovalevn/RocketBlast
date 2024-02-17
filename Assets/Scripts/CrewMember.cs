using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMember : MonoBehaviour
{
    protected GameController gameController;
    protected Rigidbody2D body;
    protected AudioSource audioSource;
    protected SpriteRenderer spriteRenderer;
    protected Sprite bodySprite;
    protected Sprite faceSprite;

    public string CrewMemberName = "Frank";
    public bool CrewMemberEjected = false;
    public float EjectTime;
    public float PrizeMoney;
    private float bodyRotationSpeed;

    private Vector2 initialPosition;
    private Vector2 initalScale;

    public int Deposit;
    public int Tickets;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameController = FindObjectOfType<GameController>();
        body = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        bodySprite = gameController.BodySprites[Random.Range(0, gameController.BodySprites.Length)];
        faceSprite = gameController.NftSprites[Random.Range(0, gameController.NftSprites.Length)];
        spriteRenderer.sprite = faceSprite;

        bodyRotationSpeed = Random.Range(30, 70) * Mathf.Sign(Random.Range(-1, 1));

        initialPosition = transform.position;
        initalScale = transform.localScale;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!CrewMemberEjected && gameController.RocketDestroyed)
        {
            CrewMemberDead();
        }
    }

    protected void EjectCrewMember()
    {
        spriteRenderer.sprite = bodySprite;
        audioSource.Play();
        transform.SetParent(null);
        CrewMemberEjected = true;
        body.isKinematic = false;
        var fallAngle = Mathf.Sign(Random.Range(-1, 1)) * 200;
        body.AddForce(new Vector2(fallAngle, 200));
        body.AddTorque(50 * Mathf.Sign(-fallAngle));
    }

    protected void CrewMemberDead()
    {
        spriteRenderer.sprite = bodySprite;
        transform.SetParent(null);
        body.rotation += bodyRotationSpeed * Time.deltaTime;
        if (transform.localScale.x > 0 && transform.localScale.y > 0) transform.localScale -= new Vector3(0.2f, 0.2f, 0) * Time.deltaTime;
    }
    public void RestoreCrewMember()
    {
        bodySprite = gameController.BodySprites[Random.Range(0, gameController.BodySprites.Length)];
        faceSprite = gameController.NftSprites[Random.Range(0, gameController.NftSprites.Length)];
        CrewMemberEjected = false;
        spriteRenderer.sprite = faceSprite;
        body.isKinematic = true;
        body.angularVelocity = 0;
        body.velocity = Vector2.zero;
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        transform.localScale = initalScale;
        transform.SetParent(FindObjectOfType<RocketBehavior>().transform);
    }
}
