cmake_minimum_required(VERSION 3.15)
project(MPI_battleship)

set(CMAKE_CXX_STANDARD 20)

find_package(MPI REQUIRED)
include_directories(SYSTEM ${MPI_INCLUDE_PATH})

add_executable(battleship main.cpp)

target_link_libraries(battleship ${MPI_C_LIBRARIES})