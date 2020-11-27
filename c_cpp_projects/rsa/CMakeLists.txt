cmake_minimum_required(VERSION 3.0.0)
project(vaja02_rsa C CXX)

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

#add_compile_options(-lboost_random)

add_definitions(-DBOOST_ASIO_STRAND_IMPLEMENTATIONS=1000000)
add_definitions(-DBUILD_VERSION=${BUILD_VERSION})

# boost
if (APPLE)
    set(BOOST_ROOT "/Users/plesasta_pevka/repos/lib/macos/${BOOST_VERSION}/")
else(APPLE)
    set(BOOST_ROOT "/Users/plesasta_pevka/repos/lib/linux/${BOOST_VERSION}/")
endif(APPLE)

if(WIN32)
    set(BOOST_ROOT "/Users/plesasta_pevka/repos/lib/macos/lib/windows/${BOOST_VERSION}/")
endif(WIN32)

message(STATUS "BOOST_ROOT is ${BOOST_ROOT}")

# Check dependencies
# Set and find boost lib
set(Boost_USE_STATIC_LIBS ON)
set(Boost_USE_MULTITHREADED ON)
set(Boost_USE_STATIC_RUNTIME ON)

find_package(Boost COMPONENTS program_options chrono REQUIRED)

include_directories(${Boost_INCLUDE_DIRS})
include_directories(include)
message(STATUS "BOOST_INCLUDE_DIRS is ${Boost_INCLUDE_DIRS}")

# Executable
set(LIBS ${Boost_LIBRARIES})

add_executable(vaja02_rsa
        main.cpp
        include/utils.cpp
        include/utils.h
        )

target_link_libraries(vaja02_rsa ${LIBS})

#Install
install(TARGETS vaja02_rsa DESTINATION .)

set(CPACK_GENERATOR "TGZ")
set(CPACK_DEBIAN_PACKAGE_MAINTAINER "Urban Vidovič")
set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PACKAGE_VERSION ${BUILD_VERSION})
set(CPACK_PACKAGE_FILE_NAME "${PROJECT_NAME}-${BUILD_VERSION}")
include(CPack)