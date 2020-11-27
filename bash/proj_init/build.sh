#!/bin/bash

flag=$1 #-d ali -r
case $flag in
    "-d")
        echo "Debug build"
        make -C $PROJECTPATH clean debug
    ;;

    "-r")
        echo "Release build"
        make -C $PROJECTPATH clean all
    ;;

    "")
        echo "Default build - debug"
        make -C $PROJECTPATH clean debug
    ;;

    *)
    echo "Incorrect input"
    exit 0
    ;;
esac
