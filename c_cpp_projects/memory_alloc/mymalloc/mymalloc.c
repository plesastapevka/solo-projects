#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/mman.h>
#include "mymalloc.h"

#define PAGE_SIZE (sysconf(_SC_PAGESIZE))
#define PAGE_INFO_SIZE 96 // bytes
#define BLOCK_INFO_SIZE 32 // bytes
#define MIN_BLOCK_SIZE 6 // bytes

// Page structure
typedef struct Page {
    size_t size;
    struct Page* next;
    struct Page* prev;
    struct Block* first_block;
} Page;

// memory Block structure
typedef struct Block {
    size_t size;
    struct Block* prev;
    struct Block* next;
} Block;


Page* first_page = NULL;
Page* last_page = NULL;

// Initialization of first page
void init() {
    void *block, *address;
    address = mmap(NULL,PAGE_SIZE,PROT_READ|PROT_WRITE,MAP_PRIVATE|MAP_ANONYMOUS,-1,0);

    if(address == MAP_FAILED) {
        printf("Error with mmap\n");
    } else {
        first_page = (Page*)address;
        last_page = first_page;
        first_page->size = (size_t)PAGE_SIZE;
        // kaze na konec memory_bloka
        block = (address + PAGE_INFO_SIZE + BLOCK_INFO_SIZE );

        first_page->first_block = (Block *)block;
        first_page->first_block->size = (size_t) PAGE_SIZE - PAGE_INFO_SIZE;
    }
}

// Page Allocation
void allocNewPage(){
    if(first_page == NULL) {
        // first allocation
        init();
    } else {
        void* page;
        Page* left_page;

        page = mmap(NULL,PAGE_SIZE,PROT_READ|PROT_WRITE,MAP_PRIVATE|MAP_ANONYMOUS,-1,0);
        if(page == MAP_FAILED) {
            printf("Error with mmap\n");
        } else {
            left_page = last_page;
            last_page->next = (Page *)page;
            last_page = last_page->next;

            //povezemo strani in inacializiramo size strani
            left_page->next = last_page;
            last_page->prev = left_page;
            last_page->size = (size_t)PAGE_SIZE;

            //kazalec premaknemo na konec page hederja
            page = (page + PAGE_INFO_SIZE + BLOCK_INFO_SIZE);

            // ga prestavimo do prvega segmenta in incializiram vrednosti
            last_page->first_block = (Block *)page;
            last_page->first_block->size = (size_t) PAGE_SIZE - PAGE_INFO_SIZE;
        }
    }
}

void deletePage(void* page_address){
    Page* delete_page = (Page *)page_address;

    // ce imamo samo eno stran
    if(delete_page->next == NULL && delete_page->prev == NULL) {
        munmap(page_address,PAGE_SIZE);
        first_page = NULL;
        last_page = NULL;
    } else if(delete_page->next != NULL && delete_page->prev != NULL) { // ce je vmes med dvema pageoma
        delete_page->prev->next = delete_page->next;
        delete_page->next->prev = delete_page->prev;
        munmap(page_address,PAGE_SIZE);
    } else if(delete_page->next != NULL) { // ce brisemo prvo stran
        delete_page->next->prev = NULL;
        first_page = delete_page->next;
        munmap(page_address,PAGE_SIZE);
    } else if(delete_page->prev != NULL) { // ce brisemo zadnjo stran
        delete_page->prev->next = NULL;
        last_page = delete_page->prev;
        munmap(page_address,PAGE_SIZE);
    } else {
        printf("Warning: Unhandled exception!\n");
    }
}

