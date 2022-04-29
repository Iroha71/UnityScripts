using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラに演出を与える
/// </summary>
public class CameraShaker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// カメラを振動させる
    /// </summary>
    /// <param name="duration">振動時間</param>
    /// <param name="strength">振動の強さ</param>
    public void ShakeCamera(float duration, float strength) {
        StartCoroutine(VibrateCamera(duration, strength));
    }

    /// <summary>
    /// フレームごとにカメラを振動させる
    /// </summary>
    /// <param name="duration">振動時間</param>
    /// <param name="strength">振動の強さ</param>
    /// <returns></returns>
    private IEnumerator VibrateCamera(float duration, float strength) {
        Vector3 origin = transform.localPosition;
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            float x = origin.x + Random.Range(-1f, 1f) * strength;
            float y = origin.y + Random.Range(-1f, 1f) * strength;
            transform.localPosition = new Vector3(x, y, origin.z);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = origin;
    }
}
