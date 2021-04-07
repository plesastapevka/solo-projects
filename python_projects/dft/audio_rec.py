import pyaudio as pya
import wave as wav
import os
import constants


def play_recording(FILE_NAME="output.wav"):
    os.system('afplay ' + FILE_NAME)


class AudioRecorder:
    def __init__(self, CHUNK=constants.CHUNK, FORMAT=pya.paInt16, CHANNELS=1, RATE=constants.SAMPLE_RATE, RECORD_TIME=constants.DURATION):
        self.chunk = CHUNK
        self.format = FORMAT
        self.channels = CHANNELS
        self.rate = RATE
        self.record_time = RECORD_TIME
        self.recorder = pya.PyAudio()
        info = self.recorder.get_host_api_info_by_index(0)
        numdevices = info.get('deviceCount')
        for i in range(0, numdevices):
            if (self.recorder.get_device_info_by_host_api_device_index(0, i).get('maxInputChannels')) > 0:
                print("Input Device id ", i, " - ", self.recorder.get_device_info_by_host_api_device_index(0, i))
        self.frames = []

    def record(self):
        stream = self.recorder.open(format=self.format,
                                    channels=self.channels,
                                    rate=self.rate,
                                    input=True,
                                    frames_per_buffer=self.chunk)
        for i in range(int(self.rate / self.chunk * self.record_time)):
            data = stream.read(self.chunk)
            self.frames.append(data)

        # Destroy stream
        stream.stop_stream()
        stream.close()

    def get_recording_string(self):
        return b''.join(self.frames)

    def get_recording_list(self):
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
