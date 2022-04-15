using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Invector.vCharacterController;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using Invector.vItemManager;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using PixelCrushers.QuestMachine;
using MagicaCloth;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// �S�̃G���A�Ɋւ��鐧����s��
/// </summary>
public class AreaManager : MonoBehaviour
{
    public string areaName;
    [SerializeField] private TextMeshProUGUI areaNameText;
    [SerializeField] private AudioSource managerAudio;
    TimeManager timeManager;
    private SavedGameData savedGameData;
    private bool isShowedWorldName = false;
    // Start is called before the first frame update
    void Start()
    {
        timeManager = GetComponent<TimeManager>();
        RandomizeWind();
    }

    private void Awake() {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable() {
        // DialogueSystem����Z�[�u�ł���悤�ɂ���
        Lua.RegisterFunction("SaveGame", this, SymbolExtensions.GetMethodInfo(() => SaveGame()));
    }

    

    /// <summary>
    /// �G���A���ƌ��ݓ������擾����
    /// </summary>
    /// <param name="areaName">�G���A��</param>
    /// <returns>�G���A���ƌ��ݓ���</returns>
    public string GetAreaSentence(string areaName) {
        return $"{areaName}\n<size=60>{timeManager.curTime.ToString()}</size>";
    }

    /// <summary>
    /// �S�̃G���A�̃G���A�����쐬����
    /// </summary>
    /// <param name="worldName">�S�̃G���A��</param>
    public void CreateWorldInfoSentence(string worldName) {
        if (isShowedWorldName)
            return;
        CreateAreaInfoSentence(worldName);
    }

    /// <summary>
    /// �G���A����HUD�ɕ\������
    /// </summary>
    /// <param name="showText">HUD�ɕ\��������</param>
    public void CreateAreaInfoSentence(string showText) {
        
        vHUDController hud = FindObjectOfType<vHUDController>();
        TextMeshProUGUI _text = Instantiate(areaNameText, hud.transform);
        _text.text = showText;
    }

    /// <summary>
    /// �f�[�^���Z�[�u����
    /// </summary>
    public void SaveGame() {
        // Invector
        vItemManager itemManager = GameObject.FindGameObjectWithTag("Player").GetComponent<vItemManager>();
        vSaveLoadInventory.SaveInventory(itemManager);
        // ���ԕۑ��i�I���W�i���j
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        TimeSaver.Save(timeManager.curTime);
        // DialogueSystem / QuestMachine
        SaveSystem.SaveToSlot(1);
        Debug.Log("SaveGame");
    }

    /// <summary>
    /// �f�[�^�����[�h����
    /// </summary>
    public void LoadGame() {
        
        if (!SaveSystem.HasSavedGameInSlot(1))
            return;
        Debug.Log("LoadGame");
        vItemManager itemManager = GameObject.FindGameObjectWithTag("Player").GetComponent<vItemManager>();
        vSaveLoadInventory.LoadInventory(itemManager);
        SaveSystem.LoadFromSlot(1);
    }

    /// <summary>
    /// �����������_���ɕύX����
    /// </summary>
    public void RandomizeWind() {
        MagicaDirectionalWind wind = FindObjectOfType<MagicaDirectionalWind>();
        wind.Main = Random.Range(1f, 40f);
        wind.DirectionAngleY = Random.Range(-180f, 180f);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AreaManager))]
public class AreaManagerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        if (EditorGUI.EndChangeCheck()) {
            if (!FindObjectOfType<DialogueSystemController>()) {
                EditorGUILayout.HelpBox("�~ DialogueSystem", MessageType.Info);
            }
            if (!FindObjectOfType<QuestMachineConfiguration>()) {
                EditorGUILayout.HelpBox("�~ QuestMachine������܂���", MessageType.Info);
            }
            if (!FindObjectOfType<TimeManager>()) {
                EditorGUILayout.HelpBox("�~ TimeManager", MessageType.Info);
            }
            if (!FindObjectOfType<SaveSystem>()) {
                EditorGUILayout.HelpBox("�~ SaveSystem", MessageType.Info);
            }
            if (!FindObjectOfType<BGMManager>()) {
                EditorGUILayout.HelpBox("�~ BGMManager", MessageType.Info);
            }
        }
    }
}
#endif
