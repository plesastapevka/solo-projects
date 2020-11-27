cmake_minimum_required(VERSION 3.10)
project(DMA C)

set(CMAKE_C_STANDARD 11)

include_directories(.)

add_executable(DMA
        main.c
        mymalloc.c
        mymalloc.h)
