bits 32
global _main
extern _printf

section .data
msg db "Urban Vidovic, E1101241 ", 0



section .text

_main:
    
    ;print prompta
    push msg
    call _printf
    add esp, 4
    
    ret