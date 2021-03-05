import pyaudio as pya
import wave as wav
import os


def play_recording(FILE_NAME="output.wav"):
    os.system('afplay ' + FILE_NAME)


class AudioRecorder:
    def __init__(self, CHUNK=1024, FORMAT=pya.paInt16, CHANNELS=1, RATE=44100, RECORD_TIME=3):
        self.chunk = CHUNK
        self.format = FORMAT
        self.channels = CHANNELS
        self.rate = RATE
        self.record_time = RECORD_TIME
        self.recorder = pya.PyAudio()
        self.frames = []

    def record(self):
        stream = self.recorder.open(format=self.format,
                                    channels=self.channels,
                                    rate=self.rate,
                                    input=True,
                                    frames_per_buffer=self.chunk)
        print("Now recording ...")
        for i in range(0, int(self.rate / self.chunk * self.record_time)):
            data = stream.read(self.chunk)
            self.frames.append(data)

        print("Stopped recording.")

        # Destroy stream
        stream.stop_stream()
        stream.close()

    def get_recording(self):
        return self.frames

    def save_to_file(self, FILE_NAME="output.wav"):
        wf = wav.open(FILE_NAME, 'wb')
        wf.setnchannels(self.channels)
        wf.setsampwidth(self.recorder.get_sample_size(self.format))
        wf.setframerate(self.rate)
        wf.writeframes(b''.join(self.frames))
        wf.close()

    def destroy(self):
        self.recorder.terminate()
