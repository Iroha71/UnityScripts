using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スタン値をUIに表示する
/// </summary>
public class StunDisplayer : MonoBehaviour
{
    [SerializeField] private Slider stunSlider;
    private StunWatcher stunWatcher;
    // Start is called before the first frame update
    void Start()
    {
        stunWatcher = GetComponentInParent<StunWatcher>();
        stunWatcher.OnAddStun.AddListener(UpdateSlider);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// スタン値をスライダーに反映する
    /// </summary>
    /// <param name="currentStun">現在のスタン値</param>
    private void UpdateSlider(int currentStun) {
        stunSlider.value = (currentStun / stunWatcher.MaxStun) * 100f;
    }
}
