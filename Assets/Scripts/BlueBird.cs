using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Reflection;

public class BlueBird : Bird
{
    public GameObject splitBirdPrefab;
    private bool hasSplit = false;
    public AudioClip splitSound;

    void Start()
    {
        // ===== 在游戏开始时就强制启用 AudioSource，并打印信息 =====
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
        {
            audio.enabled = true;
            Debug.Log($"AudioSource 已强制启用，当前 AudioClip = {(audio.clip != null ? audio.clip.name : "null")}");
        }
        else
        {
            Debug.LogWarning("该小鸟没有 AudioSource 组件！");
        }
    }

    void Update()
    {
        if (State == BirdState.Thrown && !hasSplit && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            Split();
        }
    }

    void Split()
    {
        hasSplit = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 currentVelocity = rb.velocity;

        // ===== 播放音效，并打印日志 =====
        PlaySoundSafe(splitSound);

        GameObject prefabToSpawn = splitBirdPrefab != null ? splitBirdPrefab : gameObject;

        float[] horizontalOffsets = { -2.5f, 0f, 2.5f };
        float verticalOffset = 1.5f;

        for (int i = 0; i < 3; i++)
        {
            GameObject newBird = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            Rigidbody2D newRb = newBird.GetComponent<Rigidbody2D>();
            Vector2 newVelocity = currentVelocity + new Vector2(horizontalOffsets[i], verticalOffset);
            newRb.isKinematic = false;
            newRb.velocity = newVelocity;

            BlueBird newBlue = newBird.GetComponent<BlueBird>();
            if (newBlue != null)
            {
                newBlue.InitializeSplitBird();
                newBlue.hasSplit = true;
                newBlue.PlaySoundSafe(null, 0.3f);
            }
        }

        gameObject.SetActive(false);
        Destroy(gameObject, 0.3f);
    }

    private void InitializeSplitBird()
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.enabled = true;
            trail.sortingLayerName = "Foreground";
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.isKinematic = false;

        CircleCollider2D coll = GetComponent<CircleCollider2D>();
        if (coll != null)
            coll.radius = Constants.BirdColliderRadiusNormal;

        var prop = typeof(Bird).GetProperty("State", BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.CanWrite)
            prop.SetValue(this, BirdState.Thrown);
    }

    private void PlaySoundSafe(AudioClip clip = null, float volumeScale = 1.0f)
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio == null)
        {
            Debug.LogWarning("⚠️ PlaySoundSafe: 没有 AudioSource 组件");
            return;
        }

        // 强制启用
        if (!audio.enabled)
        {
            audio.enabled = true;
            Debug.Log("PlaySoundSafe: 重新启用了被禁用的 AudioSource");
        }

        // 如果传入了新 clip，替换；否则用原有的
        if (clip != null)
            audio.clip = clip;

        // 检查是否有 clip
        if (audio.clip == null)
        {
            Debug.LogWarning("⚠️ PlaySoundSafe: AudioClip 为空，无法播放");
            return;
        }

        Debug.Log($"🔊 正在播放音效: {audio.clip.name}, 音量缩放: {volumeScale}");
        audio.PlayOneShot(audio.clip, volumeScale);
    }
}