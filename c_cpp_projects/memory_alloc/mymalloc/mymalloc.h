#ifndef DMA_MYMALLOC_H
#define DMA_MYMALLOC_H

#include <stddef.h>

void* mymalloc(size_t size);
void myFree(void* m);

size_t customRound(size_t n);
void allocNewPage(void);
void init(void);
void debug_print_info(void);

#endif //DMA_MYMALLOC_H