void myFree(void *m) {
    if(m != NULL) {
        Block *block, *left_block, *right_block;

        block = (Block *)m;
        left_block = block->prev;
        right_block = block->next;

        // sprostimo blok
        block->size ^= 1UL << 0;

        // ce je edini allociran block na pagu potem bomo zbrisali page
        if(left_block == NULL && right_block == NULL) {
            m = (m - BLOCK_INFO_SIZE - PAGE_INFO_SIZE);
            deletePage(m);
        }
            
        else if(left_block != NULL && right_block != NULL) { // ce je Block vmes med dvema drugima
            // ce sta oba free
            if ((!(left_block->size & 1)) && (!(right_block->size & 1))) {
                // ce kazeta oba L->prev = NULL in R->next = NULL then delete page
                if (left_block->prev == NULL && right_block->next == NULL) {
                    m = (void *) left_block;
                    m = (m - BLOCK_INFO_SIZE - PAGE_INFO_SIZE);
                    deletePage(m);
                } else { // merge it
                    //merging
                    left_block->next = right_block->next;
                    if(right_block->next != NULL) {
                        right_block->next->prev = left_block;
                    }
                    left_block->size += block->size + right_block->size;

                    // clearing data
                    block->next = NULL;
                    block->prev = NULL;
                    block->size = 0;

                    right_block->next = NULL;
                    right_block->prev = NULL;
                    right_block->size = 0;
                }
            } else if (!(left_block->size & 1)) {
                // merging
                left_block->next = right_block;
                right_block->prev = left_block;
                left_block->size += block->size;

                // clearing data
                block->next = NULL;
                block->prev = NULL;
                block->size = 0;
            }
                
            else if (!(right_block->size & 1)) { // ce je desni free merge it
                // merging
                block->next = right_block->next;
                if(right_block->next != NULL) {
                    right_block->next->prev = block;
                }
                block->size += right_block->size;

                // clearing data
                right_block->next = NULL;
                right_block->prev = NULL;
                right_block->size = 0;
            } else {
                printf("Warning: Unhandled exception!\n");
            }
        } else if(right_block != NULL) { // ce je prvi block na page
            // ce je free
            if(!(right_block->size & 1)) {
                // true delete page
                if(right_block->next == NULL) {
                    m = (m - BLOCK_INFO_SIZE - PAGE_INFO_SIZE);
                    deletePage(m);
                } else { // mergaj
                    // merging
                    block->next = right_block->next;
                    right_block->next->prev = block;
                    //right_block->size ^= 1UL << 0;
                    block->size += right_block->size;

                    //clearing data
                    right_block->prev = NULL;
                    right_block->next = NULL;
                    right_block->size = 0;
                }
            }
        }
            
        else if(left_block != NULL) { // ce je zadnji block na page
            //ce je free
            if(!(left_block->size & 1)) {
                // delete page
                if(left_block->prev == NULL ) {
                    m = (void *)left_block;
                    m = (m - BLOCK_INFO_SIZE - PAGE_INFO_SIZE);
                    deletePage(m);
                } else { // mergaj
                    // merging
                    left_block->next = right_block;
                    left_block->size += block->size;

                    //clearing data
                    block->prev = NULL;
                    block->next = NULL;
                    block->size = 0;
                }
            }
        } else {
            printf("Warning: Unhandled exception!\n");
        }
    }
}

