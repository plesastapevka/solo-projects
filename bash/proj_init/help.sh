#!/bin/bash

firsParam=$1

case $firstParam in
    "help")
        echo "usage: help [ukaz]"
    ;;

    "new")
        echo "Help - New"
    ;;

    "activate")
        echo "Help - Activate"
    ;;

    "add")
        echo "Help - Add"
    ;;

    "build")
        echo "Help - Build"
    ;;

    "run")
        echo "Help - Run"
    ;;

    "test")
        echo "Help - Test"
    ;;

    *)
        cat default.txt
    ;;
esac
