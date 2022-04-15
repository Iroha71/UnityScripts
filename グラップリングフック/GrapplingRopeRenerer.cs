using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グラップル時の紐を描画する（グラップリングロープが出る場所＝左手にアタッチ）
/// </summary>
public class GrapplingRopeRenerer : MonoBehaviour
{
    [SerializeField] private int devide = 500;
    private LineRenderer lineRenderer;
    [SerializeField] private AnimationCurve affectCurve;
    [SerializeField] private Vector3 goal;
    private Vector3 currentPosition;
    private const int WAVE_HEIGHT = 1, WAVE_COUNT = 3;
    private Spring spring;
    private GrapplingHook1 gh;
    private bool isLanching = false;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.target = 0;
        gh = GetComponentInParent<GrapplingHook1>();
        gh.OnLanchRope.AddListener(SetGoal);
        gh.OnArrivedRope.AddListener(ResetRope);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isLanching) {
            DrawRope();
        }
    }

    /// <summary>
    /// ロープのゴールを設定
    /// </summary>
    /// <param name="goal"></param>
    private void SetGoal(Vector3 goal) {
        this.goal = goal;
    }

    /// <summary>
    /// ロープの描画を開始（アニメーションイベントから呼び出し）
    /// </summary>
    /// <param name="animEvent"></param>
    public void Lanching(string animEvent) {
        isLanching = true;
    }

    /// <summary>
    /// ロープの描画を消す
    /// </summary>
    private void ResetRope() {
        currentPosition = transform.position;
        spring.Reset();
        if (lineRenderer.positionCount > 0)
            lineRenderer.positionCount = 0;
        isLanching = false;
    }

    /// <summary>
    /// ロープを1フレームごとに描画する
    /// </summary>
    private void DrawRope() {
        if (lineRenderer.positionCount == 0) {
            spring.velocity = 15;
            lineRenderer.positionCount = devide + 1;
        }
        spring.damper = 14;
        spring.strength = 800;
        spring.Update(Time.deltaTime);
        Vector3 up = Quaternion.LookRotation((goal - transform.position).normalized) * Vector3.up;
        currentPosition = Vector3.Lerp(currentPosition, goal, Time.deltaTime * 12f);
        for (int i = 0; i < devide + 1; i++) {
            float delta = i / (float)devide;
            Vector3 offset = up * (float)WAVE_HEIGHT * Mathf.Sin(delta * WAVE_COUNT * Mathf.PI) * spring.value * affectCurve.Evaluate(delta);
            lineRenderer.SetPosition(i, Vector3.Lerp(transform.position, currentPosition, delta) + offset);
        }
    }
}

/// <summary>
/// ロープの関節部分の設定を定義
/// </summary>
public class Spring {
    public float strength;
    public float damper;
    public float target;
    public float velocity;
    public float value;

    public void Update(float deltaTime) {
        float direction = target - value >= 0 ? 1f : -1f;
        float force = Mathf.Abs(target - value) * strength;
        velocity += (force * direction - velocity * damper) * deltaTime;
        value += velocity * deltaTime;
    }

    public void Reset() {
        velocity = 0f;
        value = 0f;
    }
}
