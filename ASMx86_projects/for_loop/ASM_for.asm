bits 32
global _main
extern _printf, _scanf

section .bss


section .text

_main:
    mov ebp, esp; for correct debugging
    
    ;print prompta
    push msg
    call _printf
    add esp, 4
    
    ;hranjenje variable
    push inpt
    push fmt
    call _scanf
    add esp, 8
    ;
    ;zacetek 1 za nadaljevanje +1 
    mov eax, 1
    mov ecx, dword[inpt] ;za for loop
    
    .loop1:
    push ecx ;nalozi na sklad ecx register, kjer je counter za for loop
    push eax ;nalozi na sklad eax, kjer je st ki ga je potrebno izpisat
    push printfmt ;nalozi format za izpis
    call _printf
    add esp, 4 ;pobrise sklad
    pop eax ;odstrani eax 
    pop ecx ;odstrani ecx  
    add eax, 1
    
    loop .loop1, ecx
    
ret

section .data
msg db "Vnesi st: ", 0 ;message za zacetek programa - prompt
fmt db "%d", 0 ;format za scanf
inpt times 4 db 4 ;format za branje integerja
printfmt db "%d", 10, 0 ;format za printf