#!/bin/bash
set -a

flag=$1 #-d ali -r
path=$2
if [[ $flag == "-d" && $path == "" ]]; then
    echo "No path set."
    exit
fi
case $flag in
    "-d")
        echo "Project activate - on: $path"
        PROJECTNAME=$(cat $path/tmp.txt)
        PROJECTPATH="$path"
    ;;

    "")
        echo "Project activate - no path set"
        PROJECTNAME=$(cat ./tmp.txt)
        PROJECTPATH="$(dirname $0)/project_$PROJECTNAME/"
    ;;

    *)
    echo "Incorrect input"
    exit 0
    ;;
esac

echo "Activating project $PROJECTNAME in $PROJECTPATH"

echo "# prevajalnik za C++
CXX= g++
# dodatne zastavice, ki se posredujejo prevajalniku
CXXFLAGS= -Wall -Iinclude
# ime cilja - programa v katerega prevedemo program
TARGET= $PROJECTNAME

# avtomatsko iskanje izvornih datotek, v src in trenutnem direktoriju
SRCS := \$(wildcard src/*.cpp *.cpp)
OBJS := \$(SRCS:src/%.cpp=build/%.o)

# izpis informacij
\$(info Compiler: \$(CXX))
\$(info Flags: \$(CXXFLAGS))
\$(info Target: \$(TARGET))
\$(info Source files: \$(SRCS))
\$(info Object files: \$(OBJS))
\$(info )

#  cilj ki prevede celoten projekt
all: build/\$(TARGET)

# cilj, ki prevede program za razhroscevanje
debug: CXXFLAGS:= -g -O0 \$(CXXFLAGS)
debug: all

# cilj, ki prevede program za izdajo
release: CXXFLAGS:= -O3 \$(CXXFLAGS)
release: all

# cilj, ki pocisti prevedene zbirke
clean:
	rm -rf build/*

# cilj, ki prevede posamezno izvorno zbirko v objektni modul
build/%.o: src/%.cpp
	\$(CXX) \$(CXXFLAGS) -c -o \$@ \$^

# cilj, ki poveze objektne module v program
build/\$(TARGET): \$(OBJS)
	\$(CXX) \$(CXXFLAGS) -o \$@ \$^" >> $PROJECTPATH"Makefile"

rm ./tmp.txt
bash --rcfile "$(dirname $0)/my_bashrc"
