using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.QuestMachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// コンパス上にマーカーを表示するための機能を実装する
/// </summary>
public class MakerPoint : MonoBehaviour
{
    /// <summary>
    /// 各マーカーを識別するための名前(一意)
    /// </summary>
    public string makerName;
    /// <summary>
    /// クエストコンパス
    /// </summary>
    private QuestCompass compass;
    /// <summary>
    /// コンパスに描画するアイコン
    /// </summary>
    public Sprite icon;
    public Vector2 Position { get { return new Vector2(transform.position.x, transform.position.z); } }

    /* クエスト用 */
    /// <summary>
    /// クエストの進行によって表示を切り替えるタイプか
    /// </summary>
    public bool isQuestMaker = false;
    /// <summary>
    /// このマーカーが表示対象のクエスト
    /// </summary>
    [HideInInspector] public Quest Quest;
    /// <summary>
    /// このマーカーが表示対象のクエストノード名
    /// </summary>
    [HideInInspector] public string QuestNode;
    // Start is called before the first frame update
    void Start()
    {
        if (!compass)
            compass = FindObjectOfType<QuestCompass>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MakerPoint))]
public class MakerPointEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        MakerPoint mp = target as MakerPoint;
        if (mp.isQuestMaker) {
            mp.Quest = (Quest)EditorGUILayout.ObjectField("Quest", mp.Quest, typeof(Quest), false);
            mp.QuestNode = EditorGUILayout.TextField("Quest Node", mp.QuestNode);
            if (!mp.Quest)
                return;
            mp.makerName = mp.Quest.id + mp.QuestNode;
        }
    }
}
#endif