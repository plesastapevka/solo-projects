#!/bin/bash
for file in ./input_data/*
do
    virtualenv/bin/python3 vaja02.py < $file
done
