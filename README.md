<p align="center"><img src="https://github.com/nf0101/NFDriveAI/blob/main/Slide.png?raw=true" width="1000"></p>

# NFDriveAI

NFDriveAI è un progetto in Unity che implementa un sistema di intelligenza artificiale con apprendimento per rinforzo, utilizzando il Q-Learning. L'obiettivo è permettere all'agente di guidare un'automobile lungo diverse piste.

## Organizzazione respository
<b>NFDriveAI:</b> Progetto Unity. Mantiene la tipica gerarchia dei file di Unity. </br>
* Nel subfolder <b>Learning</b> sono presenti la Q-Table di un agente addestrato e gli ultimi risultati ottenuti. </br>

<b>PlotData:</b> Script data visualization
* Per utilizzarlo, salvare i risultati dell'agente come spiegato più avanti.
* Eseguendo da CMD il comando <b>python main.py</b> nel folder PlotData senza inserire i parametri, verranno cercati i risultati nel folder della repository.
* Se i path vengono modificati o se non viene scaricata l'intera repository, bisogna aggiungere al comando <b>python main.py</b> tre argomenti, che saranno i path dei tre file JSON, nell'ordine: collisioni per giro, media collisioni, collisioni totali. Es. <b>python main.py C:\Folder\CollisioniGiro.json C:\Folder\MediaCOllisioni.json C:\Folder\CollisioniTotali.json </b>

## Installazione e Utilizzo
Per utilizzare il progetto seguire i seguenti passaggi:
### Requisiti:
<b>Unity Edito</b>r, possibilmente versione <b>2022.3.2f1</b> per evitare incompatibilità.
### Installazione
* Scaricare lo Unity Package dalla sezione release
* Creare un progetto 2D
* Importare il package tramite <b>Asset > Import package > Custom package...</b> > Selezionare il package scaricato.
* Accettare di importare il progetto e asscicurarsi di importare tutte le voci.
* Se la console restituisce un errore per la libreria Newtonsoft.json, andare in <b>Window</b> > <b>Package Manager</b> > Premere <b>+</b> in alto a sinistra > <b>Add package from git URL...</b> > Inserire <b>com.unity.nuget.newtonsoft-json</b> > Premere <b>Add</b>.
* Tramite la gestione dei folder del progetto, andare nella cartella <b>Assets</b> > <b>Scenes</b> > Cliccare due volte su <b>SampleScene</b>. Rifiutare di salvare la scena, altrimenti verrà sovrascritta la scena del progetto e dovrà essere reimportata.
* Si consiglia di impostare la risoluzione FHD o maggiore nella modalità Play per una migliore qualità.
* Si consiglia di disattivare le icone degli SpriteShape nella modalità Play, premendo sulla freccia della label Gizmos, in alto a destra, scorrendo fino alle icone viola degli SpriteShape e premendo sull'icona di SpriteShapeController. 

Il package caricherà anche le Settings del progetto in quanto sono state modificate per far funzionare la fisica nel modo corretto. 
### Utilizzare il progetto
Nel package scaricato saranno già presenti piste e agenti.</br>
Di default, quando il package viene importato, si avrà la terza pista, con tutte le impostazioni per il testing e l'agente caricherà la Q-Table.</br>
Assicurarsi che l'inspector sia in modalità <b>Normale</b> per effettuare modifiche alla mappa 
Per utilizzare le altre piste basta attivarle dalla Hierarchy e riposizionare la Main Camera e se necessario l'agente.</br>
Nell'inspector sarà possibile modificare i parametri degli agenti. I parametri rilevanti sono quelli dello script CarAgentFixed. Selezionare la Car dalla Hierarchy e nell'inspector scorrere fino allo script CarAgentFixed</br>.
Il funzionamento dei parametri è il seguente:</br>
* Se viene spuntata la casella <b>Training</b>, l'agente aggiornerà la Q-Table durante la simulazione.</br>
* Se viene spuntata la casella <b>Trained</b> l'agente caricherà la Q-Table presente nel path denominato Q-Table Path.</br>
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
<b>N.B. il tasto O e il tasto Save Results sovrascrivono i dati al path specificato</b>

Una volta configurato l'agente è possibile avviare la simulazione premendo il tasto Play.

### Replicare il training della documentazione
Per ottenere gli stessi risultati presenti nella documentazione:
* Togliere la spunta alla casella <b>Trained</b>
* Spuntare la casella <b>Training</b>
* Inserire 5000 giri nel campo <b>Laps to do</b>
* Avviare la simulazione
* Premere il tasto <b>Speed</b> per velocizzare il processo
* Attendere la fine dei giri e premere il tasto <b>Speed</b>
* Premere il tasto <b>O</b>
* Premere il tasto <b>Save Results<b></b> se si vogliono esportare i dati
* Premere il tasto <b>Play</b> di Unity per fermare la simulazione

A questo punto è possibile usare lo script in Python main.py per visualizzare i grafici come specificato sopra

### Replicare il testing della documentazione
Per testare l'agente dopo il training precedente:
* Togliere la spunta alla casella <b>Training</b>
* Spuntare la casella <b>Trained</b>
* Inserire 50 giri nel campo <b>Laps to do</b>
* Avviare la simulazione
* Premere il tasto <b>Speed</b> se si vuole velocizzare il processo
* Attendere la fine dei giri e premere il tasto <b>Speed</b> se necessario
* Premere il tasto <b>Save Results<b></b> se si vogliono esportare i dati
* Premere il tasto <b>Play</b> di Unity per fermare la simulazione

A questo punto è possibile usare lo script in Python main.py per visualizzare i grafici come specificato sopra

n.d.r. nel package è presente una pista aggiuntiva non documentata, non è da considerare.
