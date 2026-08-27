using UnityEngine;
using Assets.Scripts;

public class BlueBird : Bird
{
    public float splitAngle = 20f;

    public float splitSpeedMultiplier = 1f;

    private bool hasSplit = false;


    void Update()
    {
        // The bird can only split after it has been thrown.
        if (State != BirdState.Thrown)
            return;

        // The split ability can only be used once.
        if (hasSplit)
            return;

        // Detect the second mouse click.
        if (Input.GetMouseButtonDown(0))
        {
            Split();
        }
    }


    private void Split()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 currentVelocity = rb.velocity;

        // Do not split if the bird is almost stationary.
        if (currentVelocity.sqrMagnitude < 0.01f)
            return;

        // Mark the split ability as used.
        hasSplit = true;

        // Get the current flying direction.
        Vector2 direction = currentVelocity.normalized;

        // Get the current flying speed.
        float speed = currentVelocity.magnitude * splitSpeedMultiplier;


        // Create three copies of the current blue bird.
        GameObject bird1 = Instantiate(
            gameObject,
            transform.position,
            transform.rotation
        );

        GameObject bird2 = Instantiate(
            gameObject,
            transform.position,
            transform.rotation
        );

        GameObject bird3 = Instantiate(
            gameObject,
            transform.position,
            transform.rotation
        );


        // Get the Rigidbody2D components.
        Rigidbody2D rb1 = bird1.GetComponent<Rigidbody2D>();
        Rigidbody2D rb2 = bird2.GetComponent<Rigidbody2D>();
        Rigidbody2D rb3 = bird3.GetComponent<Rigidbody2D>();


        // Get the BlueBird components.
        BlueBird blueBird1 = bird1.GetComponent<BlueBird>();
        BlueBird blueBird2 = bird2.GetComponent<BlueBird>();
        BlueBird blueBird3 = bird3.GetComponent<BlueBird>();


        // Activate the normal flying state.
        blueBird1.OnThrow();
        blueBird2.OnThrow();
        blueBird3.OnThrow();


        // Calculate three different flying directions.
        Vector2 direction1 =
            Quaternion.Euler(0, 0, splitAngle) * direction;

        Vector2 direction2 =
            direction;

        Vector2 direction3 =
            Quaternion.Euler(0, 0, -splitAngle) * direction;


        // Apply velocity to the three birds.
        rb1.velocity = direction1 * speed;
        rb2.velocity = direction2 * speed;
        rb3.velocity = direction3 * speed;


        // Destroy the original blue bird.
        Destroy(gameObject);
    }
}