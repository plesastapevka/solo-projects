class Utils:
    @staticmethod
    def read_file(path):
        data = ""
        try:
            file = open(path, "r")
            data = file.read()
        except IOError:
            raise IOError

        return data

    @staticmethod
    def write_file(path, data):
        try:
            file = open(path, "w")
            file.write(data)

        except IOError:
            raise IOError
