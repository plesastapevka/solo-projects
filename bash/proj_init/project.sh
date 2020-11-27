#!/bin/bash
set +
set -a
firstParam=$1 #read parameter

case $firstParam in
    "help")
        ./help.sh $2
    ;;

    "new")
        name=$2
        flag=$3
        path=$4
        export PROJECTNAME=$2
        #./new.sh ime -d /do/projekta/
        ./new.sh "$name" "$flag" "$path"
    ;;

    "activate")
        flag=$2
        path=$3
        ./activate.sh $flag $path
    ;;

    "add")
        name=$2
        ./add.sh "$name"
    ;;

    "build")
        mode=$2
        ./build.sh $mode
    ;;

    "run")
        ./run.sh
    ;;

    "test")
        mode=$2
        testname=$3
        ./test.sh $mode $testname
    ;;

    *)
        echo "Invalid usage"
    ;;
esac
