using UnityEngine;
using Assets.Scripts;

/// <summary>
/// A bird that splits into three while it is flying.
/// The original bird keeps its direction and two copies fly above and below it.
/// </summary>
public class BlueBird : Bird
{
    [SerializeField, Range(5f, 45f)]
    private float splitAngle = 15f;

    [SerializeField, Min(0f)]
    private float splitOffset = 0.5f;

    private bool hasSplit;

    private void Update()
    {
        // The release that launches the bird does not trigger this because the
        // skill listens for the next mouse/touch press while the bird is flying.
        if (State == BirdState.Thrown &&
            !hasSplit &&
            Input.GetMouseButtonDown(0))
        {
            Split();
        }
    }

    private void Split()
    {
        hasSplit = true;

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        Vector2 velocity = body.velocity;
        Vector2 perpendicular = velocity.sqrMagnitude > 0f
            ? new Vector2(-velocity.y, velocity.x).normalized
            : Vector2.up;

        SpawnSplitBird(perpendicular * splitOffset, splitAngle, velocity);
        SpawnSplitBird(-perpendicular * splitOffset, -splitAngle, velocity);
    }

    private void SpawnSplitBird(Vector2 positionOffset, float angle, Vector2 velocity)
    {
        GameObject clone = Instantiate(
            gameObject,
            transform.position + (Vector3)positionOffset,
            transform.rotation);

        BlueBird cloneBird = clone.GetComponent<BlueBird>();
        cloneBird.hasSplit = true;
        cloneBird.BeginFlight(false);

        clone.GetComponent<Rigidbody2D>().velocity = Rotate(velocity, angle);
    }

    private static Vector2 Rotate(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos);
    }
}
