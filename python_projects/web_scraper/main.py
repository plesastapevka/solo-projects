import web_module
from web_module import entry
import sys
import getopt


def help():
    print("usage:")
    print("\tmain.py [options]")
    print("options: ")
    print("\t-h \theadless mode")
    print("\t-f, --file <filename>\tspecify file")
    print("\t-j\tdata type json")
    print("\t-c\tdata type csv")
    print("\t-r\taction type - read")
    print("\t-w\taction type - write")
    print("\t-u, --url <url>\tURL to get data from")


def main():
    headless = False
    file_name = ""
    action = "none"
    file_type = "none"
    url = ""

    try:
        opts, args = getopt.getopt(sys.argv[1:], "Hhjcrwf:u:", ["file", "url"])
    except getopt.GetoptError:
        help()
        sys.exit(2)

    for opt, arg in opts:
        if opt == '-h':
            headless = True

        elif opt in ("-f", "--file"):
            file_name = arg

        elif opt in "-r":
            action = "read"

        elif opt in "-w":
            action = "write"

        elif opt in ("-u", "--url"):
            url = arg

        elif opt in "-j":
            file_type = "json"

        elif opt in "-c":
            file_type = "csv"

    if file_name == "" or action == "none" or file_type == "none":
        help()
        sys.exit(2)

    if action == "write":
        web = web_module.WebModule(url, headless)
        web.open()

        listData = web.list_by_xpath(".//tbody[@class='lister-list']/tr")
        print(listData)
        web.close()

    if action == "read":
        if file_type == "json":
            for jsonRow in entry.Entry.json_to_entry(file_name):
                print(jsonRow)
        else:
            for csvRow in entry.Entry.csv_to_entry(file_name):
                print(csvRow)
    else:
        if file_type == "json":
            entry.Entry.entry_to_json(listData, file_name)
        else:
            entry.Entry.entry_to_csv(listData, file_name)


if __name__ == "__main__":
    main()
