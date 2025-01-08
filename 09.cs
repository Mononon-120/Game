using GameCanvas;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public sealed class Game : GameBase {
    const int BOX_NUM = 10;
    int[] box_x = new int[BOX_NUM];
    int[] box_y = new int[BOX_NUM];
    int[] box_speed = new int[BOX_NUM];
    int box_w = 24;
    int box_h = 24;
    int player_x = 304;
    int player_y = 400;
    int player_dir = 1;
    int player_speed = 3;
    int active_box_num = 0;
    int gameState = 0;
    int score = 0;
    int count = 0;
    string pname = "t23040ta";
    string url = "";
    string str = "";
    int camera_id=0;
    int camera_num;
    int high_score = 0;
    string camera_name= "";
    GcCameraDevice? m_Camera;
    public override void InitGame() {
        gc.SetResolution(640,480);
        ResetValues();
        if (gc.HasUserAuthorizedPermissionCamera) {
            PlayCamera(0);
        } else {
            gc.RequestUserAuthorizedPermissionCameraAsync(success =>
            {
                if (success) {
                    PlayCamera(0);
                } else {
                    camera_name = "Error: no permission";
                }
            });
        }
        gc.TryLoad("hs",out high_score);
    }
    void ResetValues() {
        score = 0;
        count = 0;
        box_w = 24;
        box_h = 24;
        for (int i = 0; i < BOX_NUM; i++) {
            box_x[i] = gc.Random(0, 616);
            box_y[i] = -gc.Random(100, 480);
            box_speed[i] = gc.Random(3, 6);
        }
        player_x = 304;
        player_y = 400;
        player_dir = 1;
    }
    public override void UpdateGame() {
        if (gameState == 0) {
            if (gc.GetPointerFrameCount(0) == 1) {
                gameState = 1;
            }
        }
        else if (gameState == 1) {
            count++;
            score = count / 60;
            if(score>high_score){
                high_score = score;
                gc.PlaySound(GcSound.Click1);
            }
            box_w = 24 + count / 300;
            box_h = 24 + count / 300;
            active_box_num = 5 + count/600;
            if(active_box_num > BOX_NUM){
                active_box_num = BOX_NUM;
            }
            if (gc.GetPointerFrameCount(0) == 1) {
                player_dir = -player_dir;
            }
            player_x += player_dir * player_speed;
            if (player_x < 0 || player_x > 608) {
                gameState = 2;
                gc.Save("hs",high_score);
                gc.PlaySound(GcSound.Click2);
            }
            for (int i = 0; i < active_box_num; i++) {
                box_y[i] += box_speed[i];
                if (box_y[i] > 480) {
                    box_x[i] = gc.Random(0, 616);
                    box_y[i] = -gc.Random(100, 480);
                    box_speed[i] = gc.Random(3, 6);
                }
                if (gc.CheckHitRect(player_x, player_y, 32, 32, box_x[i], box_y[i], box_w, box_h)) {
                    gameState = 2;
                }
            }
        }
        else if (gameState == 2) {
            if (gc.GetPointerFrameCount(0) == 1) {
                url = "https://web.sfc.keio.ac.jp/~wadari/sdp/k07_web/score.cgi?score=" + score + "&name=" + pname;
                gc.GetOnlineTextAsync(url, out str);
                ResetValues();
                gameState = 0;
            }
        }
    }
    public override void DrawGame() {
        gc.ClearScreen();
        if (gameState == 0) {
            gc.SetColor(0, 0, 0);
            gc.SetFontSize(36);
            gc.DrawString("TITLE", 320, 240);
        } else if (gameState == 1) {
            gc.SetColor(0, 0, 0);
            gc.DrawCameraImage(m_Camera,player_x,player_y,0.1f,0.1f,0f,true);
            //gc.FillRect(player_x, player_y, 32, 32);
            gc.SetColor(255, 0, 0);
            for (int i = 0; i < active_box_num; i++) {
                gc.FillRect(box_x[i], box_y[i], box_w, box_h);
            }
            gc.SetColor(0, 0, 0);
            gc.DrawString("SCORE: " + score, 0, 0);
            gc.DrawString("HIGH:"+high_score,0,60);
        } else if (gameState == 2) {
            gc.SetColor(0, 0, 0);
            gc.SetFontSize(36);
            gc.DrawString("GAME OVER", 320, 240);
            gc.DrawString("SCORE: " + score, 320, 280);
            gc.DrawString("HIGH:"+high_score,0,60);
            gc.DrawString(str, 0, 320);
        }
    }
    void PlayCamera(int id) {
        if(id>= gc.UpdateCameraDevice()){
            id = 0;
        }
        if (gc.TryGetCameraImageAll(out var devices)) {
            m_Camera = devices[id];
            camera_name = m_Camera.DeviceName;
        } else {
            camera_name = "Warn: no camera";
        }
    }
}

