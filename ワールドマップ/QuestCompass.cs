using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Invector.vCamera;
using TMPro;
using System.Linq;
using PixelCrushers.QuestMachine;
using Cysharp.Threading.Tasks;

/// <summary>
/// コンパスの表示を行う
/// </summary>
public class QuestCompass : MonoBehaviour
{
    /// <summary>
    /// 画面に表示するコンパス用RawImage
    /// </summary>
    private RawImage compass;
    /// <summary>
    /// プレイヤー用カメラ(vThirdPersonCamera)
    /// </summary>
    private Transform playerCamera;
    /// <summary>
    /// コンパスの描画領域の広さ
    /// </summary>
    private float compassUnit;
    /// <summary>
    /// 描画中のマーカー情報リスト
    /// </summary>
    private List<MakerDrawInfo> drawMakers = new();
    /// <summary>
    /// シーン内に存在しているMakerPointのリスト
    /// </summary>
    public MakerPoint[] existMakers;
    /// <summary>
    /// コンパスに描画する際に利用するImageプレハブ
    /// </summary>
    [SerializeField] private Image makerImage;
    // Start is called before the first frame update
    async void Start()
    {
        InitCompass(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        existMakers = FindObjectsOfType<MakerPoint>();

        await UniTask.Delay(1000);
        QuestMachine.GiveQuest("初めの一歩");
    }

    // Update is called once per frame
    void Update()
    {
        FetchCompassRotation();
        DrawMakers(drawMakers);
    }

    /// <summary>
    /// 必要コンポーネントの取得とコンパスの回転の初期化を行う
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void InitCompass(Scene scene, LoadSceneMode mode) {
        compass = GetComponent<RawImage>();
        playerCamera = FindObjectOfType<vThirdPersonCamera>().transform;
        compass.uvRect = new Rect(playerCamera.localEulerAngles.y / 360f, 0f, 1f, 1f);
        compassUnit = compass.rectTransform.rect.width / 360f;
    }

    /// <summary>
    /// カメラの回転とコンパスの回転を同期する
    /// </summary>
    private void FetchCompassRotation() {
        Rect tmp = compass.uvRect;
        tmp.x = playerCamera.localEulerAngles.y / 360f;
        compass.uvRect = tmp;
    }

    /// <summary>
    /// クエストマーカーを再描画するクラスメソッド
    /// </summary>
    /// <param name="quest">再描画するマーカーの対象クエスト</param>
    public static void ReDrawQuestMaker(Quest quest) {
        QuestCompass compass = FindObjectOfType<QuestCompass>();
        compass.UpdateQuestMaker(quest, compass.GetCurrentQuestNode(quest));
    }

    /// <summary>
    /// 指定クエストのマーカー情報を更新する
    /// </summary>
    /// <param name="quest">マーカーを更新する対象クエスト</param>
    /// <param name="nextNode">次のマーカーを表示する対象のノード</param>
    private void UpdateQuestMaker(Quest quest, string nextNode="") {
        // 更新対象クエストのマーカーのうち、既に描画中のマーカーを検索する
        MakerDrawInfo currentQuestMaker = drawMakers.FirstOrDefault(maker => maker.makerPoint.Quest.title.text.Equals(quest.title.text));

        // マーカーが重複しないように、同クエストで描画中のマーカーは削除する
        if (currentQuestMaker != null)
            DestroyMakerBy(currentQuestMaker.makerPoint.makerName);
        if (nextNode.Equals(""))
            return;

        // 最新のノード名に対応するマーカーを検索する
        MakerPoint nextMaker = existMakers.FirstOrDefault(existMaker => existMaker.makerName.Equals(quest.id.text+nextNode));
        if (nextMaker != null)
            AddMaker(nextMaker);
    }

    /// <summary>
    /// 現在の最新のクエストノード名を取得する
    /// </summary>
    /// <param name="quest">ノードを取得したいクエスト</param>
    /// <returns>最新のノード名</returns>
    private string GetCurrentQuestNode(Quest quest) {
        // NodeListのうち、Activeのノードを検索する
        QuestNode currentNode = quest.nodeList.FirstOrDefault(node => QuestMachine.GetQuestNodeState(quest.id.text, node.id.text) == QuestNodeState.Active);
        return currentNode != null ? currentNode.id.text : string.Empty;
    }

    /// <summary>
    /// フレームごとにマーカーをコンパスへ描画する
    /// </summary>
    /// <param name="_makers">描画するマーカーイメージのインスタンス</param>
    private void DrawMakers(List<MakerDrawInfo> _makers) {
        if (drawMakers.Count <= 0)
            return;
        foreach (MakerDrawInfo maker in drawMakers) {
            maker.ShowDistance(playerCamera.position);
            maker.instance.rectTransform.anchoredPosition = GetMakerPositionOnCompass(maker);
        }
    }

    /// <summary>
    /// マーカーをコンパスへ追加する
    /// </summary>
    /// <param name="maker">追加するマーカーの情報</param>
    public void AddMaker(MakerPoint maker) {
        Image _maker = Instantiate(makerImage, transform);
        _maker.GetComponent<Image>().sprite = maker.icon;
        drawMakers.Add(new MakerDrawInfo(_maker, maker));
    }

    /// <summary>
    /// マーカーをコンパス上に描画するための位置情報を取得する
    /// </summary>
    /// <param name="maker">描画するマーカー</param>
    /// <returns>コンパス上のマーカーの座標</returns>
    private Vector2 GetMakerPositionOnCompass(MakerDrawInfo maker) {
        Vector2 playerCameraPosition = new Vector2(playerCamera.transform.position.x, playerCamera.position.z);
        Vector2 playerCameraForward = new Vector2(playerCamera.forward.x, playerCamera.forward.z);
        float currentAngle = Vector2.SignedAngle(maker.Position - playerCameraPosition, playerCameraForward);

        return new Vector2(compassUnit * currentAngle, 0f);
    }

    /// <summary>
    /// 特定名のマーカーを削除する
    /// </summary>
    /// <param name="makerName">削除したいマーカ名</param>
    public void DestroyMakerBy(string makerName) {
        List<MakerDrawInfo> makers = drawMakers.Where<MakerDrawInfo>(drawMaker => drawMaker.makerPoint.makerName.Equals(makerName)).ToList<MakerDrawInfo>();

        if (makers.Count <= 0)
            return;
        foreach (MakerDrawInfo maker in makers) {
            Destroy(maker.instance.gameObject);
            drawMakers.Remove(maker);
        }
    }
}

/// <summary>
/// マーカー描画のために必要な座標とインスタンスを保持する
/// </summary>
public class MakerDrawInfo {
    /// <summary>
    /// コンパスに生成済みのマーカー
    /// </summary>
    public Image instance;
    /// <summary>
    /// マーカーのワールド座標情報
    /// </summary>
    public MakerPoint makerPoint;
    /// <summary>
    /// マーカー距離表示用テキスト表示
    /// </summary>
    public TextMeshProUGUI distanceText;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="instance">生成済みマーカー</param>
    /// <param name="makerPoint">マーカーのワールド座標情報</param>
    public MakerDrawInfo(Image instance, MakerPoint makerPoint) {
        this.instance = instance;
        this.makerPoint = makerPoint;
        distanceText = instance.GetComponentInChildren<TextMeshProUGUI>();
    }

    public Vector2 Position { get { return new Vector2(makerPoint.transform.position.x, makerPoint.transform.position.z); } }

    /// <summary>
    /// プレイヤーとマーカーの距離を取得する
    /// </summary>
    /// <param name="player">プレイヤーの座標</param>
    /// <returns>プレイヤーとマーカーの距離</returns>
    private float GetDistance(Vector3 player) {
        return Vector3.Distance(player, makerPoint.transform.position);
    }

    /// <summary>
    /// プレイヤーとマーカーの距離をテキストで表示する
    /// </summary>
    /// <param name="player">プレイヤーの座標</param>
    public void ShowDistance(Vector3 player) {
        distanceText.text = ((int)GetDistance(player)).ToString();
    }
}
