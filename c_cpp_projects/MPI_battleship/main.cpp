//
// Created by Urban Vidovič on 07/12/2020.
//

#include <iostream>
#include <mpi.h>
#include <fstream>
#include <unistd.h>

#define GRID_SIZE 20
#define SHIPS 10
#define MAX_SHIP_SIZE 5
#define TIME_TO_SLEEP 1

void create_html_table(int score[4], int** grid, int size, bool finished);
int** alloc_grid(int size);
int generate_battlefield(int** grid, int size);
void free_grid(int** grid);
void print_matrix(int** mtrx, int size);
bool check_if_hit_and_update(int** grid, const int move[2], int rank, int &hits_left);
void make_move(int** grid, int* arr);
void print_scores(int* arr);

enum orientation {
    vert,
    hor
};

void create_html_table(int score[4], int** grid, int size, bool finished) {
    std::ofstream myfile("/home/urban/html/index.html");
    if (!myfile.is_open()) std::cout << "Error opening file to write.\n";
    std::string output = "";
    output = "<!DOCTYPE html><html><head><style>table {border-collapse: collapse; }table, th,"
             "td {border: 1px solid black;} td.container > div {width: 100%;height:"
             "100%;overflow:hidden;}td.container {height: 22px; width:22px;} </style><meta http-"
             "equiv=\"refresh\" content=\"1\"></head><body><h2>MPI Battleships</h2><table>\n";

    for (int i = 0; i < size; i++) {
        output += "<tr>\n";
        for (int j = 0; j < size; j++) {
            if (grid[i][j] == 4) {
                output += R"(<td bgcolor="green" class="container")";
            } else if (grid[i][j] == 3) {
                output += R"(<td bgcolor="red" class="container")";
            } else if (grid[i][j] == 2) {
                output += R"(<td bgcolor="grey" class="container")";
            } else if (grid[i][j] == 1) {
                output += R"(<td bgcolor="blue" class="container")";
            } else if (grid[i][j] == -1) {
                output += R"(<td bgcolor="black" class="container")";
            } else output += "<td class=\"container\"";

            if (grid[i][j] != -1 && grid[i][j] != 0) output += ">" + std::to_string(grid[i][j]) + "</td>\n";
            else output += "</td>\n";
        }
        output += "</tr>\n";
    }
    output += "</table>\n";
    output += "<div>\n";
    output += "<p> <span style=\"color:blue;\">Player 1</span> score: " + std::to_string(score[0]) + "</p>";
    output += "<p> <span style=\"color:grey;\">Player 2</span> score: " + std::to_string(score[1]) + "</p>";
    output += "<p> <span style=\"color:red;\">Player 3</span> score: " + std::to_string(score[2]) + "</p>";
    output += "<p> <span style=\"color:green;\">Player 4</span> score: " + std::to_string(score[3]) + "</p>";
    if (finished) {
        int winner = 0;
        for (size_t j = 0; j < 4; ++j) {
            if (score[j] >= winner) winner = j;
        }
        output += "<h2> Player " + std::to_string(winner) + " wins!</h2>";
    }
    output += "</div>";
    output += "</body>\n</html>";
    myfile << output;
    myfile.close();
}

int** alloc_grid(int size) {
    int *data = (int*)malloc(size * size * sizeof(int));
    int **array= (int**)malloc(size * sizeof(int*));
    for (int i=0; i< size; i++)
        array[i] = &(data[size * i]);

    return array;
}

int generate_battlefield(int** grid, int size) {
    for (size_t i = 0; i < size; ++i) {
        for (size_t j = 0; j < size; ++j) {
            grid[i][j] = 0;
        }
    }
    orientation orient;
    int ship_size, full_tiles = 0;
    int x, y;
    bool tiles_ok, placed;
    for (size_t i = 0; i < SHIPS; ++i) {
        orient = static_cast<orientation>(rand() % 2);
        ship_size = (i % MAX_SHIP_SIZE) + 1;
        placed = false;
        while (!placed) {
            tiles_ok = true;
            x = (rand() % size) - ship_size;
            y = (rand() % size) - ship_size;
            if (x < ship_size) x += ship_size;
            if (y < ship_size) y += ship_size;
            if (orient == vert) {
                for (size_t j = x; j < x + ship_size; ++j) {
                    if (grid[j][y] == -1) tiles_ok = false;
                }
                if (tiles_ok) {
                    for (size_t j = x; j < x + ship_size; ++j) {
                        grid[j][y] = -1;
                        full_tiles++;
                    }
                    placed = true;
                }
            } else {
                for (size_t j = y; j < y + ship_size; ++j) {
                    if (grid[x][j] == -1) tiles_ok = false;
                }
                if (tiles_ok) {
                    for (size_t j = y; j < y + ship_size; ++j) {
                        grid[x][j] = -1;
                        full_tiles++;
                    }
                    placed = true;
                }
            }
        }
    }
    return full_tiles;
}

