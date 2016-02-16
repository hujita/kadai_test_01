//
//  main.m
//  objc_test_01
//
//  Created by hujita on 2016/02/16.
//  Copyright (c) 2016年 hujita. All rights reserved.
//

/* Cocoa.hを読み込むことによってFoundation.hとAppKit.hというCocoaフレームワークで良く使われる多くのクラスのヘッダファイルをまとめて読み込む */
#import <Cocoa/Cocoa.h>
#include <SDL.h>
#include <SDL_image.h>
/* iostreamよりstdio.hのほうが早い？ fight_project/clientでは両方使っているっぽい？ */
#include <stdio.h>

SDL_Window* window = NULL;                  /* 画面 */
SDL_Surface* screen = NULL;                 /* 描画領域 */
SDL_Surface* background_image = NULL;       /* 背景画像 */


/* 更新する */
void Update(void)
{
    /* TODO: ここにゲームの状態を更新する処理を書く */
}


/* 描画する */
void Draw(void)
{
    /* 背景を描画する */
    /* SDL_BlitSurface(コピー元のサーフェイス, コピー元のSDL_Rect。NULLのときサーフェイス全体, コピー先のサーフェイス, コピー先のSDL_Rect)
     * サーフェイスを別のサーフェイスに高速にコピーする
     * 成功なら0,失敗のとき負の数のエラーコードを戻す
     * screen(windowのサーフェイス)にbackground_image(画像を読み込んだサーフェイス)をコピーする */
    SDL_BlitSurface(background_image, NULL, screen, NULL);
    
    /* TODO: ここに描画処理を書く */
    
    /* 画面を更新する */
    /* SDL_UpdateWindowSurface(更新するウィンドウ)
     *  ウィンドウサーフェイスを画面にコピーする */
    SDL_UpdateWindowSurface(window);
}


/* 初期化する。
 * 成功したときは0を、失敗したときは-1を返す。
 */
int Initialize(void)
{
    /* SDLを初期化する */
    /* SDL_Init()
     * SDLを使うためには、まずSDL_Init()を呼んでSDLライブラリを正常に初期化し、要求に応じて個別のサブシステム(ビデオ、オーディオetc)を開始する必要がある。
     * 参考までに3つのサブシステムを初期化しているけど、別にどれも使っていないので、引数は0でも動く。
     * ファイルI/O, そしてスレッドサブシステムはデフォルトで初期化される. 他のサブシステムを使うときは指定して初期化しなければならない。
     * 他のあらゆるSDLの関数を呼び出す前にこの関数を呼ぶ必要がある
     * 成功のとき0,エラーのとき負の数のエラーコードを戻す */
    /* SDL_GetError()
     * 最後に発生したエラーのメッセージを得る */
    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO | SDL_INIT_TIMER) < 0) {
        fprintf(stderr, "SDLの初期化に失敗しました：%s\n", SDL_GetError());
        return -1;
    }
    
    /* 画面を初期化する */
    /* SDL_CreateWindow()
     * 位置, 大きさ, フラグを指定してウィンドウを生成する
     * 今回はウィンドウのタイトルを"Hey"にして、ウィンドウを中央に表示して、ウィンドウのサイズは背景画像と同じ640x480、最後のフラグは0。
     * フラグを変更すれば画面サイズを大きくしたり見えないようにしたりウィンドウの状態を変更できる。
     * フラグはSDL_WindowFlagsというウィンドウの状態の列挙体の中から指定する
     * 失敗の時はNULLを返す */
    window = SDL_CreateWindow("Hey",SDL_WINDOWPOS_CENTERED,SDL_WINDOWPOS_CENTERED,640,480,0);

    /* 画面に描画できるようにする */
    /* SDL_GetWindowSurface(調査するウィンドウ)
     * ウィンドウのSDLサーフェイスを得る 
     * サーフェイスとは描画領域の単位？のようなもの
     * 失敗の時はNULLを返す */
    screen = SDL_GetWindowSurface(window);
    if (screen == NULL) {
        fprintf(stderr, "画面の初期化に失敗しました：%s\n", SDL_GetError());
        /* SDL_Quit: サブシステムを終了する。いかなる場合でもこの関数を呼ばなければならない */
        SDL_Quit();
        return -1;
    }

    /* 画面のタイトルを変更する */
    SDL_SetWindowTitle(window, "My SDL Sample Game");
    
    /* TODO: ここで画像や音声を読み込む */
    /* SDL_Surface *IMG_Load(cサーフェイスに読み込む画像ファイルの名前)
     * イベントループの外で呼び, 読み込んだ画像は使わなくなるまで保持するのが望ましい
     * ディスクの読み込みと画像のサーフェイスへの展開はそれほど速くないため
     * 必要がなくなったとき, この関数が戻したサーフェイスのポインタに対してSDL_FreeSurfaceを呼ぶのを忘れてはならない */
    background_image = IMG_Load("background.png");
    if (background_image == NULL) {
        fprintf(stderr, "画像の読み込みに失敗しました：%s\n", SDL_GetError());
        SDL_Quit();
        return -1;
    }
    
    /* 無事に初期化が終了してここまで辿り着いたなら、0を返す */
    return 0;
}


