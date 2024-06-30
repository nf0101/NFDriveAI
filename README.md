<p align="center"><img src="https://github.com/nf0101/NFDriveAI/blob/main/Slide.png?raw=true" width="1000"></p>

# NFDriveAI

NFDriveAI è un progetto in Unity che implementa un sistema di intelligenza artificiale con apprendimento per rinforzo, utilizzando il Q-Learning. L'obiettivo è permettere all'agente di guidare un'automobile lungo diverse piste.

## Organizzazione respository
<b>NFDriveAI:</b> Progetto Unity. Mantiene la tipica gerarchia dei file di Unity. </br>
* Nel subfolder <b>Learning</b> sono presenti la Q-Table di un agente addestrato e gli ultimi risultati ottenuti. </br>

<b>PlotData:</b> Script data visualization
* Per utilizzarlo, salvare i risultati dell'agente come spiegato più avanti.
* Se non vengono modificati i path di salvataggio, eseguire da CMD il comando <b>python main.py</b> nel folder PlotData. A questo punto verranno mostrati i grafici.
* Se i path vengono modificati, bisogna aggiungere al comando <b>python main.py</b> tre argomenti, che saranno i path dei tre file JSON, nell'ordine: 

## Installazione e Utilizzo
Per utilizzare il progetto seguire i seguenti passaggi:
### Requisiti:
<b>Unity Edito</b>r, possibilmente versione <b>2022.3.2f1</b> per evitare incompatibilità.
### Installazione
* Scaricare lo Unity Package dalla sezione release
* Creare un progetto 2D
* Importare il package tramite <b>Asset > Import package > Custom package...</b> > Selezionare il package scaricato
### Utilizzare il progetto
Nel package scaricato saranno già presenti piste e agenti.</br>
Di default, quando il package viene importato, si avrà la terza pista, con tutte le impostazioni per il testing e l'agente caricherà la Q-Table.</br>
Per utilizzare le altre piste basta attivarle dalla Hierarchy e riposizionare la Main Camera e se necessario l'agente.</br>
Nell'inspector sarà possibile modificare i parametri degli agenti.</br>
Il funzionamento dei parametri è il seguente:</br>
* Se viene spuntata la casella <b>Training</b>, l'agente aggiornerà la q-table durante la simulazione.</br>
* Se viene spuntata la casella <b>Trained</b> l'agente caricherà la q-table presente nel path denominato Q-Table Path.</br>
* Se Trained è spuntata ma Training no, l'agente avrà un exploration rate di 0 e seguirà solo la Q-Table. Questa modalità è utilizzata nel <b>testing</b> del modello addestrato.</br>
* Se nessuna delle due è spuntata, l'agente avrà un exploration rate di 0.9 che non decadrà e non aggiornerà la tabella. Questa modalità è utilizzata nel <b>testing</b> del modello non addestrato.</br>
* Se sono spuntate entrambe, si potranno affinare le conoscenze dell'agente.</br>
* Nella GUI sono presenti due pulsanti:</br>
* <b>Speed</b>: aumenta il clock dell'engine di 100. Permette di addestrare più velocemente senza modificare il comportamento dell'agente.</br>
* <b>Save results</b>: salverà i risultati delle collisioni in un file json, il path viene registrato nella console e può essere modificato alla voce Results Path dell'inspector.</br>
* Premendo il tasto <b>O</b>, sarà possibile salvare la Q-Table, nel path specificato nell'apposita voce dell'inspector.</br>
* Premendo il tasto <b>L</b> è possibile caricare la Q-Table specificata. Non è necessario farlo all'inizio se è stata spuntata la casella Trained.</br>
* Premendo il tasto <b>2</b> è possibile inquadrare l'agente da una Camera secondaria. In questa modalità è possibile avvicinare o allontanare l'inquadratura con la rotella del mouse o con un trackpad.</br>
* Premendo il tasto <b>1</b> è possibile tornare all'inquadratura originale.</br>

Una volta configurato l'agente è possibile avviare la simulazione premendo il tasto Play.

