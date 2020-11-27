#!/bin/bash
name=$1
projectName=$(cat ./tmp.txt)

if [[ $name == "" ]]; then
    echo "No name set."
    exit
fi

rm $PROJECTPATH/src/$name.cpp
echo "#include \"$name.hpp\"" >> $PROJECTPATH/src/$name.cpp

rm $PROJECTPATH/include/$name.hpp

echo "#ifndef ${name}_H_
#define ${name}_H_

// tukaj implementirate funkcionalnosti objekta $name

#endif // ${name}_H_" >> $PROJECTPATH/include/$name.hpp
