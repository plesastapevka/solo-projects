import pygame


def input():
    # checking for key presses and close button presses and pause-continue funcionality
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            quit()
        if event.type == pygame.MOUSEMOTION:
            return pygame.mouse.get_pos()