/* メインループ */
void MainLoop(void)
{
    /* SDL_EventはSDL_Windowと違って、どうしてポインタの変数じゃない？ */
    /* SDL_Eventには二つの使い方がる
     * ①キューからイベントを読み込む
     * ②キューにイベントを書き込む
     * ①についてはSDL_PollEvent()で読み込める
     * まず初めに空のSDL_Eventを生成する */
    SDL_Event event;
    /* SDL_GetTicks(void);
     * SDLが初期化されてから経過した時間をミリ秒で得る */
    double next_frame = SDL_GetTicks();
    /* 1000.0は1秒 */
    double wait = 1000.0 / 60;
    /* while(true) より for(;;) の方が良い？
     * VC++2005 だと while ( true ) で警告がでるから？
     * while(true) だと無駄なチェックが入るから？ #=> 最近のコンパイラは頭が良いから影響ないらしい
     */
    for (;;) {
        /* すべてのイベントを処理する */
        /* SDL_PollEvent
         * イベントを読み込み、キューから削除する。もしキューにイベントがなければ0を、あれば1を戻す
         * whileループを使い全てのイベントを処理する 
         * 引き数のSDL_Eventへのポインタにイベント情報を代入する
         * eventのtypeメンバにはイベントの種類も書き込まれているのでswitch分で種類に応じてイベントを処理できる */
        while (SDL_PollEvent(&event)) {
            /* QUIT イベントが発生するか、ESC キーが押されたら(指が離れたら)終了する */
            if ((event.type == SDL_QUIT) ||
                (event.type == SDL_KEYUP && event.key.keysym.sym == SDLK_ESCAPE))
                /* 戻り値なしでこの関数から脱出 */
                return;
        }
        /* 1秒間に60回Updateされるようにする(ゲームの一般的な更新回数)
         * CPUは基本的に全力で命令を処理するのでCPUの使用率が100%になってしまう。たとえそれが1+1=2のような簡単な計算であっても 
         * なのでCPUに時間を制御させて画面の更新回数を調節して処理が必要がない場合はCPUを休ませる必要がある */
        /* next_frame #=> SDLが初期化された最初の時間 + 更新のたびに約16ミリ秒
         * SDL_GetTicks #=> 現在進行形の経過時間
         * wait #=> 約16ミリ秒。1回更新する時間 */
        /* Update();
         * Draw();
         * 下記のように書かずに、なぜか普通にこう書いたほうがCPU使用率が半減する
         * 下記の処理がないときはCPU使用率50%なのに、
         * 下記の処理をいれると逆にCPU使用率が99％になってしまう */
        if (SDL_GetTicks() >= next_frame) {
            Update();
            
            /* 時間がまだあるときはDrawする */
            /* どうしてUpdate()は1秒間に60回実行するのに、
             * DrawはUpdate()のあとでまだ時間が余ってたら実行みたいに、
             * ちょっと重要度が下がってる扱い？ */
            if (SDL_GetTicks() < next_frame + wait)
                Draw();
            
            next_frame += wait;
            /* SDL_Delay()
             * 指定のミリ秒の間待つ */
            SDL_Delay(0);
        }
    }
}


/* 終了処理を行う */
void Finalize(void)
{
    /* TODO: ここで画像や音声を解放する */
    /* ここではIMG_Loadで読み込んだ画像を解放している */
    /* SDL_FleeSurface(解放するサーフェイス) */
    SDL_FreeSurface(background_image);
    
    /* 終了する */
    SDL_Quit();
}


/* メイン関数 */
int main(int argc, char* argv[])
{
    /* 初期化に失敗したらOSに-1を返す */
    if (Initialize() < 0)
        return -1;
    
    MainLoop();
    /* メモリ解放などの終了処理 */
    Finalize();
    /* 処理が無事に成功したならOSに0を返す */
    return 0;
}