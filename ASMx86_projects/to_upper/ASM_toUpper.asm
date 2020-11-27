bits 32
extern _printf
extern _scanf
global _main

section .bss
    beseda resb 100
    beseda1 resb 1
    size resd 1

section .data
    outprint db "Vnesite besedo: ",0
    inpt db "%s",0
    izpis db "To upper: %s",10,0
    izpis1 db "bla %d",10,0
    
_main:
    pushad
    push dword outprint
    call _printf ;print navodil
    add esp, 4
    push beseda
    push dword inpt
    call _scanf
    add esp, 8
    mov ecx,0
    dec ebx
    mov ebx, beseda

.strLen: ;ta funkcija prešteje število znakov v stringu
    inc ecx
    inc ebx
    cmp byte[ebx], 0
    jnz .strLen
    mov [size], ecx
    ;od tu dalje gre pretvorba
    mov esi, beseda
    mov edi, beseda
    mov ebx, 0
    
.loopi:
    lodsb ;naloži str
    cmp eax, 90
    jle .toUpper
    sub eax, 32
    
.toUpper: ;crucial funkcija za pretvorbo v upper
    stosb
    inc ebx
    cmp ebx, [size]
    jl .loopi
    push beseda
    push dword izpis
    call _printf
    add esp, 8
    mov eax, 4
    mov ebx, 1
    mov ecx, beseda
    mov edx, 2
    int 80h
    popad
    ret