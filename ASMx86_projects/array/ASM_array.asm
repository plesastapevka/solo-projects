bits 32
extern _printf
extern _scanf
global _main

section .bss
    number resd 1

section .data
    array dd 1,2,3,4,5,6,7,8,9
    size dd 9
    outpt db "%d ",0,0
    summ db "Vsota: %d",10,10,0
    row db "",10,0

_function:
    push ebp
    mov ebp, esp
    mov esi, [ebp + 12] 
    mov ecx, [ebp + 8]  
    sub ecx,1 ;substract counterja
    lea edi, [esi+4*ecx] ;naloži naslov v edi

.loop:
    lodsd          
    mov ecx, eax   
    xchg esi, edi ;menja value obeh registrov
    std
    lodsd ;naloži string       
    cld            
    sub edi, 4     
    stosd          
    mov eax, ecx   
    xchg esi, edi ;menja value obeh registrov
    std            
    add edi, 4     
    stosd         
    cld            
    cmp esi, edi ;primerja esi in edi in ce je  
    jl .loop     ;esi < edi reloopa

    mov esp, ebp
    pop ebp
    ret

_outpti:
    push ebp ;funkcija za delo s stack pointerji
    mov ebp, esp
    mov esi, [ebp+12]       
    mov eax, [ebp+8]              

.loop3:
    pushad
    push dword [esi] ;push esi vrednosti na stack
    push dword outpt ;format
    call _printf ;klic printf
    add esp, 8
    popad
    add esi, 4                
    dec eax 
    jnz .loop3 ;skoci if != 0 flag
    push row
    call _printf
    add esp, 4
    pop ebp
    ret

_main:
    mov ebp, esp ;for correct debugging
    push array ;push polja na stack za delo
    push dword[size] ;push velikosti polja na stack
    call _outpti ;klic funkcije
    add esp, 8
    push array ;push polja na stack za delo
    push dword[size] ;push velikosti polja
    call _function ;klic funkcije
    add esp, 8
    push array ;spet isto
    push dword[size] ;spet isto
    call _outpti ;call za output
    add esp, 8
    ret