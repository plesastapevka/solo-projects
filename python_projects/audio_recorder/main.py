from audio_rec import AudioRecorder
import matplotlib.pyplot as plt
import numpy


def plot_frames(frames):
    fig = plt.figure()
    s = fig.add_subplot(111)
    amplitude = numpy.frombuffer(frames, numpy.int16)
    s.plot(amplitude)
    fig.savefig('t.png')


def main():
    recorder = AudioRecorder()
    recorder.record()
    recorder.save_to_file(FILE_NAME="result.wav")
    frames = recorder.get_recording_string()
    plot_frames(frames)


if __name__ == "__main__":
    main()
