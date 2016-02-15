//
//  main.cpp
//  SDL2_test_01
//
//  Created by hujita on 2016/02/15.
//  Copyright (c) 2016年 hujita. All rights reserved.
//

#include <iostream>
#include <SDL.h>

int main(int, char ** const)
{
    // SDL_Initは、全てのSDL APIに先駆けて呼び出す必要のある関数。ここではビデオ機能のみ初期化。
    SDL_Init(SDL_INIT_VIDEO);
    // 位置,大きさ,フラグを指定してアプリケーションウィンドウを生成する
    // 引数は順に、ウィンドウのタイトル、X座標、Y座標、幅、高さ
    SDL_Window* window = SDL_CreateWindow("Hey",SDL_WINDOWPOS_CENTERED,SDL_WINDOWPOS_CENTERED,640,480,0);
    // 描画処理。引数は順にレンダリングを表示するウィンドウ、...
    SDL_Renderer* render = SDL_CreateRenderer(window, -1, 0);
    
    // SDLでイベントはSDL_EVENTという共用体(構造体のようなもの)を使って扱える。
    // これを型にしてeventという変数を用意。
    SDL_Event event;
    while(true){
        // 下記の二行がないと縞縞模様の砂嵐のようになる。0,0,0,255で黒で塗りつぶして初期化している？
        // 描画操作で使う色を設定する。引数は順にレンダリングコンテキスト？、赤、緑、青、α(不透明度？)。
        SDL_SetRenderDrawColor(render, 0, 0, 0, 255);
        // SDL_Renderer構造体の内容を選択した色で消去
        SDL_RenderClear(render);
        // 未処理のイベントをキューから得る
        // 引数はキューから得たイベントを代入するSDL_Event
        // 未処理のイベントが得られたら1を、無ければ0を返す。
        while(SDL_PollEvent(&event))
        {
            // SDL_QUIT: ウインドウを閉じるときのイベント(ウインドウの「×ボタン」を押したときのイベント)
            // SDLK_ESCAPE: ESC キーが押されたら終了する
            // キーが押されたときのコードはSDL_keycide.hに書いてある。
            if (event.type == SDL_QUIT || (event.type == SDL_KEYUP && event.key.keysym.sym == SDLK_ESCAPE))
                return 0;
                // return 0;の代わりにSDL_Quitでも良さそう。
                // SDL_Quitは、終了時に最後に呼ぶ関数。今回は必要ないので、参考例として書くけどコメントアウトしておく。
                // SDL_Quit();
        }
        // 描画操作で使う色を設定、ここでは赤。
        SDL_SetRenderDrawColor(render, 255, 0, 0, 255);
        // レンダリングの対象に直線を引く。
        // 引数は順にレンダリングコンテキスト、始点のx座標、始点のy座標、終点のx座標、終点のy座標。
        SDL_RenderDrawLine(render,10, 10, 400, 400);
        // レンダリングの結果を画面に反映する。
        SDL_RenderPresent(render);
    }
    
    return 0;
}

