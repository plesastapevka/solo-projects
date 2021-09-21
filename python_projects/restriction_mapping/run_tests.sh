#!/usr/bin/env bash

source venv/bin/activate

python main.py partial_digest data/DNK1.txt TAAT,GTAC,TCTAGT
python main.py partial_digest data/DNK1.txt TAAT,GTAC,GGTT
python main.py partial_digest data/DNK1.txt TCG
python main.py partial_digest data/DNK1.txt GTA
python main.py partial_digest data/DNK1.txt CAT
python main.py partial_digest data/DNK1.txt TTTTGT,CAT

deactivate
