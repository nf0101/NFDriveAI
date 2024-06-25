import sys
import json
import matplotlib.pyplot as plt
import numpy as np
from PIL import Image

# Dati di esempio
numero_giri = np.arange(1, 301)
numero_collisioni = np.linspace(50, 1, num=300).astype(int)
results = {}
js = {}
if len(sys.argv) > 2:
    print("Errore input")
    print(sys.argv)
else:
    with open(sys.argv[1]) as f:
        data = f.read()
    js = json.loads(data)
    print(js)
# Creazione del grafico
plt.figure(figsize=(10, 5))
print(js.keys())
numero_giri = [eval(i) for i in list(js.keys())]
print(numero_giri, js.values())
plt.plot(numero_giri, list(js.values()), linestyle='-', color='b')

# Aggiunta di titoli e etichette
plt.title('Numero di Collisioni per Giro')
plt.xlabel('Numero del Giro')
plt.ylabel('Numero di Collisioni')

# Aggiunta di una griglia
plt.grid(True)
plt.savefig('testplot.jpg')
Image.open('testplot.jpg')
# Visualizzazione del grafico

plt.show()



