import sys
import json
import matplotlib.pyplot as plt
import numpy as np
from PIL import Image
import os


def calculate_mean(data):
    return np.mean(data)


current_dir = os.path.dirname(os.path.abspath(__file__))
parent_dir = os.path.dirname(current_dir)
collisions = os.path.join(parent_dir, 'NFDriveAI\\Assets\\Learning\\Results\\TrainingResults.json')
mean = os.path.join(parent_dir, 'NFDriveAI\\Assets\\Learning\\Results\\MediaCollisioni.json')
totCollisions = os.path.join(parent_dir, 'NFDriveAI\\Assets\\Learning\\Results\\CollisioniGiro.json')

print(f"Parametri inseriti: {len(sys.argv) - 1}")

if len(sys.argv) > 4 or (4 > len(sys.argv) > 1):
    print(
        "Errore input. Inserisci path collisioniPerGiro, path Media e path Collisioni, oppure non inserire parametri.")
else:
    if len(sys.argv) == 4:
        collisions = sys.argv[1]
        mean = sys.argv[2]
        totCollisions = sys.argv[3]

    with open(collisions) as f:
        data = f.read()
    collisionsJs = json.loads(data)

    with open(mean) as f:
        data = f.read()
    meanJs = json.loads(data)

    with open(totCollisions) as f:
        data = f.read()
    totJs = json.loads(data)

    plt.figure(figsize=(10, 5))
    numero_giri = [eval(i) for i in list(collisionsJs.keys())]
    collisioni = list(collisionsJs.values())

    media_aritmetica = calculate_mean(collisioni)

    plt.plot(numero_giri, collisioni, linestyle='-', color='b', label='Collisioni per giro')
    plt.axhline(y=media_aritmetica, color='r', linestyle='--', label='Media aritmetica')

    plt.title('Numero di Collisioni per giro')
    plt.xlabel('Giro')
    plt.ylabel('Collisioni')
    plt.legend()
    plt.grid(True)
    plt.savefig('Collisioni.jpg')
    Image.open('Collisioni.jpg')

    plt.figure(figsize=(10, 5))
    numero_giri = np.arange(0, len(meanJs))
    plt.plot(numero_giri, meanJs, linestyle='-', color='b')

    plt.title('Media delle collisioni')
    plt.xlabel('Giro')
    plt.ylabel('Media collisioni')
    plt.grid(True)
    plt.savefig('media.jpg')
    Image.open('media.jpg')

    plt.figure(figsize=(10, 5))
    numero_giri = np.arange(0, len(totJs))
    plt.plot(numero_giri, totJs, linestyle='-', color='b')

    plt.title('Collisioni')
    plt.xlabel('Giro')
    plt.ylabel('Collisioni')
    plt.grid(True)
    plt.savefig('tot.jpg')
    Image.open('tot.jpg')

    plt.show()
