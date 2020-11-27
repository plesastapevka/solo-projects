#include <stdio.h>
#include <stdlib.h>
#include <sys/mman.h>
#include <fcntl.h>
#include <pthread.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/semaphore.h>
#include <stdbool.h>
#define CHILD_PROCS 2
//#include <wait.h>

void printHelp(int argc, char** argv);
void submergeSortSimple(int* array, int min1, int max1, int min2, int max2);
void submergeSortProc(int* array, int min1, int max1, int min2, int max2);
void submergeSortThread(int* array, int min1, int max1, int min2, int max2);
void mergeSort(int *array, int min, int max, void(*submergeSort)(int *, int, int, int, int));
void merge(int *arr,int min,int mid,int max);

struct threadparameters {
    int* arr;
    void(*submergeSort)(int *, int, int, int, int);
    int min1;
    int max1;
};

int thread_count = 0;
int threads = 2;

pthread_mutex_t count_mutex;
pthread_cond_t count_threshold_cv;
bool execute_only_once = true;
bool thread_limit_reached = false;
sem_t *sem_ref;

void deleteSemaphore(const char *ime){
    if(sem_unlink(ime)==-1){
        perror("Error: Could not delete semaphore\n");
        exit(-1);
    }
}

void *mergeSortHelp(void *threadarg){
    printf("Thread: %d\n", pthread_self());
    pthread_mutex_lock(&count_mutex);
    thread_count++;

    pthread_mutex_unlock(&count_mutex);

    struct threadparameters *my_data;
    my_data = (struct threadparameters *) threadarg;

    mergeSort(my_data->arr, my_data->min1, my_data->max1, my_data->submergeSort);

    pthread_exit(NULL);
}

// preprosta implementacija mergeSort rekurzije,
// samo klicemo margeSort za levo in desno polovico
// v istem procesu/isti niti
void submergeSortSimple(int* array, int min1, int max1, int min2, int max2){
    mergeSort(array, min1, max1, submergeSortSimple);
    mergeSort(array, min2, max2, submergeSortSimple);
}

// TODO: funkcija ki paralelizira sortiranje z uporabo procesov
// za preprosto paralelizacijo samo izvedemo vsak klic mergeSort
// funkcije v svojem procesu, in počakamo, da se klica zaključita

void submergeSortProc(int* array, int min1, int max1, int min2, int max2){
    printf("Sorting using processes\n");
    int i, sem_val = 0;
    int min[CHILD_PROCS] = { min1, min2 };
    int max[CHILD_PROCS] = { max1, max2 };
    for(i = 0; i < CHILD_PROCS; i++){
        sem_getvalue(sem_ref, &sem_val);
        if(sem_trywait(sem_ref)==-1){
            mergeSort(array, min[i], max[i], submergeSortSimple);
        }else{
            switch(fork()){
                case -1:
                    perror("Error: Could not create new process\n");
                    exit(-1);
                case 0:
                    mergeSort(array, min[i], max[i], submergeSortProc);
                    _exit(0);
            }
        }
    }
    for(i = 0; i < CHILD_PROCS; i++) {
        if(sem_val != 0){
            wait(NULL);
        }
    }
    return;
}

// TODO: funkcija, ki paralelizira sortiranje z uporabo niti
// za preprosto paralelizacijo samo izvedemo vsak klic mergeSort
// funkcije v svoji niti, in počakamo, da se klica zaključita
void submergeSortThread(int* array, int min1, int max1, int min2, int max2) {
    if(thread_count > threads) {
        mergeSort(array, min1, max1, submergeSortSimple);
        mergeSort(array, min2, max2, submergeSortSimple);
    } else {
        int ret_code, i;
        void* status_quo;

        struct threadparameters thread1;
        thread1.submergeSort = submergeSortThread;
        thread1.arr = array;
        thread1.min1 = min1;
        thread1.max1 = max1;

        struct threadparameters thread2;
        thread2.submergeSort = submergeSortThread;
        thread2.arr = array;
        thread2.min1 = min2;
        thread2.max1 = max2;

        struct threadparameters data[CHILD_PROCS] = { thread1, thread2 };
        pthread_t active_threads[CHILD_PROCS];

        pthread_attr_t params;
        pthread_attr_init(&params);
        pthread_attr_setdetachstate(&params, PTHREAD_CREATE_JOINABLE);

        for( i = 0; i < CHILD_PROCS; i++) {
            //printf("Thread created\n");
            ret_code = pthread_create(&active_threads[i], &params, mergeSortHelp, (void *)&data[i]);
            if (ret_code) {
//                printf("Error: pthread_create() returned %d\n", ret_code);
                exit(-1);
            } else {
//                printf("Success: pthread_create() returned %d\n", ret_code);
            }
        }

        pthread_attr_destroy(&params);

        for( i = 0; i < CHILD_PROCS; i++) {
            ret_code = pthread_join(active_threads[i], &status_quo);
            if (ret_code) {
                printf("Error: pthread_join() returned %d\n", ret_code);
                exit(-1);
            } else {
                printf("Success: pthread_join() returned %d\n", ret_code);
            }
        }
    }
    return;
}

// mergeSort in merge funkciji
// ti dve izvajata dejansko sortiranje

