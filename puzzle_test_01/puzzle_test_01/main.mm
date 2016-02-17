//
//  main.m
//  puzzle_test_01
//
//  Created by hujita on 2016/02/17.
//  Copyright (c) 2016年 hujita. All rights reserved.
//

//
//  main.m
//  key_test_01
//
//  Created by hujita on 2016/02/17.
//  Copyright (c) 2016年 hujita. All rights reserved.
//

#include <SDL.h>
#include <SDL_image.h>
#include <SDL_ttf.h>
#include <stdio.h>


/* 入力データ */
typedef struct {
    int left;
    int up;
    int right;
    int down;
    int button1;
    int button2;
} InputData;


InputData current_input, prev_input;        /* 入力データ */

SDL_Window* window = NULL;                  /* 画面 */
SDL_Surface* screen = NULL;                 /* 画面の描画領域 */
SDL_Surface* background_image = NULL;       /* 背景画像 */
SDL_Surface* puzzle_block = NULL;           /* ブロック */

SDL_Surface* word_main = NULL;              /* メインテキスト */
SDL_Surface* word_sub = NULL;               /* サブテキスト */
SDL_Surface* word_0 = NULL;                 /* テキスト */
SDL_Surface* word_1 = NULL;                 /* テキスト */
SDL_Surface* word_2 = NULL;                 /* テキスト */
SDL_Surface* word_3 = NULL;                 /* テキスト */
SDL_Surface* word_4 = NULL;                 /* テキスト */

TTF_Font* font;                             /* フォント */
SDL_Color white = {0xff, 0xff, 0xff};       /* 色 */
SDL_Rect destrect_main_word = { 330, 210 }; /* コメント位置 */
SDL_Rect destrect_sub_word = { 330, 310 };  /* サブコメント位置 */

int view_type = 0;                          /* 0:TOP, 1:Play */
int config_phase = 0;                       /* 設定の段階 */
int line = 0;                               /* 行数 */
int row = 0;                                /* 列数 */
int type = 0;                               /* ブロック種類数 */
int chain = 0;                                /* 必要連鎖数 */
double x, y;                                /* 音符の座標 */
const int TOP_VIEW = 0;
const int PLAY_VIEW = 1;

/* 入力データを更新する */
void UpdateInput(void)
{
    const Uint8* keys = SDL_GetKeyboardState(NULL);
    
    prev_input = current_input;
    current_input.left = keys[SDL_SCANCODE_LEFT] | keys[SDL_SCANCODE_H];        /* [←], [H] */
    current_input.up = keys[SDL_SCANCODE_UP] | keys[SDL_SCANCODE_K];            /* [↑], [K] */
    current_input.right = keys[SDL_SCANCODE_RIGHT] | keys[SDL_SCANCODE_L];      /* [→], [L] */
    current_input.down = keys[SDL_SCANCODE_DOWN] | keys[SDL_SCANCODE_J];        /* [↓], [J] */
    current_input.button1 = keys[SDL_SCANCODE_LSHIFT] | keys[SDL_SCANCODE_Z];   /* 左Shift, [Z] */
    current_input.button2 = keys[SDL_SCANCODE_LCTRL] | keys[SDL_SCANCODE_X];    /* 左Ctrl, [X] */
}


/* 更新する */
void Update(void)
{
    int speed = 6;
    
    UpdateInput();
    
    /* ボタン1が押されているときは低速 */
    if (current_input.button1)
        speed = 2;
    /* ボタン2が押されたら(0, 0)に移動 */
    if (current_input.button2 && !prev_input.button2)   /* 押された瞬間のみ */
        x = y = 0;
    
    /* 入力にしたがって移動する */
    if (current_input.left)
        x -= speed;
    if (current_input.right)
        x += speed;
    if (current_input.up)
        y -= speed;
    if (current_input.down)
        y += speed;
    
    /* 画面外に出ないようにする */
    if (x < 0)
        x = 0;
    else if (x > 640 - 64)
        x = 640 - 64;
    if (y < 0)
        y = 0;
    else if (y > 480 - 64)
        y = 480 - 64;
}

/* TOP画面更新 */
void UpdateTop(void)
{
}

/* 描画する */
void Draw(void)
{

    /* 背景を描画する */
    SDL_BlitSurface(background_image, NULL, screen, NULL);
    /* メインテキスト描画 */
    SDL_BlitSurface(word_main, NULL, screen, &destrect_main_word);
    /* サブテキスト描画 */
    switch (config_phase) {
        case 0:
            word_sub = word_0;
            break;
        case 1:
            word_sub = word_1;
            break;
        case 2:
            word_sub = word_2;
            break;
        case 3:
            word_sub = word_3;
            break;
        case 4:
            word_sub = word_4;
            break;
    }
    SDL_BlitSurface(word_sub, NULL, screen, &destrect_sub_word);
    
    /* ブロックを描画する */
    //SDL_BlitSurface(puzzle_block, NULL, screen, &destrect);
    
    /* 画面を更新する */
    SDL_UpdateWindowSurface(window);
}


/* 初期化する。
 * 成功したときは0を、失敗したときは-1を返す。
 */
