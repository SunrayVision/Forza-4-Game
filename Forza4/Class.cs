using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forza4
{
    public class Forza4
    {
        public enum Colore
        {
            Vuoto,
            Rosso,
            Giallo,
        }
        public Colore turno = Colore.Vuoto;
        public class Casella
        {
            
            public int riga;
            public int colonna;
            public Colore colore = Colore.Vuoto;
            public Casella(int r, int c)
            {
                riga = r;
                colonna = c;
            }
        }
        public Colore stato = Colore.Vuoto; //Vuoto = In corso, se non e' Vuoto allora e' uguale al colore che ha vinto.
        public Casella[,] matrice = {
            {new Casella(0, 0), new Casella(0,1), new Casella(0,2), new Casella(0, 3), new Casella(0, 4), new Casella(0, 5), new Casella(0, 6) },
            {new Casella(1, 0), new Casella(1,1), new Casella(1,2), new Casella(1, 3), new Casella(1, 4), new Casella(1, 5), new Casella(1, 6) },
            {new Casella(2, 0), new Casella(2,1), new Casella(2,2), new Casella(2, 3), new Casella(2, 4), new Casella(2, 5), new Casella(2, 6) },
            {new Casella(3, 0), new Casella(3,1), new Casella(3,2), new Casella(3, 3), new Casella(3, 4), new Casella(3, 5), new Casella(3, 6) },
            {new Casella(4, 0), new Casella(4,1), new Casella(4,2), new Casella(4, 3), new Casella(4, 4), new Casella(4, 5), new Casella(4, 6) },
            {new Casella(5, 0), new Casella(5,1), new Casella(5,2), new Casella(5, 3), new Casella(5, 4), new Casella(5, 5), new Casella(5, 6) },
        };
        public bool Valido(int r, int c) //Controlla validita' posizione di una casella controllando che essa si trovi entro il range delle dimensioni della matrice
        {
            int righe = matrice.GetLength(0);   //6
            int colonne = matrice.GetLength(1); //7
            return (righe > r) && (r >= 0) && (colonne > c) && (c >= 0);
        }
        public bool Valido(int x, int y, int dx, int dy)
        {
            return Valido(x + dx, y + dy);
        }        
        public int[][] trovaAdiacenti(Casella cas)
        {
            //Array di deltaCoordinate relative, indicano caselle adiacenti:
            int[,] adiacenti = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };
            //                   GIU'   |   DESTRA  |  SU  |   SINISTRA  |GIU-DESTRA|SU-SINISTRA|GIU'-SINISTRA|SU-SINISTRA
            int[][] res = new int[8][];
            int cur = 0;
            for (int i = 0; adiacenti.GetLength(0) > i; i++)
            {
                if (Valido(cas.riga, cas.colonna, adiacenti[i, 0], adiacenti[i, 1])) //Se risultato e' fuori dalla matrice, allora continua
                {
                    res[cur] = new int[] { adiacenti[i, 0], adiacenti[i, 1] }; //Aggiunge risultato ad array
                    cur++;
                }
            }
            int[][] ress = new int[cur][];
            for (int i = 0; cur > i; i++) //Creazione di un array di dimensioni corrette, senza risultati "sbagliati" (slicing - versione fai da te)
            {
                ress[i] = res[i];
            }
            return ress; //Ritorna un array senza risultati "sbagliati"
        }
        public Casella[] trovaCaselleAdiacenti(Casella cas)
        {
            int[][] adiacenti = trovaAdiacenti(cas);
            int len = adiacenti.GetLength(0);
            Casella[] res = new Casella[len];
            for (int i = 0; len > i; i++)
            {
                res[i] = matrice[cas.riga + adiacenti[i][0], cas.colonna + adiacenti[i][1]];
            }
            return res;
        }
        public int Mossa(Colore colore, int colonna)
        {
            if ((colonna >= matrice.GetLength(1)) | (0 > colonna)) //Controllo validita' colonna
            {
                return -1;
            }
            for (int i = matrice.GetLength(0) - 1; i >= 0; i--) //Itera casella della colonna, a partire dal basso
            {
                if (matrice[i, colonna].colore == Forza4.Colore.Vuoto) //Cerca prima casella vuota (a partire dal basso)
                {
                    matrice[i, colonna].colore = colore;
                    Controllo(matrice[i, colonna]);
                    return i;
                }
            }
            return -1; //Se non ci sono caselle vuote nella colonna
        }
        public bool Controllo(Casella cas)
        {
            if (cas.colore == Colore.Vuoto) //Se la casella e' vuota, ci deve essere stato qualche errore (Controllo e' chiamata solo subito dopo Mossa)
            {
                Console.WriteLine("E' stata passata una casella vuota come parametro della funzione di controllo");
                return false;
            }
            int count; //Variabile temporanea per conteggio colori adiacenti in sequenza
            int[][] adiacenti = trovaAdiacenti(cas); //Array di coordinate relative alla casella che indicano le sue caselle adiacenti
            int distR = 0; //Var temporanea per deltaR (per leggibilita' codice)
            int distC = 0; //Var temporanea per deltaC (per leggibilita' codice)
            for (int i = 0; adiacenti.Length > i; i++) //Per ogni casella adiacente...
            {
                distR = adiacenti[i][0];
                distC = adiacenti[i][1];
                //cur = matrice[cas.rig + distX, cas.col + distY];
                /*
                    Moltiplicando entrambe le variabili dist per un valore (j) si ottiene la j-esima casella in sequenza nella stessa direzione
                    retta ha 2 direzioni -> 2 for loop
                */
                count = 1;
                for (int j = 1; 4 > j; j++)
                {
                    if (!(Valido(cas.riga + distR * j, cas.colonna + distC * j)))
                    {
                        break;
                    }
                    if (matrice[cas.riga + distR * j, cas.colonna + distC * j].colore == cas.colore)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                for (int j = 1; 4 > j; j++)
                {
                    if (!(Valido(cas.riga - distR * j, cas.colonna - distC * j)))
                    {
                        break;
                    }
                    if (matrice[cas.riga - distR * j, cas.colonna - distC * j].colore == cas.colore)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (count >= 4) //Se conteggio stesso colore in sequenza e' maggiore o uguale a 4, qualcuno ha vinto
                {
                    stato = cas.colore; //Stato = colore che ha vinto (oppure -1, se la partita e' ancora in corso)
                    return true;
                }
            }
            return false; //Se nessuno ha vinto
        }
        public bool Pieno(int r = 0)
        {
            int colonne = matrice.GetLength(1);
            for(int i = 0; colonne > i; i++)
            {
                if(matrice[r,i].colore == Forza4.Colore.Vuoto)
                {
                    return false;
                }
            }
            return true;
        }
        public bool tuttoPieno()
        {
            int righe = matrice.GetLength(0);   //6
            int colonne = matrice.GetLength(1); //7
            for (int i = 0; righe > i; i++)
            {
                for(int j = 0; colonne > j; j++)
                {
                    if(matrice[i,j].colore == Colore.Vuoto)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
