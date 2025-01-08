#nullable enable
using GameCanvas;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public sealed class Game : GameBase {
    int sec = 0;
    int pointer_start_x;
    int pointer_start_y;
    int pointer_dx;
    int pointer_dy;
    int old_pointer_count = 0;
    int swipe_dir=0;
    int swipe_dir_last = 0;
    const int NO_DIR = 0;
    const int DIR_UP = 1;
    const int DIR_DOWN = 2;
    const int DIR_RIGHT = 3;
    const int DIR_LEFT = 4;
    const int SWIPE_DIST = 30;
    string[] dir_name = {"NO DIR","UP","DOWN","RIGHT","LEFT"};
    int GameStateSub = 0;
    int count = 0;
    int round = 0;
    int target_dir=0;
    int score = 0;
    public override void InitGame() {
        gc.SetResolution(720,1280);
    }
    public override void UpdateGame() {
        sec = (int)gc.TimeSinceStartup;
        CalcSwipe();
        if(GameStateSub == 0){
            count++;
            if(count == 180){
                GameStateSub = 1;
                count =0;
                target_dir = gc.Random(1,4);
            }
        } else if(GameStateSub == 1){
            count++;
            if(swipe_dir!=NO_DIR){
                if(swipe_dir == target_dir){
                    GameStateSub = 2;
                    score += 300-count;
                    count = 0;
                } else {
                    GameStateSub = 3;
                    score -= 500;
                    count = 0;
                }
            }
        } else if(GameStateSub == 2 || GameStateSub ==3){
            count++;
            if(count > 180){
                round++;
                if(round >=10){
                    GameStateSub = 4;
                    count = 0;
                } else {
                    GameStateSub=0;
                    count = 0;
                }
            }
        }
    }
    public override void DrawGame() {
        gc.ClearScreen();
        gc.SetColor(0, 0, 0);
        gc.SetFontSize(36);
        gc.DrawString("DIR:"+swipe_dir,0,0);
        gc.DrawString("LAST:"+swipe_dir_last,0,40);
        gc.DrawString(dir_name[swipe_dir_last],0,80);
        gc.DrawString("STATESUB:"+GameStateSub,0,160);
        gc.DrawString("COUNT:"+count,0,200);
        gc.DrawString("ROUND:"+round,0,240);
        gc.DrawString("TARGET:"+target_dir,0,280);
        gc.DrawString("SCORE:"+score,0,320);
        gc.SetFontSize(48);
        if(GameStateSub==0){
            gc.DrawString("ROUND:"+(round+1),320,600);
            gc.DrawString("READY!",320,650);
        } else if(GameStateSub==1){
           gc.DrawString(dir_name[target_dir],360,640);
        } else if(GameStateSub==2){
           gc.DrawString("SUCCESS",360,640);
        } else if(GameStateSub==3){
           gc.DrawString("FAIL",360,640);
        } else if(GameStateSub==4){
            gc.DrawString("FINISHED!",360,600);
            gc.DrawString("SCORE:"+score,360,650);
        }
    }
    void CalcSwipe(){
        if(gc.GetPointerFrameCount(0)==1){
            pointer_start_x = (int)gc.GetPointerX(0);
            pointer_start_y = (int)gc.GetPointerY(0);
        }
        if(gc.GetPointerFrameCount(0)>0){
            pointer_dx = (int)gc.GetPointerX(0)-pointer_start_x;
            pointer_dy = (int)gc.GetPointerY(0)-pointer_start_y;
        }
        swipe_dir = NO_DIR;
        if(gc.PointerCount == 0 && old_pointer_count ==1 ){
            if(pointer_dx * pointer_dx > pointer_dy * pointer_dy){
                if(pointer_dx > SWIPE_DIST){
                    swipe_dir = DIR_RIGHT;
                } else if(pointer_dx < -SWIPE_DIST){
                    swipe_dir = DIR_LEFT;
                }
            } else {
                if(pointer_dy > SWIPE_DIST){
                    swipe_dir = DIR_DOWN;
                } else if(pointer_dy < -SWIPE_DIST){
                    swipe_dir = DIR_UP;
                }
            }
        }
        old_pointer_count = gc.PointerCount;
        if(swipe_dir != NO_DIR){
            swipe_dir_last = swipe_dir;
        }
}

}
