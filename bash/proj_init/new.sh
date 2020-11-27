#!/bin/bash
#new <ime projekta> [-d <pot>]

name=$1 #ime projekta
flag=$2 #-d
if [[ $name == "" ]]; then
  echo "Must specify a project name."
  exit
fi

rm ./tmp.txt
echo $name >> tmp.txt

case $flag in
    "-d")
        path=$3
        echo "Creating project in $path/project_$name"
    ;;

    "")
        path=""
        echo "Creating project in /project_$name"
    ;;

    *)
    echo "Incorrect input"
    exit 0
    ;;
esac

#create project

mkdir $path"project_$name"/ #project_<ime projekta>/
path=$path"project_$name/"/
mkdir $path"src"/           #-src/
mkdir $path"include"/       #-include/
mkdir $path"build"/         #-build/
mkdir $path"tests"/         #-tests/
touch $path"main.cpp"         #-main.c

echo "#include <iostream>
int main()
{
   std::cout << \"Hello World\" << std::endl;
   return 0;
}" > $path"main.cpp"

touch $path"Makefile"       #-Makefile

echo $name >> $path"tmp.txt"
