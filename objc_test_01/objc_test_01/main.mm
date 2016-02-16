//
//  main.m
//  objc_test_01
//
//  Created by hujita on 2016/02/16.
//  Copyright (c) 2016年 hujita. All rights reserved.
//
#include <iostream>
#import <Cocoa/Cocoa.h>

int main(int argc, const char * argv[]) {
    NSString* str = [[NSBundle mainBundle] resourcePath];
    NSLog(str);
    return NSApplicationMain(argc, argv);
}
