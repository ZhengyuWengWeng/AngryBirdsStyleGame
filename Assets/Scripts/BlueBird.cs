using Assets.Scripts;
using UnityEngine;

public class BlueBird : Bird
{
    private bool hasSplit = false;

    void Update()
    {
        // 只有已经被弹射出去后，再点击才分裂
        if (State == BirdState.Thrown &&
            !hasSplit &&
            Input.GetMouseButtonDown(0))
        {
            Split();
        }
    }

    void Split()
    {
        hasSplit = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 velocity = rb.velocity;

        // 复制另外两只，所以加上原来的正好三只
        GameObject birdUp =
            Instantiate(gameObject, transform.position, transform.rotation);

        GameObject birdDown =
            Instantiate(gameObject, transform.position, transform.rotation);

        // 防止复制出来的鸟继续无限分裂
        birdUp.GetComponent<BlueBird>().hasSplit = true;
        birdDown.GetComponent<BlueBird>().hasSplit = true;

        birdUp.GetComponent<Rigidbody2D>().velocity =
            Rotate(velocity, 15f);

        birdDown.GetComponent<Rigidbody2D>().velocity =
            Rotate(velocity, -15f);
    }

    Vector2 Rotate(Vector2 vector, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }
}