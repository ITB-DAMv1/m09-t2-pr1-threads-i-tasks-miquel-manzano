# DinnerPhilosophers

## 🧠 Solució proposada

Aquest projecte implementa el problema clàssic del **sopar dels filòsofs**, on cada filòsof és representat com un fil (`Thread`) que necessita dos palets (recursos compartits) per menjar. Per evitar interbloquejos i condicions de fam:

- Els palets són bloquejats amb l’ordre altern (filòsofs parells comencen per l’esquerra, senars per la dreta).
- Es controla el temps des de l’últim àpat de cada filòsof. Si supera els **15 segons**, es deté la simulació.
- També s’atura la simulació si passen **60 segons totals**, per evitar execucions infinites.

Les dades finals de cada filòsof (vegades menjades i temps màxim amb fam) es guarden a `dinner_stats.csv`.

### 🔄 Estructura del projecte

- `Program.cs`: Entrada principal
- `Dinner`: Controla la simulació i sincronització global
- `Philosopher`: Defineix el comportament de cada filòsof

---

## ❓ Preguntes requerides

### 📌 Enunciat 1: Com has fet per evitar interbloquejos i que ningú passes fam?

- **Estrategia per evitar interbloquejos**: Cada filòsof agafa els palets en ordre invers segons el seu identificador (parell/esquerra → dreta, senar/dreta → esquerra). Això trenca el cicle de dependències circulars que genera un interbloqueig.
- **Evitar fam**: Cada filòsof manté un registre de l'última vegada que va menjar. Si un supera els 15 segons sense menjar, la simulació es deté immediatament.

---

## 📊 Dades d’anàlisi (exemple)

- **Quants comensals han passat fam?**  
  Cap filòsof ha estat més de 15s sense menjar (sinó s'hauria aturat la simulació).
- **Temps mitjà de fam:**  
  Cal calcular-ho a partir del CSV.
- **Vegades que ha menjat cada filòsof de mitjana:**  
  Dependrà de l'execució, però l'objectiu és que tots tinguin accés equitatiu.
- **Rècord de més i menys menjars:**  
  També al fitxer CSV (`dinner_stats.csv`).


<br>


# AsteroidsGame

## 🎮 Solució proposada

Aquest projecte simula un minijoc retro d'asteroides per consola, desenvolupat en C# amb programació **paral·lela i asíncrona** usant la classe `Task`.

La consola mostra una nau (`^`) que es pot moure lateralment amb les tecles `A` (esquerra) i `D` (dreta). Asteroides (`*`) cauen des de la part superior. Si col·lideixen amb la nau, es compta com a vida perduda. El joc es tanca si l’usuari prem `Q` o si acaba la simulació web paral·lela (entre 30 i 60 segons).

Les estadístiques del joc es guarden en un fitxer CSV: `game_stats.csv`.

---

## ⚙️ Bucle i estructura de tasques

### 🔁 Tasques implementades:

- **InputLoop**: escolta les tecles de l'usuari (`A`, `D`, `Q`).
- **DrawLoop**: redibuixa la pantalla a 20 Hz (cada 50 ms).
- **LogicLoop**: actualitza posicions dels asteroides i col·lisions a 50 Hz (cada 20 ms).
- **SimulateWebCheck**: simula una tasca en segon pla de revisió de webs, amb una durada entre 30 i 60 segons.

Cada tasca és executada amb `Task.Run()` i controlada des del fil principal.

---

## ❓ Preguntes requerides

### 📌 Com has executat les tasques per tal de pintar, calcular i escoltar el teclat al mateix temps?

He utilitzat `Task.Run()` per iniciar cada bucle de forma concurrent. Això permet que la consola reaccioni a l'usuari mentre continua actualitzant la lògica del joc i la visualització. El joc també executa una tasca paral·lela de simulació de revisió web per emular un procés secundari.

### 📌 Has diferenciat entre programació paral·lela i asíncrona?

Sí. El joc empra **programació paral·lela** per a les tasques contínues (`DrawLoop`, `LogicLoop`, `InputLoop`) que s'executen alhora. També empra **programació asíncrona** (`async/await`) per a controlar el final del joc quan la tasca web acaba.

---

## 📊 Dades recollides

Al final de cada partida, es guarda un registre amb:

- Asteroides esquivats
- Vides perdudes
- Temps total de joc

Fitxer: `game_stats.csv`

---

## ✍️ Nota

Per fer el `README`, he argumentat les respostes a les preguntes i he explicat el codi amb l’ajuda d’una IA, que m’ho ha generat tot en format Markdown.

