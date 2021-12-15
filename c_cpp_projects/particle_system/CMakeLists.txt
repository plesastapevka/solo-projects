cmake_minimum_required(VERSION 3.20)
project(particle_system)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_OSX_ARCHITECTURES "x86_64")
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_AUTOUIC ON)
set(CMAKE_PREFIX_PATH "$ENV{HOME}/Qt5.12.12/5.12.12/clang_64/lib/cmake/")

set(SOURCES
        main.cpp
        window/mainwindow.cpp
        window/widgetopengldraw.cpp
        )

set(MOC_HEADERS
        window/mainwindow.h
        window/widgetopengldraw.h
        )

set(UIS
        window/mainwindow.ui
        )

# Including GLFW
include_directories("/usr/local/include")
link_directories("/usr/local/lib")

# Finding QT
find_package(Qt5Widgets REQUIRED)

# Custom header files linking
include_directories("./window")

# Executable and lib linking
add_executable(particle_system main.cpp ${SOURCES} ${MOC_HEADERS} ${UIS} window/mainwindow.cpp)
target_link_libraries(particle_system
        Qt5::Widgets
        glfw
        )

