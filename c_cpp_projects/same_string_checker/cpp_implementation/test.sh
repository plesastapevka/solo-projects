#!/bin/bash

for file in ./tests/in/*.in
do
    tmp=$(cmake-build-debug/same_strings_checker < $file)
    name=$(echo $file | cut -c12-18)

    if [[ $tmp == $(cat "./tests/out/$name.out") ]]; then
        echo "OK"
    else
        echo "FAILED"
    fi
done
