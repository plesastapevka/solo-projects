//
//  main.c
//  bintoascii
//
//  Created by Urban Vidovič on 19/01/2019.
//  Copyright © 2019 Urban Vidovič. All rights reserved.
//

#include <stdio.h>
#include <getopt.h>
#include <memory.h>
#include <stdlib.h>
#include <unistd.h>
#include <math.h>
#include <sys/wait.h>
#include <string.h>
#include <fcntl.h>

int main(int argc, char *argv[]) {
    int opt;
    int vFile = 0; 
    int izFile = 0;
    char *inFile, *outFile;
    int izOpriimek = 0, vOpriimek = 1;

    if(argc == 1) {
        int i = 0;
        long long arr[10000];
        char tmp;
        long long ret;
        printf("Input numbers: \n");
        do {
            scanf("%lld%c", &arr[i], &tmp);
            ret = arr[i];
            i++;
            printf("%d ", binToDec(ret));
        } while(tmp != '\n');
        printf("\n");
        return 0;
    }

    while ((opt = getopt(argc, argv, "o:i:O:I:")) != -1) {
        switch (opt) {
            case 'i': {
                inFile = optarg;
                printf("Input file: %s\n", inFile);
                izFile = 1;
                break;
            }
            case 'o': {
                outFile = optarg;
                printf("Output file: %s\n", outFile);
                vFile = 1;
                break;
            }
        }
    }

    //spodaj poteka potrebna inicializacija fileov I/O
    int in, out;
    //writing FILE -> FILE
    if (izFile == 1 && vFile == 1) {
        in = open(inFile, O_RDONLY);
        out = open(outFile, O_APPEND | O_CREAT | O_WRONLY, 0644);

        if (in == -1) {
            printf("Could not open input file.\n");
            return -1;
        }
        if (out == -1) {
            printf("Could not open output file.\n");
            return -1;
        }

        char *ven = calloc(5, sizeof(char)); //alokacija prostora za array
        char byte[1];
        int read1 = 1, write1, prejeto;
        for (;;) {

            read1 = (int) read(in, byte, 1); //prebere 1 byte
            if (read1 == 0) break; //če je konec zaključi
            if (read1 == -1) { //error
                printf("Error in reading.\n");
                return -1;
            }

            //prejeto zapiši v out
            prejeto = snprintf(ven, 5, "%d ", (int) byte[0]);
            write1 = (int) write(out, ven, prejeto);

            if (write1 == -1) { //error
                printf("Error in writing.\n");
                return -1;
            }
        }
    }

    //writing FILE -> STDIN
    if (izFile == 1 && vFile != 1) {
        char *ven = calloc(5, sizeof(char)); //alokacija prostora za array
        in = open(inFile, O_RDONLY);
        if (in == -1) {
            printf("Could not open input file.\n");
            return -1;
        }

        char byte[1];
        int read1 = 1, write1, prejeto;
        for (;;) {

            read1 = (int) read(in, byte, 1); //prebere 1 byte
            if (read1 == 0) break; //če je konec zaključi
            if (read1 == -1) { //error
                printf("Error in reading.\n");
                return -1;
            }

            //prejeto zapiši v out
            prejeto = snprintf(ven, 5, "%d ", (int) byte[0]);
            write1 = (int) write(1, ven, prejeto);

            if (write1 == -1) { //error
                printf("Error in writing.\n");
                return -1;
            }
        }
    }
    
    //writing STDIN -> FILE
    if (izFile != 1 && vFile == 1) {
        char *ven = calloc(5, sizeof(char)); //alokacija prostora za array
        out = open(outFile, O_APPEND | O_CREAT | O_WRONLY, 0644);
        if (out == -1) {
            printf("Could not open input file.\n");
            return -1;
        }

        int i = 0, prejeto, write1;
        long long arr[10000];
        char tmp;
        long long ret;
        //branje bin števil na stdinu
        printf("Input numbers: \n");
        do {
            scanf("%lld%c", &arr[i], &tmp); //prebere po 8 bitov
            ret = arr[i];
            i++;
            printf("%d ", binToDec(ret));
            prejeto = snprintf(ven, 5, "%d ", (int) binToDec(ret));
            write1 = (int) write(out, ven, prejeto);
        } while(tmp != '\n');
        printf("\n");
    }

}

int binToDec(long long n) {
    int decNum = 0, i = 0, rem;
    while(n != 0) {
        rem = n % 10;
        n /= 10;
        decNum += rem*pow(2, i);
        ++i;
    }
    return decNum;
}