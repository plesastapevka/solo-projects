class Utils:
    @staticmethod
    def read_file(path):
        data = ""
        try:
            file = open(path, "r")
            data = file.read()
            file.close()
        except IOError:
            raise IOError

        return data

    @staticmethod
    def write_file(path, data):
        try:
            file = open(path, "w")
            for i in data:
                file.write(" ".join(map(str, i)) + "\n")
            print(f"Results written to {path}")
            file.close()

        except IOError:
            raise IOError
