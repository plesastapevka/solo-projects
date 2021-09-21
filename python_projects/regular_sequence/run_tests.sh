#!/usr/bin/env bash

source venv/bin/activate

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 10 -l 4 -t 2
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 10 -l 4 -t 2

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 10 -l 8 -t 2
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 10 -l 8 -t 2

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 15 -l 4 -t 2
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 15 -l 4 -t 2

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 15 -l 8 -t 2
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 15 -l 8 -t 2

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 20 -l 4 -t 2
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 4 -t 2

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 20 -l 8 -t 2
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 8 -t 2

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 10 -l 4 -t 3
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 4 -t 3

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 10 -l 8 -t 3
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 10 -l 8 -t 3

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 15 -l 4 -t 3
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 15 -l 4 -t 3

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 15 -l 8 -t 3
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 15 -l 8 -t 3

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 20 -l 4 -t 3
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 4 -t 3

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 20 -l 8 -t 3
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 8 -t 3

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 10 -l 4 -t 4
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 10 -l 4 -t 4

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 10 -l 8 -t 4
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 10 -l 8 -t 4

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 15 -l 4 -t 4
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 15 -l 4 -t 4

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 15 -l 8 -t 4
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 15 -l 8 -t 4

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 20 -l 4 -t 4
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 4 -t 4

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 20 -l 8 -t 4
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 8 -t 4

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 10 -l 4 -t 5
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 10 -l 4 -t 5

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 10 -l 8 -t 5
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 10 -l 8 -t 5

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 15 -l 4 -t 5
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 15 -l 4 -t 5

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 15 -l 8 -t 5
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 15 -l 8 -t 5

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 20 -l 4 -t 5
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 4 -t 5

echo "Greedy"
python main.py -f data/DNK.txt -m greedy -n 20 -l 8 -t 5
echo "Branch"
python main.py -f data/DNK.txt -m branch -n 20 -l 8 -t 5

deactivate
