cmake_minimum_required(VERSION 3.15)
project(MPI_test)

set(CMAKE_CXX_STANDARD 17)
#set(CMAKE_CXX_COMPILER "/usr/bin/mpicxx")
#set(CMAKE_CXX_COMPILER "/usr/bin/mpicc")

find_package(MPI REQUIRED)
include_directories(SYSTEM ${MPI_INCLUDE_PATH})

add_executable(MPI_test main.cpp)

target_link_libraries(MPI_test ${MPI_C_LIBRARIES})