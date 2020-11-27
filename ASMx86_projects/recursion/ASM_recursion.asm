global _main
extern _printf, _scanf
section .data
    printTxt db "%d. clen: ", 0
    printNum db "%d", 10, 0

section .text

rec:
    mov edx, [esp+4] ;prejemanje prvega arg
    cmp edx, 0 ;counter za št clena
    je .eq1 ;ce je edx = 0 premakni 1 v eax
    cmp edx, 1                          ;to zato ker sta 0. in 1. clen enaka 1
    je .eq1 ;ce je edx = 1 premakni 1 v eax
    cmp edx, 2 ;ce je edx = 2
    je .eq7    ;premakni 7 v eax, ker je 2. clen 7
    sub edx, 2 ;dva clena nazaj
    push edx ;naloži na sklad za nadaljno gledanje rekurzije
    call rec ;rekurzivni klic funkcije
    pop edx
    mov ecx, eax ;eax v ecx n-3. clena
    push ecx 
    sub edx, 1 ;en clen nazaj
    push edx ;spet isto
    call rec ;spet isto
    pop edx 
    pop ecx
    add eax, ecx ;seštej eax in ecx za primerni clen in vrni
    ret	
    
.eq1:
    mov eax, 1
    ret

.eq7:
    mov eax, 7
    ret

_main:
    
    mov ebx, 2 ;ker je n > 2
    
    .loop1:
        add ebx, 1
        push ebx
        call rec
        pop ebx
        
        push eax ;za izpis, ret value od rekurzije
        push printNum
        call _printf
        add esp, 8
        cmp ebx, 15
        jl .loop1
    
    ret