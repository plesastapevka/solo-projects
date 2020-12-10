#  Class for working with the selenenium web driver
from selenium import webdriver
import time
from selenium.webdriver.firefox.firefox_binary import FirefoxBinary
from selenium.webdriver.firefox.options import Options
import entry
import os
import regex

class WebModule:
    def __init__(self, url, headless, driver_path=r"/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"):
        self.driver_path = driver_path
        self.url = url
        options = Options()
        if headless:
            options.headless = True

        # PROJECT_ROOT = os.path.abspath(os.path.dirname(__file__))
        # DRIVER_BIN = os.path.join(PROJECT_ROOT, "drivers/chromedriver")
        # options.binary_location = self.driver_path
        self.browser = webdriver.Safari()

    def open(self):
        self.browser.get(self.url)

    def print_source(self):
        print(self.browser.page_source)

    def close(self):
        self.browser.close()

    def list_by_xpath(self, selection):
        print("Started data fetching ...")
        entry_list = []
        list = self.browser.find_elements_by_xpath(selection)
        for elem in list:
            title = elem.find_elements_by_xpath(".//td[@class='titleColumn']/a")[0].text
            year = int(elem.find_elements_by_xpath(".//td[@class='titleColumn']/span")[0].text[1:5])
            rating = float(elem.find_elements_by_xpath(".//td/strong")[0].text)
            entry_list.append(entry.Entry(title, year, rating))
        print("Data fetched!")
        return entry_list
