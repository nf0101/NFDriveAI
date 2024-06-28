<p align="center"><img src="?raw=true" height="400"></p>

# NFDriveAI

NFDriveAI è un progetto in Unity che implementa un sistema di intelligenza artificiale con apprendimento per rinforzo, utilizzando il Q-Learning. L'obiettivo è permettere all'agente di guidare un'automobile lungo diverse piste.

## Organizzazione respository
NFDriveAI: Progetto Unity. Manitene la tipica gerarchia dei file di Unity. Nel subfolder Learning sono presenti la Q-Table di un agente addestrato e gli ultimi risultati ottenuti.
PlotData: Script data visualization

## Installazione e Utilizzo
Per utilizzare il progetto seguire i seguenti passaggi:
### Requisiti:
Unity Editor, possibilmente versione 2022.3.2f1
### Installazione
* Scaricare lo Unity Package
* Creare un progetto 2D
* Importare il package tramite Asset > Import package > Importare il package scaricato
### Utilizzare il progetto
Nel package scaricato saranno già presenti piste e agenti. Nell'inspector sarà possibile modificare i parametri degli agenti.
Se viene spuntata la casella Training, l'agente aggiornerà la q-table durante la simulazione.
Se viene spuntata la casella Trained l'agente caricherà la q-table presente nel path denominato Q-Table Path.
Se Trained è spuntata ma Training no, l'agente avrà un exploration rate di 0 e seguirà solo la Q-Table. Questa modalità è utilizzata nel testing.
Se sono spuntate entrambe, si potranno affinare le conoscenze dell'agente.
Nella GUI sono presenti due pulsanti:
Speed: aumenta il clock dell'engine di 100. Permette di addestrare più velocemente senza modificare il comportamento dell'agente.
Save results: salverà i risultati delle collisioni in un file json, il path viene registrato nella console e può essere modificato alla voce Results Path dell'inspector.
Premendo il tasto O, sarà possibile salvare la Q-Table, nel path specificato nell'apposita voce dell'inspector.
Premendo il tasto L è possibile caricare la Q-Table specificata.
Una volta configurato l'agente è possibile avviare la simulazione premendo il tasto Play.

