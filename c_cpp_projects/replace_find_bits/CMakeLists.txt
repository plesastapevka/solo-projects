cmake_minimum_required(VERSION 3.0.0)
project(vaja01 C CXX)

set(CMAKE_CXX_STANDARD 20)
set(BOOST_VERSION "boost_1_73_0")


if (NOT BUILD_VERSION)
    set(BUILD_VERSION "1.0.0-dev")
endif()
message(STATUS "Configuring ${BUILD_VERSION}")

# Compiler options
if (UNIX)
    add_definitions(-std=c++20)
endif(UNIX)
if (WIN32)
    add_definitions(-std=c++2a)
endif(WIN32)
add_definitions(-DBOOST_ASIO_STRAND_IMPLEMENTATIONS=1000000)
add_definitions(-DBUILD_VERSION=${BUILD_VERSION})

# boost
if (APPLE)
    set(BOOST_ROOT "${CMAKE_CURRENT_SOURCE_DIR}/lib/macos/${BOOST_VERSION}/")
else(APPLE)
    set(BOOST_ROOT "${CMAKE_CURRENT_SOURCE_DIR}/lib/linux/${BOOST_VERSION}/")
endif(APPLE)

if(WIN32)
    set(BOOST_ROOT "${CMAKE_CURRENT_SOURCE_DIR}/lib/windows/${BOOST_VERSION}/")
endif(WIN32)

message(STATUS "BOOST_ROOT is ${BOOST_ROOT}")

# Check dependencies
# Set and find boost lib
set(Boost_USE_STATIC_LIBS ON)
set(Boost_USE_MULTITHREADED ON)
set(Boost_USE_STATIC_RUNTIME ON)

find_package(Boost COMPONENTS string program_options REQUIRED)

include_directories(${Boost_INCLUDE_DIR})
message("${Boost_INCLUDE_DIR}")

# Executable
set(LIBS ${Boost_LIBRARIES})

add_executable(vaja01 main.cpp)
target_link_libraries(vaja01 ${LIBS})

#Install
install(TARGETS vaja01 DESTINATION .)

set(CPACK_GENERATOR "TGZ")
set(CPACK_DEBIAN_PACKAGE_MAINTAINER "Urban Vidovič")
set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PACKAGE_VERSION ${BUILD_VERSION})
set(CPACK_PACKAGE_FILE_NAME "${PROJECT_NAME}-${BUILD_VERSION}")
include(CPack)