//
//  main.m
//  SDL2_test_03
//
//  Created by hujita on 2016/02/15.
//  Copyright (c) 2016年 hujita. All rights reserved.
//

#import <Cocoa/Cocoa.h>

int main(int argc, const char * argv[]) {
    NSString* str = [[NSBundle mainBundle] resourcePath];
    NSLog(str);
    return NSApplicationMain(argc, argv);
}
