using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// マップ用カメラの映像を2Dに反映する機能を実装する
/// </summary>
public class MapMakerDisplayer : MonoBehaviour
{
    /// <summary>
    /// マップ用カメラ
    /// </summary>
    [SerializeField] private Camera mapCamera;
    /// <summary>
    /// 現在設置されているマーカーのリスト
    /// </summary>
    private List<Maker> makers = new();
    /// <summary>
    /// マップ情報表示用Canvaa
    /// </summary>
    [SerializeField] private RectTransform mapUI;
    /// <summary>
    /// 画面中央に移すマーカーImage
    /// </summary>
    [SerializeField] private RectTransform targetIcon;
    /// <summary>
    /// マーカープレハブ
    /// </summary>
    [SerializeField] private Image makerIcon;
    /// <summary>
    /// プレイヤーマーカープレハブ
    /// </summary>
    [SerializeField] private Image playerIcon;
    private Vector3 playerPos;
    /// <summary>
    /// 現在表示されているプレイヤーアイコン
    /// </summary>
    private RectTransform _playerIcon;
    public Sprite MakerIcon { get { return makerIcon.sprite; } }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FetchPlayerIcon();
        if (makers.Count <= 0)
            return;

        foreach (Maker maker in makers) {
            DrawMaker(maker.worldPos, maker.instance);
        }
    }

    /// <summary>
    /// マーカーをキャンバスに描画する
    /// </summary>
    /// <param name="worldPos">マーカーのワールド座標</param>
    /// <param name="instancedMaker">生成されたマーカーオブジェクト</param>
    private void DrawMaker(Vector3 worldPos, RectTransform instancedMaker) {
        // マーカーの設置された座標を画面座標に変換
        Vector3 screenPos = mapCamera.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapUI, screenPos, mapCamera, out var localpos);

        // Canvas上に生成されたマーカーオブジェクトの位置を移動
        instancedMaker.localPosition = localpos;
    }

    /// <summary>
    /// 既にマーカーが設置済みか確認する
    /// </summary>
    /// <returns>マーカーが設置済みか</returns>
    public bool IsDupMaker() {
        if (makers.Count <= 0)
            return false;
        foreach (Maker maker in makers) {
            // targetIconのRect範囲とマーカーのRectがかぶる場合、マーカーを削除
            if (targetIcon.rect.Overlaps(maker.instance.rect)) {
                Destroy(maker.instance.gameObject);
                makers.Remove(maker);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// プレイヤーアイコンをプレイヤー座標と合わせる
    /// </summary>
    public void FetchPlayerIcon() {
        if (!_playerIcon)
            return;

        // プレイヤー座標をCanvas上の座標に変換
        Vector3 screenPosition = mapCamera.WorldToScreenPoint(playerPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapUI, screenPosition, mapCamera, out var localPos);
        _playerIcon.localPosition = localPos;
    }

    /// <summary>
    /// マーカーをマップ上に表示する
    /// </summary>
    /// <param name="makerposition">マーカーが置かれたワールド座標</param>
    public void AddMaker(Vector3 makerposition) {
        Image _maker = Instantiate(makerIcon, mapUI);
        makers.Add(new Maker(makerposition, _maker.GetComponent<RectTransform>()));
    }

    /// <summary>
    /// プレイヤーアイコンの生成を行う
    /// </summary>
    /// <param name="player">現在のプレイヤー座標</param>
    public void UpdatePlayerPosition(Transform player) {
        _playerIcon = Instantiate(playerIcon.rectTransform, mapUI);
        _playerIcon.localEulerAngles = new Vector3(0f, 0f, -player.localEulerAngles.y);
        playerPos = player.position;
    }

    /// <summary>
    /// プレイヤーアイコンを削除する
    /// </summary>
    public void DestroyPlayerIcon() {
        if (_playerIcon) {
            Destroy(_playerIcon.gameObject);
            _playerIcon = null;
        }
    }
}

/// <summary>
/// マップ上に設置されたマーカー情報を保持する
/// </summary>
public class Maker {
    /// <summary>
    /// マーカーが設置された座標
    /// </summary>
    public Vector3 worldPos;
    /// <summary>
    /// Canvas上に生成されたマーカーImage
    /// </summary>
    public RectTransform instance;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="worldPos">マーカーが設置された座標</param>
    /// <param name="instance">Canvas上に生成されたマーカーImage</param>
    public Maker(Vector3 worldPos, RectTransform instance) {
        this.worldPos = worldPos;
        this.instance = instance;
    }
}