import math
import pygame
import manager
import colors


class Bone:
    def __init__(self):
        self.bone_length = manager.bone_length
        self.joint = (0, 0)
        self.relative_angle = 0
        self.effector = (0, 0)

        self.effector = self.ComputeEffector(0)

    def update(self, _joint, _total_angle):
        self.joint = _joint
        self.effector = self.ComputeEffector(_total_angle)

    def ComputeEffector(self, _total_angle):
        theta = (_total_angle + self.relative_angle) * math.pi / 180
        return self.joint[0] + (self.bone_length * math.cos(theta)), self.joint[1] + (
                    self.bone_length * math.sin(theta))

    # draw the snake on the pygame screen
    def draw(self, screen):
        # draw bone
        pygame.draw.line(screen, colors.boneColor, self.joint, self.effector, manager.bone_width)

        # draw joint
        pygame.draw.circle(screen, colors.jointColor, (self.joint[0], self.joint[1]), manager.joint_radius)

        return screen
