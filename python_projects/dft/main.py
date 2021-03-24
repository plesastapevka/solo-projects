from audio_rec import AudioRecorder
import numpy as np
import utils
import matplotlib.pyplot as plt
import constants


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

    # Create ax for each graph
    fig, (ax1, ax2) = plt.subplots(2)

    # Get value array
    amplitudes = np.frombuffer(recording, np.int16)
    time = np.linspace(0, len(amplitudes) / constants.SAMPLE_RATE, num=len(amplitudes))

    # FFT
    xf, yf = utils.dft(recording)

    # Plotting
    ax1.plot(time, amplitudes)
    ax1.set(xlabel="time (s)")
    ax1.set(ylabel="amplitude (???)")

    ax2.plot(xf, np.abs(yf))
    ax2.set(xlabel="frequency (Hz)")
    ax2.set(ylabel="amplitude (???)")
    plt.show()
    # x, y = utils.generate_sine(60)
    # Plot sinusoide
    # plt.plot(x, y)
    # plt.show()
    # utils.dft(x, y)
    # utils.dft_impl(y)
    # xf = fftfreq(N, 1 / constants.SAMPLE_RATE)
    # plt.stem(xf, ft_vals)
    # plt.show()


if __name__ == "__main__":
    main()
