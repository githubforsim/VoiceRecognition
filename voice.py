import vosk
import pyaudio
import json
import socket
import argparse

def reconnaissance_vocale_vosk(model_path):
    model = vosk.Model(model_path)

    recognizer = vosk.KaldiRecognizer(model, 16000)

    p = pyaudio.PyAudio()
    stream = p.open(format=pyaudio.paInt16, channels=1, rate=16000, input=True, frames_per_buffer=8000)

    print("En attente de la commande...")

    udp_client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    server_address = ('localhost', 12345)  # Utilisez l'adresse du serveur C#

    while True:
        udp_client.sendto("Type1:Ready".encode('utf-8'), server_address)
        data = stream.read(8000)
        if len(data) == 0:
            break
        if recognizer.AcceptWaveform(data):
            result = json.loads(recognizer.Result())
            text_result = result["text"]
            print(text_result)
            
            udp_client.sendto(("Type2:" + text_result).encode('utf-8'), server_address)

    stream.stop_stream()
    stream.close()
    p.terminate()
    udp_client.sendto("Type1:UnReady".encode('utf-8'), server_address)
    udp_client.close()

if __name__ == "__main__":
    try:
        parser = argparse.ArgumentParser(description='Script de reconnaissance vocal')
        parser.add_argument('model', type=str, help='path of model')

        args = parser.parse_args()
        reconnaissance_vocale_vosk(args.model)
    except:
        print("Something went wrong")
    t = input("wait")