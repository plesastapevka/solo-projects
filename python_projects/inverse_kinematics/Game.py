import colors as col
import time
from Canvas import *
from Bone import *
from Armature import *
import manager
from UserInput import *


class Game:

    def __init__(self):
        # pygame initialization
        self.armature = None
        self.bones = None
        self.target = None
        pygame.init()
        self.screen = pygame.display.set_mode((manager.width, manager.height))
        pygame.display.set_caption('vaja05-Inverse_kinematics')
        self.canvas = Canvas(manager.width, manager.height)

    def __del__(self):
        pygame.quit()

    def start(self):
        # create target
        self.target = (0, 0)
        # create bones for armature
        self.bones = []
        for i in range(manager.number_of_bones):
            self.bones.append(Bone())

        self.armature = Armature(self.bones, self.target)

    def run(self, msBetweenFrames):
        while True:
            self.update()
            # render current game state
            self.render()
            # speed of rendering
            time.sleep(msBetweenFrames)

    def update(self):
        mousePos = input()
        # if mouse moved we move the target and accordingly move the armature
        if mousePos is not None:
            self.target = mousePos

            self.armature.update(self.target)

    def render(self):
        # render canvas
        self.screen = setup(self.screen, col.canvasColor)
        # render target
        pygame.draw.circle(self.screen, col.targetColor, self.target, manager.target_radius)
        # render bones
        for bone in self.bones:
            self.screen = bone.draw(self.screen)

        pygame.display.update()


if __name__ == "__main__":
    _game = Game()

    _game.start()

    _game.run(0.01)

    del _game
    quit()
