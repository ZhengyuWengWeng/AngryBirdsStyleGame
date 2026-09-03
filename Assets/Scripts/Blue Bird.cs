using UnityEngine;
using Assets.Scripts;

public class BlueBird : Bird
{
    // 分裂后上下两只鸟偏离原飞行方向的角度
    [SerializeField]
    private float splitAngle = 15f;

    // 防止重复分裂
    private bool hasSplit = false;

    void Update()
    {
        // 只有小蓝鸟已经飞出去以后才能使用技能
        if (State != BirdState.Thrown)
        {
            return;
        }

        // 每只蓝鸟只能分裂一次
        if (hasSplit)
        {
            return;
        }

        // 飞行过程中再次点击屏幕
        if (Input.GetMouseButtonDown(0))
        {
            Split();
        }
    }

    private void Split()
    {
        // 标记已经使用过技能
        hasSplit = true;

        // 获取当前蓝鸟的飞行速度
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 currentVelocity = rb.velocity;

        // 原来的蓝鸟继续沿原方向飞
        // 额外生成上、下两只
        CreateSplitBird(currentVelocity, splitAngle);
        CreateSplitBird(currentVelocity, -splitAngle);
    }

    private void CreateSplitBird(Vector2 originalVelocity, float angle)
    {
        // 复制当前正在飞行的小蓝鸟
        GameObject newBird = Instantiate(
            gameObject,
            transform.position,
            transform.rotation
        );

        // 获取复制出来的小蓝鸟
        BlueBird newBlueBird = newBird.GetComponent<BlueBird>();

        if (newBlueBird != null)
        {
            // 分裂出来的小鸟不能再继续分裂
            newBlueBird.hasSplit = true;

            // 禁用技能脚本
            // 它仍然可以靠 Rigidbody2D 正常飞行和碰撞
            newBlueBird.enabled = false;
        }

        // 获取复制出来的小鸟的 Rigidbody
        Rigidbody2D newRb = newBird.GetComponent<Rigidbody2D>();

        // 保证它处于正常物理状态
        newRb.isKinematic = false;

        // 给它新的飞行方向
        newRb.velocity = RotateVelocity(
            originalVelocity,
            angle
        );

        // 防止复制出来的小鸟一直留在场景里
        Destroy(newBird, 8f);
    }

    private Vector2 RotateVelocity(Vector2 velocity, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            velocity.x * cos - velocity.y * sin,
            velocity.x * sin + velocity.y * cos
        );
    }
}