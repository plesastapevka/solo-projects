from audio_rec import AudioRecorder


def main():
    recorder = AudioRecorder()
    recorder.record()
    recorder.save_to_file(FILE_NAME="result.wav")
    frames = recorder.get_recording()


if __name__ == "__main__":
    main()
