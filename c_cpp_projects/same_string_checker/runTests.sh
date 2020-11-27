#!/bin/bash

programName='./bruteforce.py'
inputFilePath='./tests/in/'
inputFiles=($(ls $inputFilePath))
outputFilePath='./tests/out/'
outputFiles=($(ls $outputFilePath))

echo "input files: ${inputFiles[*]}"
echo "output files: ${outputFiles[*]}"

for i in "${!inputFiles[@]}"; do

  res=$(python "$programName" < "$inputFilePath${inputFiles[$i]}")
  expectedRes=$(cat $outputFilePath"${outputFiles[$i]}")

  if [ "$res" == "$expectedRes" ]; then
     printf "run %s: python %s < %s SUCCESS\n" "$i" "$programName" "${inputFiles[$i]}"
  else
     printf "run %s: python %s < %s ERROR\n" "$i" "$programName" "${inputFiles[$i]}"
  fi

done