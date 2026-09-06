using UnityEngine;
using System.Collections;
using Assets.Scripts;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    private bool destroyScheduled;

    // Use this for initialization
    protected virtual void Start()
    {
        // A split bird can be put into the thrown state immediately after it is
        // instantiated. Do not reset it if Start runs after that happens.
        if (State == BirdState.Thrown)
            return;

        //trailrenderer is not visible until we throw the bird
        GetComponent<TrailRenderer>().enabled = false;
        GetComponent<TrailRenderer>().sortingLayerName = "Foreground";
        //no gravity at first
        GetComponent<Rigidbody2D>().isKinematic = true;
        //make the collider bigger to allow for easy touching
        GetComponent<CircleCollider2D>().radius = Constants.BirdColliderRadiusBig;
        State = BirdState.BeforeThrown;
    }



    protected virtual void FixedUpdate()
    {
        //if we've thrown the bird
        //and its speed is very small
        if (!destroyScheduled &&
            State == BirdState.Thrown &&
            GetComponent<Rigidbody2D>().velocity.sqrMagnitude <= Constants.MinVelocity)
        {
            //destroy the bird after 2 seconds
            destroyScheduled = true;
            StartCoroutine(DestroyAfter(2));
        }
    }

    public virtual void OnThrow()
    {
        BeginFlight(true);
    }

    protected void BeginFlight(bool playSound)
    {
        //play the sound
        if (playSound)
            GetComponent<AudioSource>().Play();
        //show the trail renderer
        GetComponent<TrailRenderer>().enabled = true;
        //allow for gravity forces
        GetComponent<Rigidbody2D>().isKinematic = false;
        //make the collider normal size
        GetComponent<CircleCollider2D>().radius = Constants.BirdColliderRadiusNormal;
        State = BirdState.Thrown;
    }

    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    public BirdState State
    {
        get;
        protected set;
    }
}
