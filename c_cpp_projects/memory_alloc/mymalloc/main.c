#include <stdio.h>
#include "mymalloc.h"

int main() {

    void* p1 = mymalloc(3200);
    void* tmp = mymalloc(200);
    void* tmp1 = mymalloc(200);
    void* tmp2 = mymalloc(200);
    debug_print_info();
    printf("New print\n\n");
    myFree(tmp);
    myFree(tmp1);
    myFree(tmp2);
//    mymalloc(1000);
    debug_print_info();
    return 0;
    void* p2 = mymalloc(4000);
//    void* p3 = mymalloc(4000);
//    void* p4 = mymalloc(2000);
//    void* p5 = mymalloc(500);

//    printf("Allocating 5 blocks %x %x %x %x %x:\n",(int)p1,(int)p2,(int)p3,(int)p4,(int)p5);
    debug_print_info();

//    printf("\nFreeing block %x and %x\n", (int)p2, (int)p4);

    myFree(p2);

    debug_print_info();
    printf("\n\n\n");

    void* p6 = mymalloc(2500);

    printf("\nAllocating new block %x\n", (int)p6);
    debug_print_info();
    void* p7 = mymalloc(4000);
    
//    myFree(p3);
    myFree(p6);
    myFree(p7);
//    printf("\nFreeing block %x and %x\n", (int)p3, (int)p6);
    debug_print_info();

//    myFree(p5);
    myFree(p1);
    /*
    p1 = mymalloc(100);
    p2 = mymalloc(500);
    p3 = mymalloc(500);
    p4 = mymalloc(900);

    p5 = mymalloc(3000);

    p6 = mymalloc(2500);

    myFree(p5);
    myFree(p6);

    p5 = mymalloc(1000);
    p6 = mymalloc(700);


    myFree(p3);
    myFree(p5);
    myFree(p4);

    p3 = mymalloc(2400);

    myFree(p6);
    myFree(p2);
    myFree(p1);

    myFree(p3);
     */

    return 0;
}
