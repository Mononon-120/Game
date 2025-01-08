#nullable enable
using GameCanvas;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// ゲームクラス。
/// 学生が編集すべきソースコードです。
/// </summary>
public sealed class Game : GameBase {
    float lat;
    float lng;
    string text;
    bool isStartGPS = false;
    int gameState = 0;
    float base_lat=0,base_lng=0;
    float player_lat=0,player_lng=0;
    const int CHECK_NUM = 9;
    int [] check_dx = new int[CHECK_NUM] ;
    int [] check_dy = new int[CHECK_NUM] ;
    bool [] isCheck = new bool[CHECK_NUM];
    bool isComplete;
    float calcRate = 0.001f;
    int playcount ;
    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void InitGame() {
        gc.SetResolution(720, 1280);
        lat = 35.685410f;
        lng = 139.752842f;
        text = "取得中";
        resetValue();
    }
    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame() {
        if(!isStartGPS){
            gc.StartGeolocationService();
            isStartGPS = true;
        }
        if(!gc.HasGeolocationPermission){
            text = "位置情報サービスが無効です";
        }
        if(gc.HasGeolocationUpdate){
            lat = gc.GeolocationLastLatitude;
            lng = gc.GeolocationLastLongitude;
            text = string.Format("緯度: {0}\n経度: {1}", lat, lng);
        }
        if(gameState == 0){
            if(gc.GetPointerFrameCount(0)==1){
                gameState = 1;
                base_lat = lat;
                base_lng = lng;
                resetValue();
            }
        } else if(gameState == 1){ //ゲーム中の処理
            playcount++;
            player_lat = lat - base_lat;
            player_lng = lng - base_lng; //今いる場所をtrueに
            for (int i = 0; i < CHECK_NUM; i++) {
                float check_lat = check_dx[i] * calcRate;
                float check_lng = check_dy[i] * calcRate;
                if( player_lat - check_lat > -calcRate/2 && player_lat - check_lat <  calcRate/2 && player_lng - check_lng > -calcRate/2 && player_lng - check_lng <  calcRate/2){
                    isCheck [i] = true;
                }
            } //全部通ったかの判定
            isComplete = true;
            for (int i = 0; i < CHECK_NUM; i++) {
                if (!isCheck [i]) {
                    isComplete = false;
                }
            }
            if (isComplete) {
                gameState = 2;
            }
        } else if(gameState == 2){
            //ゲームクリアー時の処理
            if(gc.GetPointerFrameCount(0)==1){
                resetValue();
                gameState = 0;
            }
        }
    }
    /// <summary>
    /// 描画の処理
    /// </summary>
    public override void DrawGame() {
        gc.ClearScreen();
        gc.SetColor(0,0,0);
        gc.DrawString(text, 0, 0); // 画面を白で塗りつぶします
        if(gameState == 0){ //タイトル画面の処理
            gc.DrawString("TAP TO START！",320, 60);
        } else if(gameState == 1){ //ゲーム中の処理
            gc.DrawString("PLAYING",320, 60);
            gc.DrawString ("SCORE"+playcount,320, 90);
            gc.DrawString ("lat:" + player_lat/calcRate,320, 120);
            gc.DrawString ("lng:" + player_lng/calcRate,320, 150);
            for( int i = 0;i < CHECK_NUM; i++){
                if(isCheck[i]){
                    gc.DrawString("o",400+check_dx[i]*30,250+check_dy[i]*30 );
                } else {
                    gc.DrawString("x",400+check_dx[i]*30,250+check_dy[i]*30 );
                }
            }
        } else if(gameState == 2){ //ゲームクリアー時の処理
            gc.DrawString("CLEAR!!",320, 60 );
            gc.DrawString ("SCORE:"+playcount,320, 90);
        }
    }
    public void resetValue(){
        for(int i=0;i < CHECK_NUM;i++){
            isCheck[i]= false;
            check_dx[i] = (i%3) -1;
            check_dy[i] = (i/3) -1;
        }
        isComplete = false;
        playcount = 0;
    }
}
