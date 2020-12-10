from selenium import webdriver
import json
import io
import csv
import sys


class Entry(object):
    def __init__(self, title, year, rating):
        self.title = title
        self.year = year
        self.rating = rating

    @staticmethod
    def entry_to_json(entry_list, file_name):
        # convert entry list to json file
        print("Writing JSON data to file ...")
        with open(file_name, 'w') as outfile:
            outfile.write("[")
            colon = False
            for entry in entry_list:
                if colon:
                    outfile.write(", ")
                jsonString = json.dumps(entry.__dict__, sort_keys=True)
                outfile.write(jsonString)
                colon = True
            outfile.write("]")

    @staticmethod
    def json_to_entry(file_name):
        # read from json file and return a generator
        counter = 0
        try:
            with open(file_name, mode='r') as f:
                for line in f:
                    counter += 1
                    yield json.dumps(json.loads(line), sort_keys=True)
                    if counter == 100:
                        break
        except:
            print("Error reading file!")

    @staticmethod
    def entry_to_csv(entry_list, file_name):
        # convert entry list to csv file
        print("Writing CSV data to file ...")
        with open(file_name, 'w', newline='', encoding='utf8') as f:
            writer = csv.writer(f, delimiter=',')
            for entry in entry_list:
                writer.writerow(list(entry))

    @staticmethod
    def csv_to_entry(file_name):
        # read from csv file and return a generator
        counter = 0
        try:
            with open(file_name, 'r') as f:
                print("Reading csv data ...")
                reader = csv.reader(f, delimiter=',')
                for row in reader:
                    counter += 1
                    yield Entry(row[0], row[1], row[2])
                    if counter == 100:
                        break
        except csv.Error:
            print("Error reading file!")

    def __iter__(self):
        return iter([self.title, self.year, self.rating])

    def __repr__(self):
        return "\n" + str(self)

    def __str__(self):
        return "Title: " + self.title + "; Year: " + str(self.year) + "; Rating: " + str(self.rating)
