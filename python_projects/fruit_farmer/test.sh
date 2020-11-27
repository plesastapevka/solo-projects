#!/bin/bash
for file in ./input_data/*
do
    python sadjar_v1.py < $file
done
