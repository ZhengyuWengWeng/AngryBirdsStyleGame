using UnityEngine;
using System.Collections;
using Assets.Scripts;

[RequireComponent(typeof(Rigidbody2D))]
public class BlueBird : Bird
{
    // 蓝鸟只能使用一次分裂技能
    private bool hasSplit = false;

    // 上下两只蓝鸟分开的角度
    public float splitAngle = 15f;

    // 分裂时稍微错开位置，避免三只鸟完全叠在一起
    public float splitOffset = 0.3f;

    private bool destroyStarted = false;


    // BlueBird 自己完成 Bird 原本的初始化
    void Awake()
    {
        // 飞出去之前不显示尾迹
        GetComponent<TrailRenderer>().enabled = false;
        GetComponent<TrailRenderer>().sortingLayerName = "Foreground";

        // 一开始不受重力影响
        GetComponent<Rigidbody2D>().isKinematic = true;

        // 发射前碰撞范围大一点，方便点击
        GetComponent<CircleCollider2D>().radius =
            Constants.BirdColliderRadiusBig;
    }


    // 留空，初始化已经在 Awake 中完成
    void Start()
    {
    }


    void Update()
    {
        // 蓝鸟已经被发射出去以后
        // 再点击一次鼠标，触发分裂
        if (State == BirdState.Thrown &&
            !hasSplit &&
            Input.GetMouseButtonDown(0))
        {
            Split();
        }
    }


    void FixedUpdate()
    {
        // 保留原本 Bird 的销毁逻辑
        if (State == BirdState.Thrown &&
            !destroyStarted &&
            GetComponent<Rigidbody2D>().velocity.sqrMagnitude
                <= Constants.MinVelocity)
        {
            destroyStarted = true;
            StartCoroutine(DestroyAfter(2f));
        }
    }


    void Split()
    {
        hasSplit = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 originalVelocity = rb.velocity;

        // 计算与飞行方向垂直的方向
        Vector2 perpendicular =
            new Vector2(-originalVelocity.y, originalVelocity.x).normalized;

        // 生成上面一只
        CreateSplitBird(
            originalVelocity,
            splitAngle,
            perpendicular * splitOffset
        );

        // 生成下面一只
        CreateSplitBird(
            originalVelocity,
            -splitAngle,
            -perpendicular * splitOffset
        );
    }


    void CreateSplitBird(
        Vector2 originalVelocity,
        float angle,
        Vector2 positionOffset)
    {
        // 复制当前蓝鸟
        GameObject newBird = Instantiate(
            gameObject,
            (Vector2)transform.position + positionOffset,
            transform.rotation
        );

        BlueBird newBlueBird = newBird.GetComponent<BlueBird>();

        // 新生成的小鸟不能继续无限分裂
        newBlueBird.hasSplit = true;

        // 告诉新鸟：你已经被发射了
        newBlueBird.OnThrow();

        // 把原来的速度旋转一定角度
        Vector3 rotatedVelocity =
            Quaternion.Euler(0f, 0f, angle) *
            new Vector3(
                originalVelocity.x,
                originalVelocity.y,
                0f
            );

        newBird.GetComponent<Rigidbody2D>().velocity =
            new Vector2(
                rotatedVelocity.x,
                rotatedVelocity.y
            );
    }


    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}