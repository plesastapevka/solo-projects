#include <mpi.h>
#include <iostream>
#include <chrono>
#include <cmath>

#define SLAVES 4
#define LO 270.505
#define UP 270.515

int main(int argc, char** argv) {
    MPI_Init(nullptr, nullptr);
    int rank;
    int world;
    MPI_Comm_rank(MPI_COMM_WORLD, &rank);
    MPI_Comm_size(MPI_COMM_WORLD, &world);

    float num;
    float sum{0};
    size_t count{0};
    if (rank == 0) {
        std::srand(time(nullptr));
        while (sum < LO || sum > UP) {
            num = ((float)rand()/(float)(RAND_MAX)) * (float)180.0;
            // Generate num and send it to all slaves
            for (size_t i{1}; i <= SLAVES; ++i) {
                MPI_Send(&num, 1, MPI_FLOAT, i, 0, MPI_COMM_WORLD);
            }
            // Receive num back from all slaves and count each pass and sum
            for (size_t i{1}; i <= SLAVES; ++i) {
                MPI_Recv(&num, 1, MPI_FLOAT, i, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
                sum = std::fmod(sum, (float)360) + num;
                count++;
            }
        }
        for (size_t i{1}; i <= SLAVES; ++i) {
            MPI_Send(&i, 0, MPI_INT, i, 33, MPI_COMM_WORLD);
        }
        std::cout << "After " << count << " PingPongs, the value was: " << sum << "\n";
    } else {
        MPI_Status stat;
        int tag;
        while (true) {
            MPI_Recv(&num, 1, MPI_FLOAT, 0, MPI_ANY_TAG, MPI_COMM_WORLD, &stat);
            if (stat.MPI_TAG == 33) break;
            MPI_Send(&num, 1, MPI_FLOAT, 0, 0, MPI_COMM_WORLD);
        }
    }

    MPI_Finalize();
    return 0;
}