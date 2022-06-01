import math
import manager


def distance(p1, p2):
    return math.sqrt(math.pow(p2[0] - p1[0], 2) + math.pow(p2[1] - p1[1], 2))


def angleBetweenPoints(p1, p2):
    return math.atan2(p2[1] - p1[1], p2[0] - p1[0]) * 180 / math.pi


class Armature:
    def __init__(self, _bones, _target):
        self.bones = _bones
        self.target = _target
        self.anchor = manager.anchor
        self.armature_length = self.armature_length_function()

    def armature_length_function(self):
        dist = 0
        for bone in self.bones:
            dist = dist + bone.bone_length
        return dist

    def err(self):
        return distance(self.get_armature_effector(), self.target)

    def get_armature_effector(self):
        return self.bones[len(self.bones) - 1].effector

    def set_armature_angles(self, _relative_angles_array):
        for i in range(len(self.bones)):
            self.bones[i].relative_angle = abs(_relative_angles_array[i] % 360)
        self.update_armature()

    def update_armature(self):
        # ===updates the positions of bones (joints, angles, and effectors)===
        _total_angle = 0
        # set joint of child bone to effector of previous bone, and update the current bone position
        for i in range(len(self.bones)):
            if i == 0:
                self.bones[i].update(self.anchor, _total_angle)
            else:
                self.bones[i].update(self.bones[i - 1].effector, _total_angle)
            _total_angle = _total_angle + self.bones[i].relative_angle

    def gradient_optimization_method(self):
        current_relative_angles = []
        for bone in self.bones:
            current_relative_angles.append(bone.relative_angle)

        iter = 0
        while self.err() > manager.d and iter < manager.max_iter:
            for i in range(len(current_relative_angles)):
                bonesA_relative_angles = list(current_relative_angles)
                bonesB_relative_angles = list(current_relative_angles)

                bonesA_relative_angles[i] += manager.g
                bonesB_relative_angles[i] -= manager.g

                self.set_armature_angles(bonesA_relative_angles)
                errA = self.err()
                self.set_armature_angles(bonesB_relative_angles)
                errB = self.err()

                gradient = errA - errB
                current_relative_angles[i] = current_relative_angles[i] - gradient
            self.set_armature_angles(current_relative_angles)
            iter = iter + 1

    def update(self, target_location):
        # compute Gradient optimization
        self.target = target_location

        self.gradient_optimization_method()
        # self.update_armature()
