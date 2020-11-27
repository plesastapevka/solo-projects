# Seminarska naloga - Enaki nizi (same-strings-checker)


## Problem description
Pri tej nalogi se bomo ukvarjali z nekoliko drugačno definicijo enakosti nizov. Niza, kista sestavljena iz enega samega znaka, sta enaka, če gre za enaka znaka. Za daljše nizedolžin3k(k∈N) pa velja, da sta niza enaka, če ju razbijemo na podnize dolžin3k−1(tj.na tri tretjine) in med dobljenimi podnizi obeh nizov obstaja tako popolno ujemanje, daso si podnizi v ujemajočih se parih enaki.

## Pseudocode
```
equalSubStrings(arr1,arr2):
    1. za vsaki string v arr1 pogledam ce obstaja enak string v arr2
    2. ce obstaja, oba zbrisem
    3. po while loopu, pogledam ce je arr1 prazen array
    4. ce je arr1 prazen (ce je arr1 prazen potem je tudi arr2 prazen), to pomeni da sta niza enaka in vrnem true
    5. ce pa arr1 ni prazen, pomeni da moremo nize iz polja arr1 in arr2 razbiti na 1/3 (tretjine)
    6. preden jih razbijemo moramo preveri ce jih sploh lahko razbijemo na 1/3 (to pomeni da je niz dolzine 3 ali vecji)
    7. ce smo jih lahko razbili potem ponovno klicemo funkcijo equalSubStrings
    6. drugace pa vrnemo false (to pomeni da smo prisli do listov drevesa oz. do posameznih charov npr. 'a')
```