// void mergeSort(int *array, int min, int max, void(*submergeSort)(int *, int, int, int, int) )
//
// int *array
//   kazalec na tabelo števil, ki jih urejamo
//
// int min, int max
//   indeks prvega in zadnjega števila v tabeli,
//   označujeta interval, ki ga sortiramo
//
// void (*submergeSort)(int *array, int min1, int max1, int min2, int max2)
//   kazalec na funkcijo, ki naj kliče mergeSort za dva podintervala
//   in vrne, ko sta oba intervala sortirana
void mergeSort(int *array, int min, int max, void(*submergeSort)(int *, int, int, int, int) ){
    int mid;
    if(min < max){
        mid=(min+max)/2;

        submergeSort(array, min, mid, mid+1, max);

        merge(array, min, mid, max);
    }
}

// void merge(int *arr, int min,int mid,int max)
//
// int *arr
//   kazalec na tabelo
//
// int min, int mid, int max
//   indeksi na del tabele, ki jih je potrebno združiti
//   min je začetni indeks prve podtabele, mid je zadnji indeks
//   prve podtabele in max je zadnji indeks druge podtabele
//
// metoda zdruzi dve sosednji sortirani podtabeli,
// tako da je nova podtabela tudi sortirana
void merge(int *arr, int min,int mid,int max)
{
    // drugi korak algoritma mergeSort
    int *tmp = malloc((max-min+1)*sizeof(int));
    int i,j,k,m;
    j=min;
    m=mid+1;
    for(i=min; j<=mid && m<=max ; i++)
    {
        if(arr[j]<=arr[m])
        {
            tmp[i-min]=arr[j];
            j++;
        }
        else
        {
            tmp[i-min]=arr[m];
            m++;
        }
    }
    if(j>mid)
    {
        for(k=m; k<=max; k++)
        {
            tmp[i-min]=arr[k];
            i++;
        }
    }
    else
    {
        for(k=j; k<=mid; k++)
        {
            tmp[i-min]=arr[k];
            i++;
        }
    }
    for(k=min; k<=max; k++)
        arr[k]=tmp[k-min];

    free(tmp);
}


int main(int argc, char *argv[]) {
    #define NO_PAR 0
    #define PROC_PAR 1
    #define THREAD_PAR 2
    int technique= NO_PAR;
    void (*submergeSortFun)(int *, int, int, int, int);
    submergeSortFun = submergeSortSimple;
    while(1) {
        int c, n;
        c = getopt(argc, argv, ":ptn");
        if(c==-1){
            break;
        }
        switch(c){
            case 'p':
                technique = PROC_PAR;
                submergeSortFun = submergeSortProc;
                break;
            case 't':
                technique = THREAD_PAR;
                submergeSortFun = submergeSortThread;
                break;
            case 'n':
                for(; optind < argc; optind++){
                    threads = atoi(argv[optind]);
                    printf("Using %d threads/processes\n", threads);
                }
                break;
            default:
//                printHelp(argc, argv);
                return 0;
        }
    }
    
    int i;
    int size = 1000000;
    int *arr = NULL;
//    if(optind >= argc){
//        printHelp(argc, argv);
//        return -1;
//    }

//    size = atoi(argv[optind]);

    // TODO: inicializacija za razlicne tehnike
    switch(technique){
        case NO_PAR:
            arr = malloc(sizeof(int)*size);
            break;

        case PROC_PAR:
            arr = mmap(NULL, sizeof(int)*size, PROT_READ | PROT_WRITE,
                       MAP_SHARED | MAP_ANONYMOUS,-1, 0);
            if(arr == MAP_FAILED) {
                perror("Error: Could not mirror memory\n");
                return -1;
            }
            // semaphore init
            sem_ref = sem_open("/semaphore", O_RDWR|O_CREAT|O_EXCL, 0660, threads);
            if(sem_ref==SEM_FAILED) {
                perror("Error: Could not open semaphore\n");
                return -1;
            }
            break;

        case THREAD_PAR:
            arr = malloc(sizeof(int)*size);
            // initialization of mutex
            pthread_mutex_init(&count_mutex, NULL);
            pthread_cond_init (&count_threshold_cv, NULL);
            break;
    }

    int filedesc = open("rand_stevila.bin", O_RDONLY);
    char buffer[101];
    for(i=0; i < size; i+=1){
        // preberi binarne vrednosti
        read(filedesc, &arr[i], 4);
    }

//    for(i=0; i<size; i++){
//        printf("%d ",arr[i]);
//    }
//    printf("\n");

    mergeSort(arr, 0, size-1, submergeSortFun);

//    for(i=0; i<size; i++){
//        printf("%d ",arr[i]);
//    }
//    printf("\n");

    // TODO: ciscenje za razlicnimi tehnikami
    switch(technique){
        case NO_PAR:
            free(arr);
            break;

        case PROC_PAR:
            if(munmap(arr,sizeof(int)*size)  == -1) {
                perror("Error: Could not remove image\n");
            }
            if(sem_close(sem_ref) == -1) {
                perror("Error: Problem with semaphore\n");
            }

            const char* sem_char= malloc(sizeof(char)*10);
            sem_char = "/semaphore";
            deleteSemaphore(sem_char);
            break;

        case THREAD_PAR:
            free(arr);

            if(pthread_mutex_destroy(&count_mutex) != 0) {
                perror("Error: Could not remove mutex\n");
            }
            break;
    }
    pthread_exit(NULL);
    return 0;
}

void printHelp(int argc, char** argv){
    printf("uporaba\n");
    printf("%s <opcija> <n>\n",argv[0]);
    printf("\tn je število celih števil prebranih z standardnega vhoda\n");
    printf("\tfunkcije prebere n*4 bajtov v tabelo in jih sortira\n");
    printf("opcije:\n");
    printf("-p\n");
    printf("\tparalelizacija s pomočjo procesov\n");
    printf("-t\n");
    printf("\tparalelizacija s pomočjo niti\n");
}
