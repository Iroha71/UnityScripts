using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mewlist.MassiveClouds;
using System.IO;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TimeManager : MonoBehaviour
{
    [SerializeField] private AtmosPad atmosPad;
    [HideInInspector] public TimeInfo curTime;
    private const int INIT_YEAR = 1240, INIT_MONTH = 4, INIT_DAY = 6;
    private const float DAY_NIGHT_BORDER = 17f, MORNING = 6f, NIGHT = 20f;
    [SerializeField] private List<Light> lights = new List<Light>();
    private enum TimeState { MORNING, NIGHT };
    private TimeState currentTimeState = TimeState.MORNING;
    public bool IsStopUpdate { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        curTime = TimeSaver.Load();
        if (curTime == null)
            InitTime();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
    }

    private void UpdateTime() {
        if (IsStopUpdate)
            return;

        float addTime = (Time.deltaTime * 33f) / 3600f;
        curTime.AddTime(addTime);
        if (curTime.time >= DAY_NIGHT_BORDER && currentTimeState != TimeState.NIGHT) {
            SwitchAllLights(true);
            currentTimeState = TimeState.NIGHT;
        } else if (curTime.time >= MORNING && curTime.time < DAY_NIGHT_BORDER && currentTimeState != TimeState.MORNING) {
            SwitchAllLights(false);
        }
        atmosPad.SetHour(curTime.time);
    }

    public void TimeLapse(float goalTime, float duration) {
        DOVirtual.Float(curTime.time, goalTime, duration, value => {
            curTime.time = value - 24f >= 0f ? value - 24f : value;
            atmosPad.SetHour(curTime.time);
        });
    }

    private void SwitchAllLights(bool isEnabled) {
        foreach (Light light in lights) {
            light.enabled = isEnabled;
        }
    }

    private void InitTime() {
        curTime = new TimeInfo(year: INIT_YEAR, season: INIT_MONTH, day: INIT_DAY);
    }

    public void SetTime(float time) {
        curTime.SetTime(time);
    }

    public void SleepPlayer() {
        SetTime(curTime.time >= DAY_NIGHT_BORDER ? MORNING : NIGHT);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TimeManager))]
public class TimeManagerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("ŠX“”‚ðŽæ“¾‚·‚é")) {
            
        }
        EditorGUILayout.HelpBox("LightsƒtƒHƒ‹ƒ_‚ðLightsƒŠƒXƒg‚É“o˜^", MessageType.Info);
    }
}
#endif

public class TimeInfo {
    public enum Season { SPRING, SUMMTER, FALL, WINTER };
    public int year;
    public int season;
    public int day;
    public float time;
    private const int DAYLIMIT = 15, SEASONLIMIT = 12;

    public TimeInfo(int year, int season, int day) {
        this.year = year;
        this.season = season;
        this.day = day;
        time = 9f;
    }

    public void AddTime(float time) {
        this.time += time;
        if (this.time > 24f) {
            this.time = 0f;
            UpdateNextDay();
        }
    }

    private void UpdateNextDay() {
        day++;
        if (day > DAYLIMIT) {
            day = 1;
            if (season >= SEASONLIMIT) {
                season = 1;
                year++;
            } else {
                season++;
            }
        }
    }

    public void SetTime(float goalTime) {
        if (goalTime - time <= 0)
            UpdateNextDay();
        time = goalTime;
    }

    public override string ToString() {
        return $"{year}”N {season}ŒŽ{day}“ú";
    }

    public string GetSeasonName(int season) {
        string[] seasonNames = {
            "Œ¹—¬",
            "ŠJ‹™",
            "ŠÉ–q",
            "”_k",
            "”É‰h",
            "ŒŽ‰B",
            "Žë—Â",
            "—ö•ç",
            "R”»",
            "»o",
            "–é‘é",
            "ŽRŠx"
        };

        return seasonNames[season - 1];
    }
}

public static class TimeSaver {
    private readonly static string SAVE_FILE = Application.dataPath + Path.DirectorySeparatorChar + "TimeInfo.json";
    public static void Save(TimeInfo timeInfo) {
        string timeInfoJson = JsonUtility.ToJson(timeInfo);
        using (StreamWriter writer = new StreamWriter(SAVE_FILE)) {
            writer.WriteLine(timeInfoJson);
        }
        Debug.Log("Time saved" + timeInfo.time);
    }

    public static TimeInfo Load() {
        if (!IsExist())
            return null;
        string loadTimeInfo;
        using (StreamReader reader = new StreamReader(SAVE_FILE)) {
            loadTimeInfo = reader.ReadToEnd();
        }

        return JsonUtility.FromJson<TimeInfo>(loadTimeInfo);
    }

    private static bool IsExist() {
        return File.Exists(SAVE_FILE);
    }
}
