//
//  main.cpp
//  boost_test_01
//
//  Created by hujita on 2016/02/15.
//  Copyright (c) 2016年 hujita. All rights reserved.
//

#include <iostream>
#include <boost/foreach.hpp>
// 動的配列を生成する標準ライブラリ
#include <vector>
// boost::sortが使えるようになる
// std::sortでは(v.begin(), v.end())のような引数が必要で、それに比べより直感的に書ける。
#include <boost/range/algorithm.hpp>

using namespace std;

// ここで引数を参照渡しにして遠隔操作できるようにする必要ある？ 「&」消して値渡しにしても動く。
void dump(vector<int> v)
{
    // 第一引数がループ変数で、第二引数に対象の配列など。対象の要素を順番に処理していく。
    BOOST_FOREACH(int x, v) {
        cout << x << endl;
    }
}

int main (int argc, char *argv[])
{
    // 空の動的配列を生成する。(std::は省略)
    vector<int> v;
    // 末尾に追加
    v.push_back ( 3 );
    v.push_back ( 4 );
    v.push_back ( 1 );
    v.push_back ( 2 );
    
    cout << "Before sort" << endl;
    dump (v);
    // 昇順でソートする
    boost::sort(v);
    
    cout << "After sort" << endl;
    dump (v);
    return 0;
}