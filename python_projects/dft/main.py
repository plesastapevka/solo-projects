from audio_rec import AudioRecorder
import numpy as np
import utils
import matplotlib.pyplot as plt


def dft(frames):
    transformed_frames = np.fft


def main():
    # recorder = AudioRecorder()
    # print("Now recording ...")
    # recorder.record()
    # print("Stopped recording.")
    # recorder.save_to_file(FILE_NAME="result.wav")
    # frames = recorder.get_recording_string()
    # utils.plot_frames(frames)
    x, y = utils.generate_sine(2)
    utils.dft(x, y)


if __name__ == "__main__":
    main()
