using UnityEngine;
using System.Collections;
using Assets.Scripts;

[RequireComponent(typeof(Rigidbody2D))]
public class BlueBird : MonoBehaviour
{
    // The angle of the two split birds.
    public float SplitAngle = 30f;

    // Distance between the three birds when splitting.
    public float SplitPositionOffset = 0.15f;

    // Prevent the bird from splitting more than once.
    private bool hasSplit = false;


    void Start()
    {
        GetComponent<TrailRenderer>().enabled = false;

        GetComponent<TrailRenderer>().sortingLayerName = "Foreground";

        GetComponent<Rigidbody2D>().isKinematic = true;

        GetComponent<CircleCollider2D>().radius =
            Constants.BirdColliderRadiusBig;

        State = BirdState.BeforeThrown;
    }


    void Update()
    {
        if (State != BirdState.Thrown)
            return;

        if (hasSplit)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Split();
        }
    }


    void FixedUpdate()
    {
        if (State == BirdState.Thrown &&
            GetComponent<Rigidbody2D>().velocity.sqrMagnitude <= Constants.MinVelocity)
        {
            StartCoroutine(DestroyAfter(2));
        }
    }


    public void OnThrow()
    {
        GetComponent<AudioSource>().Play();

        GetComponent<TrailRenderer>().enabled = true;

        GetComponent<Rigidbody2D>().isKinematic = false;

        GetComponent<CircleCollider2D>().radius =
            Constants.BirdColliderRadiusNormal;

        State = BirdState.Thrown;
    }


    private void Split()
    {
        if (hasSplit)
            return;

        Rigidbody2D originalRb = GetComponent<Rigidbody2D>();

        Vector2 originalVelocity = originalRb.velocity;

        if (originalVelocity.sqrMagnitude < 0.001f)
            return;

        hasSplit = true;

        Vector3 originalPosition = transform.position;

        float speed = originalVelocity.magnitude;

        Vector2 direction = originalVelocity.normalized;

        // Get the direction perpendicular to the flying direction.
        Vector2 perpendicular =
            new Vector2(-direction.y, direction.x);


        // =====================================================
        // Middle bird
        // =====================================================

        GameObject middleObject = Instantiate(
            gameObject,
            originalPosition,
            transform.rotation
        );

        BlueBird middleBird =
            middleObject.GetComponent<BlueBird>();

        Rigidbody2D middleRb =
            middleObject.GetComponent<Rigidbody2D>();

        middleBird.hasSplit = true;

        middleBird.State = BirdState.Thrown;

        middleRb.isKinematic = false;

        middleRb.velocity = originalVelocity;

        middleObject.GetComponent<CircleCollider2D>().radius =
            Constants.BirdColliderRadiusNormal;

        middleObject.GetComponent<TrailRenderer>().enabled = true;


        // =====================================================
        // Upper bird
        // =====================================================

        Vector3 upperPosition =
            originalPosition +
            (Vector3)(perpendicular * SplitPositionOffset);

        GameObject upperObject = Instantiate(
            gameObject,
            upperPosition,
            transform.rotation
        );

        BlueBird upperBird =
            upperObject.GetComponent<BlueBird>();

        Rigidbody2D upperRb =
            upperObject.GetComponent<Rigidbody2D>();

        upperBird.hasSplit = true;

        upperBird.State = BirdState.Thrown;

        upperRb.isKinematic = false;

        Vector2 upperDirection =
            RotateVector(direction, SplitAngle);

        upperRb.velocity =
            upperDirection * speed;

        upperObject.GetComponent<CircleCollider2D>().radius =
            Constants.BirdColliderRadiusNormal;

        upperObject.GetComponent<TrailRenderer>().enabled = true;


        // =====================================================
        // Lower bird
        // =====================================================

        Vector3 lowerPosition =
            originalPosition -
            (Vector3)(perpendicular * SplitPositionOffset);

        GameObject lowerObject = Instantiate(
            gameObject,
            lowerPosition,
            transform.rotation
        );

        BlueBird lowerBird =
            lowerObject.GetComponent<BlueBird>();

        Rigidbody2D lowerRb =
            lowerObject.GetComponent<Rigidbody2D>();

        lowerBird.hasSplit = true;

        lowerBird.State = BirdState.Thrown;

        lowerRb.isKinematic = false;

        Vector2 lowerDirection =
            RotateVector(direction, -SplitAngle);

        lowerRb.velocity =
            lowerDirection * speed;

        lowerObject.GetComponent<CircleCollider2D>().radius =
            Constants.BirdColliderRadiusNormal;

        lowerObject.GetComponent<TrailRenderer>().enabled = true;


        // =====================================================
        // Destroy the original bird
        // =====================================================

        Destroy(gameObject);
    }


    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        float x =
            vector.x * cos -
            vector.y * sin;

        float y =
            vector.x * sin +
            vector.y * cos;

        return new Vector2(x, y).normalized;
    }


    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Destroy(gameObject);
    }


    public BirdState State { get; private set; }
}