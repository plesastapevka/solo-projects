#!/bin/bash
flag=""
succesful=0
failed=0
while test $# -gt 0; do
  case "$1" in
    -l|-v)
      flag="$1"
      shift
      ;;

    *)
      testname=$1
      shift
    ;;
  esac
done

case $flag in
    "-l")
        if [[ $testname != "" ]]; then
          echo "This command can only be used without name parameter"
          exit
        fi
        echo "Test names:"
        for file in $PROJECTPATH\tests/*; do
          name="${file#*test_}"
          echo "${name%.sh*}"
        done
    ;;

    "-v")
        if [[ $testname == "" ]]; then
          for file in $PROJECTPATH\tests/*; do
            valgrind --quiet --trace-children=yes --error-exitcode=1 $file
            if [[ $? == 0 ]]; then
              ((succesful++))
            else
              ((failed++))
            fi
          done
          echo "Succesful tests: $succesful"
          echo "Failed tests: $failed"
        else
          valgrind --quiet --trace-children=yes --error-exitcode=1 $PROJECTPATH\tests/test_${testname}.sh
          if [[ $? == 0 ]]; then
            ((succesful++))
          else
            ((failed++))
          fi
          echo "Succesful tests: $succesful"
          echo "Failed tests: $failed"
        fi
    ;;

    *)
        if [[ $testname == "" ]]; then
          for file in $PROJECTPATH\tests/*; do
            $file
            if [[ $? == 0 ]]; then
              ((succesful++))
            else
              ((failed++))
            fi
          done
          echo "Succesful tests: $succesful"
          echo "Failed tests: $failed"
        else
          $PROJECTPATH\tests/test_${testname}.sh
          if [[ $? == 0 ]]; then
            ((succesful++))
          else
            ((failed++))
          fi
          echo "Succesful tests: $succesful"
          echo "Failed tests: $failed"
        fi
    ;;

    *)
    echo "Incorrect input"
    exit 0
    ;;
esac
