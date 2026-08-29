using UnityEngine;
using System.Collections;
using Assets.Scripts;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public float splitSpeed = 3f;   // 分裂后上下两只鸟的速度偏移

    private bool hasSplit = false;

    // Use this for initialization
    void Start()
    {
        // trailrenderer is not visible until we throw the bird
        GetComponent<TrailRenderer>().enabled = false;
        GetComponent<TrailRenderer>().sortingLayerName = "Foreground";

        // no gravity at first
        GetComponent<Rigidbody2D>().isKinematic = true;

        // make the collider bigger to allow for easy touching
        GetComponent<CircleCollider2D>().radius = Constants.BirdColliderRadiusBig;

        State = BirdState.BeforeThrown;
    }

    void Update()
    {
        // 鸟已经发射后，再点击一次鼠标触发分裂
        if (State == BirdState.Thrown &&
            Input.GetMouseButtonDown(0) &&
            !hasSplit)
        {
            Split();
        }
    }

    void FixedUpdate()
    {
        // if we've thrown the bird
        // and its speed is very small
        if (State == BirdState.Thrown &&
            GetComponent<Rigidbody2D>().velocity.sqrMagnitude <= Constants.MinVelocity)
        {
            // destroy the bird after 2 seconds
            StartCoroutine(DestroyAfter(2));
        }
    }

    public void OnThrow()
    {
        // play the sound
        GetComponent<AudioSource>().Play();

        // show the trail renderer
        GetComponent<TrailRenderer>().enabled = true;

        // allow for gravity forces
        GetComponent<Rigidbody2D>().isKinematic = false;

        // make the collider normal size
        GetComponent<CircleCollider2D>().radius = Constants.BirdColliderRadiusNormal;

        State = BirdState.Thrown;
    }

    void Split()
    {
        hasSplit = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 currentVelocity = rb.velocity;

        // 复制出上方一只鸟
        GameObject upperBird = Instantiate(
            gameObject,
            transform.position,
            transform.rotation
        );

        // 复制出下方一只鸟
        GameObject lowerBird = Instantiate(
            gameObject,
            transform.position,
            transform.rotation
        );

        Bird upperScript = upperBird.GetComponent<Bird>();
        Bird lowerScript = lowerBird.GetComponent<Bird>();

        // 防止复制出的鸟继续分裂
        upperScript.hasSplit = true;
        lowerScript.hasSplit = true;

        // 因为复制出来时 Rigidbody 状态可能继承当前状态
        Rigidbody2D upperRb = upperBird.GetComponent<Rigidbody2D>();
        Rigidbody2D lowerRb = lowerBird.GetComponent<Rigidbody2D>();

        upperRb.isKinematic = false;
        lowerRb.isKinematic = false;

        upperScript.State = BirdState.Thrown;
        lowerScript.State = BirdState.Thrown;

        Vector2 direction = currentVelocity.normalized;
        float speed = currentVelocity.magnitude;

        // 上下各偏转 15 度
        Vector2 upperDirection = Quaternion.Euler(0, 0, 15) * direction;
        Vector2 lowerDirection = Quaternion.Euler(0, 0, -15) * direction;

        upperRb.velocity = upperDirection * speed;
        lowerRb.velocity = lowerDirection * speed;

        // 中间原鸟保持原方向
        rb.velocity = direction * speed;
    }

    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    public BirdState State
    {
        get;
        private set;
    }
}