int Initialize(void)
{
    if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO | SDL_INIT_TIMER) < 0) {
        fprintf(stderr, "SDLの初期化に失敗しました：%s\n", SDL_GetError());
        return -1;
    }
    if (TTF_Init() < 0) {
        fprintf(stderr, "TTFの初期化に失敗しました：%s\n", TTF_GetError());
        return -1;
    }

    window = SDL_CreateWindow("My Family Puzzle", SDL_WINDOWPOS_CENTERED,SDL_WINDOWPOS_CENTERED,1000,850,0);
    screen = SDL_GetWindowSurface(window);
    if (screen == NULL) {
        fprintf(stderr, "画面の初期化に失敗しました：%s\n", SDL_GetError());
        SDL_Quit();
        return -1;
    }
    
    /* 画像を読み込む */
    background_image = IMG_Load("background.gif");
    if (background_image == NULL) {
        fprintf(stderr, "画像の読み込みに失敗しました：%s\n", SDL_GetError());
        SDL_Quit();
        return -1;
    }
    puzzle_block = IMG_Load("puzzle_block.png");
    if (puzzle_block == NULL) {
        fprintf(stderr, "画像の読み込みに失敗しました：%s\n", SDL_GetError());
        SDL_Quit();
        return -1;
    }
    
    /* フォント読み込み */
    font = TTF_OpenFont("AquaKana.ttc", 24);
    if (font == NULL) {
        fprintf(stderr, "fontの取得に失敗しました：%s\n", SDL_GetError());
        SDL_Quit();
        return -1;
    }
    
    /* 文字作成 */
    word_main = TTF_RenderUTF8_Blended(font, "パズルの設定(1~9で入力)", white);
    word_0 = TTF_RenderUTF8_Blended(font, "行数を指定してください", white);
    word_1 = TTF_RenderUTF8_Blended(font, "列数を指定してください", white);
    word_2 = TTF_RenderUTF8_Blended(font, "ブロックのの種類数", white);
    word_3 = TTF_RenderUTF8_Blended(font, "ブロックを繋げるべき数", white);
    word_4 = TTF_RenderUTF8_Blended(font, "Enter:ゲーム開始 / 右Shift:TOPに戻る / ESC:終了", white);

    return 0;
}

void ConfigTopIn(int n){
    switch (config_phase){
        case 0:
            line = n;
            break;
        case 1:
            row = n;
            break;
        case 2:
            type = n;
            break;
        case 3:
            chain = n;
            break;
    }
}

void ConfigTop(SDL_Event event){
    int value = 0;
    switch (event.key.keysym.sym) {
        case SDLK_1:
            value = 1;
            break;
        case SDLK_2:
            value = 2;
            break;
        case SDLK_3:
            value = 3;
            break;
        case SDLK_4:
            value = 4;
            break;
        case SDLK_5:
            value = 5;
            break;
        case SDLK_6:
            value = 6;
            break;
        case SDLK_7:
            value = 7;
            break;
        case SDLK_8:
            value = 8;
            break;
        case SDLK_9:
            value = 9;
            break;
    }
    ConfigTopIn(value);
    ++config_phase;
}


/* メインループ */
void MainLoop(void)
{
    SDL_Event event;
    double next_frame = SDL_GetTicks();
    double wait = 1000.0 / 60;
    
    for (;;) {
        /* すべてのイベントを処理する */
        while (SDL_PollEvent(&event)) {
            /* QUIT イベントが発生するか、ESC キーが押されたら終了する */
            if ((event.type == SDL_QUIT) ||
                (event.type == SDL_KEYUP && event.key.keysym.sym == SDLK_ESCAPE))
                return;
            if (event.type == SDL_KEYDOWN){
                int value = 0;
                switch (event.key.keysym.sym) {
                    case SDLK_1:
                        value = 1;
                        break;
                    case SDLK_2:
                        value = 2;
                        break;
                    case SDLK_3:
                        value = 3;
                        break;
                    case SDLK_4:
                        value = 4;
                        break;
                    case SDLK_5:
                        value = 5;
                        break;
                    case SDLK_6:
                        value = 6;
                        break;
                    case SDLK_7:
                        value = 7;
                        break;
                    case SDLK_8:
                        value = 8;
                        break;
                    case SDLK_9:
                        value = 9;
                        break;
                }
                ConfigTopIn(value);
                ++config_phase;
                fprintf(stderr, "画面の初期化に失敗しました：%d\n", config_phase);
            }
               // ConfigTop(event);
            if (event.type == SDL_KEYDOWN && event.key.keysym.sym == SDLK_RSHIFT){
                line = 0;
                row = 0;
                type = 0;
                chain = 0;
                config_phase = 0;
                view_type = TOP_VIEW;
            }
        }
        /* 1秒間に60回Updateされるようにする */
        if (SDL_GetTicks() >= next_frame) {
            if (view_type == TOP_VIEW) {
                UpdateTop();
            };
            //Update();
            /* 時間がまだあるときはDrawする */
            if (SDL_GetTicks() < next_frame + wait)
                Draw();
            next_frame += wait;
            SDL_Delay(0);
        }
    }
}


/* 終了処理を行う */
void Finalize(void)
{
    SDL_FreeSurface(background_image);
    SDL_FreeSurface(puzzle_block);
    TTF_CloseFont(font);
    
    /* 終了する */
    TTF_Quit();
    SDL_Quit();
}


/* メイン関数 */
/* 初期化 -> メイン処理 -> 終了処理 */
int main(int argc, char* argv[])
{
    if (Initialize() < 0)
        return -1;
    MainLoop();
    Finalize();
    return 0;
}