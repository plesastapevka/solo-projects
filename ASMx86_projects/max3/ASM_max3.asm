bits 32
global _main
extern _printf, _scanf

section .bss
inpt1 resd 1
inpt2 resd 1
inpt3 resd 1

section .text

max3:
    mov eax, [esp+4] ;prvi arg
    mov ebx, [esp+8] ;drugi arg
    mov ecx, [esp+12] ;tretji arg
    cmp eax, ebx
    jg .1vs3 ;ce je eax > ebx skoci na 1vs3
    cmp ebx, ecx ;ce je ebx > ecx, izpiši ebx
    jg .2veax
    jmp .3veax

.1vs3:
    cmp eax, ecx ;ce je eax > ecx
    jg .vrni ;vrni eax value
    jmp .3veax
    
.2veax:
    mov eax, ebx
    ret
    
.3veax:
    mov eax, ecx ;ecx v eax, ker je eax ret value
    ret 
    
.vrni:
    ret
    

_main:
    mov ebp, esp; for correct debugging
    
    ;print prompta
    push msg
    call _printf
    add esp, 4
    
    ;prejem prvega st
    push inpt1
    push fmt
    call _scanf
    add esp, 8
    
    ;prejem drugega st
    push inpt2
    push fmt
    call _scanf
    add esp, 8
    
    ;prejem tretjega st
    push inpt3
    push fmt
    call _scanf
    add esp, 8
    
    ;nalaganje za klic funkcije
    push dword[inpt1]
    push dword[inpt2]
    push dword[inpt3]
    call max3
    add esp, 12
    
    ;izpis najvecjega
    push eax
    push printfmt
    call _printf
    add esp, 8

ret

section .data
msg db "Vnesi 3 stevila: ", 0 ;message za zacetek programa - prompt
fmt db "%d", 0 ;format za scanf
printfmt db "%d", 10, 0 ;format za printf