void free_grid(int** grid) {
    free(grid[0]);
    free(grid);
}

void print_matrix(int** mtrx, int size) {
    for (size_t i = 0; i < size; ++i) {
        for (size_t j = 0; j < size; ++j) {
            std::cout << mtrx[i][j] << " ";
        }
        std::cout << "\n";
    }
    std::cout << std::flush;
}

bool check_if_hit_and_update(int** grid, const int move[2], int rank, int &hits_left) {
    bool hit = false;
    if (grid[move[0]][move[1]] == -1) {
        hits_left--;
        hit = true;
    }
    grid[move[0]][move[1]] = rank;

    return hit;
}

void make_move(int** grid, int* arr) {
    int x;
    int y;
    bool ok = false;
    while (!ok) {
        x = rand() % GRID_SIZE;
        y = rand() % GRID_SIZE;
        if (grid[x][y] < 1) {
            ok = true;
        }
    }
    arr[0] = x;
    arr[1] = y;
}

void print_scores(int* arr) {
    std::cout << "Global score: { ";
    for (size_t i = 0; i < 4; ++i) {
        std::cout << arr[i] << " ";
    }
    std::cout << "}\n";
}

int main() {
    MPI_Init(nullptr, nullptr);
    int rank, world_size;
    MPI_Comm_size(MPI_COMM_WORLD, &world_size);
    MPI_Comm_rank(MPI_COMM_WORLD, &rank);
    int** grid;
//    int** hits;
    int hits_left;
    grid = alloc_grid(GRID_SIZE);
    if (rank == 0) {
        srand((unsigned)time(nullptr));
        std::cout << "Initiated MPI World with " << world_size << " clusters\n";
        std::cout << "Master starting grid generation ...\n";
        hits_left = generate_battlefield(grid, GRID_SIZE);
        std::cout << "Battlefield generate.\n";
        print_matrix(grid, GRID_SIZE);

        int move[2];
        int scores[4] = {0};
        int rounds = 0;
        bool hit = true, game_over = false;
        create_html_table(scores, grid, GRID_SIZE, false);
        std::cout << "Game starting in 3 seconds ...\n";
        sleep(3);
        while (!game_over) {
            for (size_t i = 1; i < world_size; ++i) { // one round
                if (hits_left == 0) break;
                hit = true;
                while (hit) {
                    MPI_Send(&grid[0][0], GRID_SIZE * GRID_SIZE, MPI_INT, i, 0, MPI_COMM_WORLD);
                    MPI_Recv(&move, 2, MPI_INT, i, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
                    hit = check_if_hit_and_update(grid, move, i, hits_left);
                    if (hit) {
                        scores[i - 1]++;
                        if (hits_left == 0) {
                            break;
                        }
                    }
                    create_html_table(scores, grid, GRID_SIZE, false);
                    sleep(TIME_TO_SLEEP);
                }
            }
            rounds++;
            std::cout << "After " << rounds << " rounds stats:\n";
            print_scores(scores);
            std::cout << "Hits left: " << hits_left << "\n";
            std::cout << "\n";
            if (hits_left == 0) {
                for (int i = 1; i < world_size; ++i) {
                    MPI_Send(&i, 0, MPI_INT, i, 33, MPI_COMM_WORLD);
                }
                game_over = true;
                std::cout << "Game over!\n";
                create_html_table(scores, grid, GRID_SIZE, true);
            }
        }
    } else {
        srand((unsigned)time(nullptr));
        MPI_Status status;
        int move[2];
        while (true) {
            MPI_Recv(&grid[0][0], GRID_SIZE * GRID_SIZE, MPI_INT, 0, MPI_ANY_TAG, MPI_COMM_WORLD, &status);
            if (status.MPI_TAG == 33) {
                break;
            }
            make_move(grid, move);
            MPI_Send(&move, 2, MPI_INT, 0, 0, MPI_COMM_WORLD);
        }
    }
    free_grid(grid);
    MPI_Finalize();
    return 0;
}