void *mymalloc(size_t size) {
    // najdemo primerno velikost
    size_t suitable_size = customRound(size);
//    printf("Page size: %d", PAGE_SIZE);

    // preverimo ce je allociran Block vecji od page
    if(suitable_size > PAGE_SIZE - PAGE_INFO_SIZE){
        printf("Error alocirali ste Block vecji kot page\n");
        return NULL;
    }

    Page* tmp_page;
    Block* tmp_block;
    void* memory_block_header;

    // preverimo ce se nimamo nobenega pagea
    if(first_page == NULL) {
        allocNewPage();
    }

    // prvo pogledamo zadnjo stran ce lahko tam kje najdemo prosto
    tmp_block = last_page->first_block;

    while(tmp_block != NULL) {
        // ce se ni Block alociran in ce je size vecji ali enak kot user size
        if( (!(tmp_block->size & 1)) && tmp_block->size >=  suitable_size){
            // nasli smo prvi primeren Block za allokacijo
            // pogledamo ce lahko alociramo novi header ce imamo toliko prostora
            if((tmp_block->size - suitable_size) > BLOCK_INFO_SIZE){
                /*void**/ //memory_block_header= NULL;

                memory_block_header = (void *) tmp_block;
                memory_block_header = (memory_block_header + suitable_size);

                tmp_block->next= (Block *)memory_block_header;
                tmp_block->next->prev = tmp_block;
                tmp_block->next->size = tmp_block->size - suitable_size;

                tmp_block->size = suitable_size;
                //spremenimo zadnji bit s tem povemo da je Block ze alociran
                tmp_block->size ^= 1UL << 0;

                return (void *) tmp_block;
            } else {
                // ce nimamo dosti prostora da bi allocirali novi block_header
                //tmp_page->first_block->next = NULL;
                tmp_block->size = suitable_size;
                tmp_block->size ^= 1UL << 0;

                return (void *)tmp_block;
            }
        }
        tmp_block = tmp_block->next;
    }

    //size_t suitable_size = find_optimal_memory_size(size);
    /*Page**/ tmp_page = first_page;

    // se sprehodimo ce vse strani in vse blocke na strani
    while(tmp_page != NULL){
        /*Block**/ tmp_block = tmp_page->first_block;
        while(tmp_block != NULL){
            // ce se ni Block alociran in ce je size vecji ali enak kot user size
            if((!(tmp_block->size & 1)) && tmp_block->size >=  suitable_size) {
                // nasli smo prvi primeren Block za allokacijo
                // pogledamo ce lahko alociramo novi header ce imamo toliko prostora
                if((tmp_block->size - suitable_size) > BLOCK_INFO_SIZE){
                    /*void**/ //memory_block_header= NULL;

                    memory_block_header = (void *) tmp_block;
                    memory_block_header = (memory_block_header + suitable_size);

                    tmp_block->next= (Block *)memory_block_header;
                    tmp_block->next->prev = tmp_block;
                    tmp_block->next->size = tmp_block->size - suitable_size;

                    tmp_block->size = suitable_size;
                    //spremenimo zadni bit s tem povemo da je Block ze alociran
                    tmp_block->size ^= 1UL << 0;

                    return (void *) tmp_block;
                } else {
                    // ce nimamo dosti prostora da bi allocirali novi block_header
                    //tmp_page->first_block->next = NULL;
                    tmp_block->size = suitable_size;
                    //1UL being basically long 1 in bits: 00000000000000000000000000000001
                    tmp_block->size ^= 1UL << 0;

                    return (void *)tmp_block;
                }
            }
            tmp_block = tmp_block->next;
        }
        tmp_page = tmp_page->next;
    }

    // ce nenajdemo dobenega primernega blocka oz. dosti velikega
    // allociramo novo stran
    allocNewPage();

    // pogledamo ce lahko alociramo novi block_info ce imamo dovolj prostora
    if((last_page->first_block->size - suitable_size) > BLOCK_INFO_SIZE){

        /*void* */memory_block_header = (void *) last_page->first_block;
        memory_block_header = (memory_block_header + suitable_size);

        //potem lahko alociramo nov block_info_size
        //premaknemo kazalec first_block->next na konec novega blocka info
        last_page->first_block->next = (Block *)memory_block_header;
        //ga povezemo s prejsnim blokom
        last_page->first_block->next->prev = last_page->first_block;
        // nastavimo free size tega novega blocka
        last_page->first_block->next->size = last_page->first_block->size - suitable_size;
        // spremenimo size tmp_page->first_block
        last_page->first_block->size = suitable_size;
        // nastavimo prvi bit na true to nam pove da je Block alociran in da ni free
        last_page->first_block->size ^= 1UL << 0;

        return (void *)last_page->first_block;
    } else {
        // ce nimamo dosti prostora da bi allocirali novi block_header
        last_page->first_block->size = suitable_size;
        last_page->first_block->size ^= 1UL << 0;

        return (void *)last_page->first_block;
    }
}

// rounding number to nearest multiple of 10
size_t customRound(size_t n) {
    if( n < MIN_BLOCK_SIZE ) {
        n = MIN_BLOCK_SIZE;
    }

    size_t a,b;
    // Smaller multiple
    a = (n / 10) * 10;

    // Larger multiple
    b = a + 10;

    // Return of closest of two
    return (n - a > b - n) ? b : a;
}

void debug_print_info() {
    Page* tmp_page;
    Block* tmp_block;

    tmp_page = first_page;
    while(tmp_page != NULL)
    {
        printf("New page.\n");
        tmp_block = tmp_page->first_block;
        printf("/////////////////////////////////////////\n"
               "                                         \n"
               "     page address: %x                    \n"
               "     size:  %lu                          \n"
               "     next: %x                            \n"
               "     prev: %x                            \n"
               "     block: %x                           \n"
               "                                         \n"
               "/////////////////////////////////////////\n"
               "                     ||\n"
               "                     ||\n",
               (int)tmp_page, tmp_page->size, (int)tmp_page->next, (int)tmp_page->prev,(int)tmp_block);

        while(tmp_block != NULL) {
            printf("///////////////////////////////////////\n"
                   "                                       \n"
                   "     block address: %x                 \n"
                   "     free: %i                          \n"
                   "     size:  %lu                        \n"
                   "     next: %x                          \n"
                   "     prev: %x                          \n"
                   "                                       \n"
                   "///////////////////////////////////////\n"
                   "                     ||\n"
                   "                     ||\n",
                   (int)tmp_block,(int)(!(tmp_block->size)) , tmp_block->size, (int)tmp_block->next, (int)tmp_block->prev);

            tmp_block = tmp_block->next;
        }
        tmp_page = tmp_page->next;
        printf("\n\n");
    }
}
