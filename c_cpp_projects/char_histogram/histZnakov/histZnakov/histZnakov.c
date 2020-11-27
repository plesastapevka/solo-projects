//
//  main.c
//  histZnakov
//
//  Created by Urban Vidovič on 13/01/2020.
//  Copyright © 2020 Urban Vidovič. All rights reserved.
//

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <fcntl.h>
#include <stdbool.h>
#include <sys/mman.h>
#include <sys/stat.h>
#include <time.h>

void count_occurrences(const char arr[], int n, char x) {
    int res = 0;
    for (int i = 0; i < n; ++i) {
        if (x == arr[i]) res++;
    }
    
    printf("%c - %d\n", x, res);
}

bool char_inspected(char curr, const char ins[], int bytes) {
    for(int i = 0; i < bytes; ++i) {
        if(curr == ins[i]) return true;
    }
    
    return false;
}

void count_characters(const char data[], int bytes) {
    char inspected[bytes];
    char currchar = data[0];
    for(int i = 1; i < bytes; ++i) {
        if(!char_inspected(currchar, inspected, bytes)) {
            count_occurrences(data, bytes, currchar);
            inspected[i] = currchar;
        }
        currchar = data[i];
    }
}

void type_read(int bytes, const char* path) {
    int file = open(path, O_RDONLY);
    off_t fsize = lseek(file, 0, SEEK_END);
    
    close(file);
    file = open(path, O_RDONLY);
//    int size = (int)fsize;
    char data[(int)fsize];
    char tmp_data[bytes];
    if (file < 0) {
        write(2, "Error opening file.\n", 20);
        exit(1);
    }
    
    int index = 0;
    while(1) {
        int r = (int)read(file, tmp_data, bytes);
        for(int i = 0; i < bytes; ++i) {
            if(index <= fsize) {
                data[index] = tmp_data[i];
            }
            index++;
        }
        tmp_data[r] = 0;
        if(r == 0) break;
    }
    close(file);
    count_characters(data, (int)fsize);
}


void type_mmap(const char* path) {
//    size_t page = getpagesize();
    int file = open(path, O_RDWR|O_SYNC);
    
    if (file < 0) {
        write(2, "Error opening file.\n", 20);
        exit(1);
    }
    
    char* result;
    unsigned int len;
    struct stat buf;
    
    len = (unsigned int)buf.st_size;
    size_t page = getpagesize();
    result = (char*)mmap(0, page, PROT_READ,MAP_FILE|MAP_PRIVATE, file, 0);
    if (result == MAP_FAILED) write(2, "Error allocating memory.\n", 25);
    
    count_characters(result, (int)strlen(result));
}

int main(int argc, const char * argv[]) {
    int bytes = 0;
    const char* type = NULL;
    const char* path = argv[argc - 1];
    
    for (int i = 1; i < argc - 1; ++i) {
        if(strcmp(argv[i], "-b") == 0) {
            bytes = atoi(argv[++i]);
        } else if(strcmp(argv[i], "-t") == 0) {
            type = argv[++i];
        }
    }

    clock_t start, end;
    
    start = clock();
    if(strcmp(type, "read") == 0) {
        type_read(bytes, path);
    } else if(strcmp(type, "mmap") == 0) {
        type_mmap(path);
    } else {
        printf("Error: Invalid type.\n");
        return 1;
    }
    
    end = clock();
    
    printf("Type: %s\n", type);
    printf("Time elapsed: %f milliseconds\n", ((double)(end - start)));
    printf("File: %s\n", path);
    printf("Bytes: %d\n\n", bytes);
    
    return 0;
}
