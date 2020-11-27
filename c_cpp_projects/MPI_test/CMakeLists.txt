cmake_minimum_required(VERSION 3.15)
project(vaja03)

set(CMAKE_CXX_STANDARD 17)
#set(CMAKE_CXX_COMPILER "/usr/bin/mpicxx")
#set(CMAKE_CXX_COMPILER "/usr/bin/mpicc")

find_package(MPI REQUIRED)
include_directories(SYSTEM ${MPI_INCLUDE_PATH})

add_executable(vaja03 main.cpp)

target_link_libraries(vaja03 ${MPI_C_LIBRARIES})