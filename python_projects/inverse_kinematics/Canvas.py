def setup(screen, color):
    screen.fill(color)
    return screen


class Canvas:
    def __init__(self, width, height):
        self.height = height
        self.width = width
