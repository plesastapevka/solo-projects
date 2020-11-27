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
    
    ;zacetek 11, za nadaljevanje +1 
    mov ebx, 11 ;delitelj
    mov eax, 1 ;prvi deljenec
    mov ecx, dword[inpt] ;za for loop
    
    .loop1:
        push ecx
        jmp .divide ;zacne z deljenjem
    
    .intervene:
        mov eax, edi ;st kjer smo ostali premaknemo nazaj v eax iz edi
        inc eax ;pristeje eax 1 za preverjanje naprej
        pop ecx
        loop .loop1 ;skok nazaj na zacetek loopa
        ret ;ko je konec loopa je konec programa
    
    .divide:
        ;eax ima vrednost ki se povecuje - za to vrednost moram
        ;gledat ce je deljiva z 11
        mov edx, 0 ;resetiramo edx da je pripravljen za shranjevanje ostanka
        mov edi, eax ;damo eax vrednost v edi za potem
        div ebx ;instrukcija za deljenje, shrani kvocient v eax, ostanek v edx
                ;prejsnji eax value imam v edi
         
        cmp edx, 0
        je .print ;ce je edx = 0 skoci na print, mora izpisat eax
        jmp .intervene ;drugace na intervene
        
        
    .print:
        push edi ;push na sklad za izpis integerja
        push printfmt ;push na sklad za format izpisa
        call _printf ;klica printa
        add esp, 8 ;pomik na stack pointeru dol
        jmp .intervene
    
ret

section .data
msg db "Vnesi st: ", 0 ;message za zacetek programa - prompt
fmt db "%d", 0 ;format za scanf
inpt times 4 db 4 ;format za branje integerja
printfmt db "%d", 10, 0 ;format za printf