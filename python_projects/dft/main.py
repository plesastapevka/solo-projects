from audio_rec import AudioRecorder
import numpy as np
import utils
import matplotlib.pyplot as plt


def record_sound():
    recorder = AudioRecorder()
    print("Now recording ...")
    recorder.record()
    print("Stopped recording.")
    recorder.save_to_file(FILE_NAME="result.wav")
    frames = recorder.get_recording_string()
    return frames


def main():
    recording = record_sound()
    utils.plot_frames(recording)
    utils.dft(0, recording)
    # x, y = utils.generate_sine(60)
    # Plot sinusoide
    # plt.plot(x, y)
    # plt.show()
    # utils.dft(x, y)
    # utils.dft_impl(y)


if __name__ == "__main__":
    main()
