using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

/// <summary>
/// マップ閲覧時の視点とマーカー操作を実装する
/// </summary>
public class MapController : MonoBehaviour
{
    /// <summary>
    /// マップオブジェクト生成時の初期ローカル位置
    /// </summary>
    [SerializeField] private Vector3 initPosition;
    /// <summary>
    /// マップオブジェクト生成時の初期ローカル回転
    /// </summary>
    [SerializeField] private Vector3 initRotation;
    /// <summary>
    /// 視点の高さ定義（0→2 : 低→高）
    /// </summary>
    [SerializeField] private float[] heightLimit = new float[3];
    /// <summary>
    /// プレイヤー位置を示すマーカープレハブ
    /// </summary>
    [SerializeField] private Canvas playerMaker;
    /// <summary>
    /// 入力系
    /// </summary>
    private GenericInput horizontalMove = new GenericInput("Horizontal", "LeftAnalogHorizontal", "Horizontal");
    private GenericInput verticalMove = new GenericInput("Vertical", "LeftAnalogVertical", "Vertical");
    private GenericInput makerPut = new GenericInput("E", "A", "A");
    private GenericInput zoomInput = new GenericInput("Mouse0", "RT", "RT");
    private GenericInput unzoomInput = new GenericInput("Mouse1", "LT", "LT");
    private enum ZoomLevel { HIGH = 2, MIDDLE = 1, LOW = 0 };
    private ZoomLevel currentZoom = ZoomLevel.MIDDLE;
    private const int LOW = 0, HIGH = 2;
    /// <summary>
    /// 地形全体を映しているカメラ
    /// </summary>
    private Camera mapCamera;
    /// <summary>
    /// マップ反映機能
    /// </summary>
    private MapMakerDisplayer mapMakerDisplayer;
    private const string PUT_MAKER = "PutMaker";
    /// <summary>
    /// クエストコンパス機能
    /// </summary>
    private QuestCompass compass;

    // Start is called before the first frame update
    void Start()
    {
        // マップ表示位置をプレイヤー位置にするため、マップ用カメラの位置とプレイヤー位置を取得
        mapCamera = GameObject.FindGameObjectWithTag("MapSystem").GetComponentInChildren<Camera>();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        mapCamera.transform.position = new Vector3(player.position.x, mapCamera.transform.position.y, player.position.z);

        // ズーム：中は低と高の中間にするため、低と高の中央値をとる
        heightLimit[1] = (heightLimit[HIGH] + heightLimit[LOW]) / 2f;
        currentZoom = ZoomLevel.MIDDLE;

        mapCamera.transform.localPosition = new Vector3(mapCamera.transform.localPosition.x, mapCamera.transform.localPosition.y, heightLimit[(int)currentZoom]);
        mapMakerDisplayer = GameObject.FindGameObjectWithTag("MapSystem").GetComponent<MapMakerDisplayer>();

        // プレイヤーアイコンを作成
        mapMakerDisplayer.UpdatePlayerPosition(player);
        compass = FindObjectOfType<QuestCompass>();
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveMapControll();
        ReceiveMapInput();
    }

    /// <summary>
    /// マップオブジェクトを事前設定したローカル位置に移動
    /// </summary>
    public void SetPosition() {
        transform.localPosition = initPosition;
        transform.localEulerAngles = initRotation;
    }

    /// <summary>
    /// マップの視点を上下左右に移動する
    /// </summary>
    private void ReceiveMapControll() {
        mapCamera.transform.localPosition += (horizontalMove.GetAxis() * 15f * mapCamera.transform.right * Time.deltaTime) + (verticalMove.GetAxis() * -15f * mapCamera.transform.forward * Time.deltaTime);
    }

    /// <summary>
    /// マーカーの設置とズーム操作を受け付ける
    /// </summary>
    private void ReceiveMapInput() {
        if (zoomInput.GetButtonDown() && currentZoom != ZoomLevel.LOW) {
            currentZoom--;
            mapCamera.transform.localPosition = new Vector3(mapCamera.transform.localPosition.x, mapCamera.transform.localPosition.y, heightLimit[(int)currentZoom]);
        }
        if (unzoomInput.GetButtonDown() && currentZoom != ZoomLevel.HIGH) {
            currentZoom++;
            mapCamera.transform.localPosition = new Vector3(mapCamera.transform.localPosition.x, mapCamera.transform.localPosition.y, heightLimit[(int)currentZoom]);
        }
        PutMakerIcon();
    }

    /// <summary>
    /// マーカーを置く
    /// </summary>
    private void PutMakerIcon() {
        if (makerPut.GetButtonDown()) {
            RaycastHit hit;
            // 画面の中心にRayを飛ばす
            Ray makerRay = mapCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            if (Physics.Raycast(makerRay, out hit)) {
                if (!mapMakerDisplayer.IsDupMaker()) {
                    mapMakerDisplayer.AddMaker(hit.point);

                    // QuestCompassに反映する（MakerPointがアタッチされたオブジェクトを作成する）
                    GameObject maker = new GameObject("CompassMaker");
                    maker.transform.position = hit.point;
                    MakerPoint makerPoint = maker.AddComponent<MakerPoint>();
                    makerPoint.makerName = PUT_MAKER;
                    makerPoint.icon = mapMakerDisplayer.MakerIcon;
                    compass.AddMaker(makerPoint);
                } else {
                    compass.DestroyMakerBy(PUT_MAKER);
                }
            }
        }
    }

    private void OnDestroy() {
        mapMakerDisplayer.DestroyPlayerIcon();
    }
}
