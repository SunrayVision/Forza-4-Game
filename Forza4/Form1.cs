using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forza4
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }
        public Bitmap Rosso = new Bitmap(Properties.Resources.DiscoRosso);
        public Bitmap Giallo = new Bitmap(Properties.Resources.DiscoGiallo);
        public Forza4 f = new Forza4();
        public Forza4.Colore coloreGiocatore = Forza4.Colore.Rosso;

        private void Form1_Load(object sender, EventArgs e)
        {
            f.turno = coloreGiocatore;
        }
        private void Restart()
        {
            PictureBox[] arr = Controls.OfType<PictureBox>().ToArray();
            int len = arr.GetLength(0) - 1;
            for (int i = 0; len > i; i++)//Itera tutte le pictureBox (tranne quella il cui indice == len(arr)-1 [e' picPreview, non fa parte della griglia]
            {
                Controls.Remove(arr[i]);//Rimuove tutte le pictureBox che fanno parte della griglia
            }
            f = new Forza4();//Inizializza una nuova istanza della classe "Forza4" (principalmente per "ripulire" la matrice in cui erano contenuti i colori delle pedine posizionate)
            f.turno = coloreGiocatore;
            picPreview.Image = Immagine(f.turno); 
            return;
        }
        private bool controlloStato()
        {
            if (f.stato != Forza4.Colore.Vuoto)
            {
                MessageBox.Show("Giocatore " + (f.turno == Forza4.Colore.Rosso ? Forza4.Colore.Giallo : Forza4.Colore.Rosso) + " ha vinto!");
                Restart();
                return true;
            }
            if(f.Pieno())
            {
                MessageBox.Show("Pareggio!");
                Restart();
                return true;
            }
            return false;
        }
        private Bitmap Immagine(Forza4.Colore c)
        {
            switch (c)
            {
                case Forza4.Colore.Rosso:
                    return Rosso;
                case Forza4.Colore.Giallo:
                    return Giallo;
                default:
                    return null;
            }
        }
        private void picNuova(int x, int y, Bitmap bmp)
        {
            PictureBox pic = new PictureBox();
            pic.Name = "pic" + x.ToString() + y.ToString();
            pic.Size = bmp.Size;
            pic.Tag = y;
            pic.Location = new Point(y * bmp.Width, x * bmp.Height + 141);
            pic.Image = bmp;
            pic.BackColor = Color.Transparent;
            pic.Click += picClick;
            pic.DoubleClick += picClick;
            Controls.Add(pic);
            pic.BringToFront();
        }
        private Point Mossa(int c)
        {
            int r = f.Mossa(f.turno, c);
            if (r != -1)
            {
                Bitmap bmp = Immagine(f.turno);
                if (bmp != null)
                {
                    picNuova(r, c, bmp);
                    f.turno = f.turno == Forza4.Colore.Rosso ? Forza4.Colore.Giallo : Forza4.Colore.Rosso;
                    picPreview.Image = Immagine(f.turno);
                    return new Point(r, c);
                }
            }
            return new Point(-1, -1);
        }
        private void MossaGiocatore(int c)
        {
            var ultimaMossa = Mossa(c);
            if (!(ultimaMossa.X == -1))
            {
                if (giocaControIAToolStripMenuItem.Checked & (!controlloStato()))
                {
                    megaswitchone(ultimaMossa);
                    controlloStato();
                }
            }
        }
        private void AggiornaPosPreview(int n)
        {
            picPreview.Location = new Point(picPreview.Size.Width * n, picPreview.Location.Y);
        }
        private void buttonMouseEnter(object sender, EventArgs e)
        {
            AggiornaPosPreview(int.Parse(((Button)sender).Tag.ToString()));
        }
        private void buttonClick(object sender, EventArgs e)
        {
            MossaGiocatore(int.Parse(((Button)sender).Tag.ToString()));
            ((Button)sender).SendToBack();
        }
        private void picClick(object sender, EventArgs e)
        {
            MossaGiocatore(int.Parse(((PictureBox)sender).Tag.ToString()));
            //((PictureBox)sender).BringToFront();
            //Button[] buttons = new Button[7] { button0, button1, button2, button3, button4, button5, button6 };
            //buttons[int.Parse(((PictureBox)sender).Tag.ToString())].SendToBack();
        }
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart();
        }
        ////////////////////////////////////////////////////////////////////////

        public void megaswitchone(Point ultimaMossa)
        {
            if (controllo_verticale(ultimaMossa))
            {
                return;
            }
            if (controllo_orizzontale(ultimaMossa))
            {
                return;
            }
            if (controllo_diagonale(ultimaMossa))
            {
                return;
            }
            if (controllo_orizzontale_ulteriore(ultimaMossa))
            {
                return;
            }
            if (controllo_diagonale_ulteriore(ultimaMossa))
            {
                return;
            }
            Random random = new Random();
            if(!(f.Pieno()))
            {
                while(Mossa(random.Next(0, 7)).X == -1);
            }
            return;
        }
        public bool controllo_verticale(Point ultimaMossa)
        {
            if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y) == true)
                if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y].colore == coloreGiocatore)
                {
                    if (f.Valido(ultimaMossa.X + 2, ultimaMossa.Y) == true)
                        if (f.matrice[ultimaMossa.X + 2, ultimaMossa.Y].colore == coloreGiocatore)
                        {
                            return !(Mossa(ultimaMossa.Y).X == -1);
                        }
                }
            return false;
            //aumenta la colonna di 1 fino, in un if, se il nemico ha  le 2 celle sottostanti mettere disco, altrimenti continuare
        }
        public bool controllo_orizzontale(Point ultimaMossa)
        {
            if (f.Valido(ultimaMossa.X, ultimaMossa.Y + 1) == true)
            {         //verso destra
                if (f.matrice[ultimaMossa.X, ultimaMossa.Y + 1].colore == coloreGiocatore)
                {
                    if (f.Valido(ultimaMossa.X, ultimaMossa.Y + 2) == true)

                    {
                        if (f.matrice[ultimaMossa.X, ultimaMossa.Y + 2].colore == coloreGiocatore)
                            if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y - 1) == false)
                                return !(Mossa(ultimaMossa.Y - 1).X == -1);
                        if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y - 1) == true)
                            if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y - 1].colore != Forza4.Colore.Vuoto)
                            {
                                return !(Mossa(ultimaMossa.Y - 1).X == -1);
                            }
                    }
                    else
                    {
                        if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y + 1))
                            if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y + 1].colore != Forza4.Colore.Vuoto)
                            {
                                return !(Mossa(ultimaMossa.Y - 1).X == -1);
                            }

                    }

                }
            }
            if (f.Valido(ultimaMossa.X, ultimaMossa.Y - 1) == true)
            {         //verso sinistra
                if (f.matrice[ultimaMossa.X, ultimaMossa.Y - 1].colore == coloreGiocatore)
                {
                    if (f.Valido(ultimaMossa.X, ultimaMossa.Y - 2) == true)
                    {
                        if (f.matrice[ultimaMossa.X, ultimaMossa.Y - 2].colore == coloreGiocatore)
                            if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y + 1) == false)
                                return !(Mossa(ultimaMossa.Y + 1).X == -1);
                        if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y + 1) == true)
                            if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y + 1].colore != Forza4.Colore.Vuoto)
                            {
                                return !(Mossa(ultimaMossa.Y + 1).X == -1);
                            }
                    }
                    else
                    {
                        if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y - 1))
                            if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y - 1].colore != Forza4.Colore.Vuoto)
                            {
                                return !(Mossa(ultimaMossa.Y + 1).X == -1);
                            }
                    }
                }
            }
            return false;
        }
        public bool controllo_diagonale(Point ultimaMossa)
        {
            if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y - 1) == true)//guardare se la posiz. Alto Sinistra è vuota
                if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y - 1].colore == Forza4.Colore.Vuoto)
                {
                    if (f.Valido(ultimaMossa.X - 2, ultimaMossa.Y - 2) == true)
                        if (f.matrice[ultimaMossa.X - 2, ultimaMossa.Y - 2].colore == coloreGiocatore)
                        {
                            if (f.matrice[ultimaMossa.X, ultimaMossa.Y - 1].colore != Forza4.Colore.Vuoto)
                                return !(Mossa(ultimaMossa.Y - 1).X == -1);
                        }
                }

            if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y + 1) == true)//guardare se la posiz. Basso Destra è vuota
                if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y + 1].colore == Forza4.Colore.Vuoto)
                {
                    if (f.Valido(ultimaMossa.X + 2, ultimaMossa.Y + 2) == true)
                        if (f.matrice[ultimaMossa.X + 2, ultimaMossa.Y + 2].colore == coloreGiocatore)
                        {
                            if (f.matrice[ultimaMossa.X + 2, ultimaMossa.Y + 1].colore != Forza4.Colore.Vuoto)
                                return !(Mossa(ultimaMossa.Y + 1).X == -1);
                        }
                }

            if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y + 1) == true)//guardare se la posiz. Alto Destra è vuota
                if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y + 1].colore == Forza4.Colore.Vuoto)
                {
                    if (f.Valido(ultimaMossa.X - 2, ultimaMossa.Y + 2) == true)
                        if (f.matrice[ultimaMossa.X - 2, ultimaMossa.Y + 2].colore == coloreGiocatore)
                        {
                            if (f.matrice[ultimaMossa.X, ultimaMossa.Y + 1].colore != Forza4.Colore.Vuoto)
                                return !(Mossa(ultimaMossa.Y + 1).X == -1);
                        }
                }

            if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y - 1) == true)//guardare se la posiz. basso Sinistra è vuota
                if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y - 1].colore == Forza4.Colore.Vuoto)
                {
                    if (f.Valido(ultimaMossa.X + 2, ultimaMossa.Y - 2) == true)
                        if (f.matrice[ultimaMossa.X + 2, ultimaMossa.Y - 2].colore == coloreGiocatore)
                        {
                            if (f.matrice[ultimaMossa.X + 2, ultimaMossa.Y - 1].colore != Forza4.Colore.Vuoto)
                                return !(Mossa(ultimaMossa.Y - 1).X == -1);
                        }
                }

            if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y - 1) == true)//guardare se la posiz. Basso Sinstra è vuota e c'è sotto la barriera
                if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y - 1].colore == Forza4.Colore.Vuoto)
                {
                    if (f.Valido(ultimaMossa.X + 2, ultimaMossa.Y - 2) == false)
                        if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y + 1) == true)
                            if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y + 1].colore == coloreGiocatore)
                            {
                                return !(Mossa(ultimaMossa.Y - 1).X == -1);
                            }

                }

            if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y + 1) == true)//guardare se la posiz. Basso Destra è vuota e c'è sotto la barriera
                if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y + 1].colore == Forza4.Colore.Vuoto)
                {
                    if (f.Valido(ultimaMossa.X + 2, ultimaMossa.Y + 2) == false)
                        if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y - 1) == true)
                            if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y - 1].colore == coloreGiocatore)
                            {
                                return !(Mossa(ultimaMossa.Y + 1).X == -1);
                            }

                }

            if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y + 1) == true)//guardare se la posiz. Altro Destra è vuota e c'è sotto la barriera
                if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y + 1].colore == Forza4.Colore.Vuoto)
                {
                    if (f.Valido(ultimaMossa.X - 2, ultimaMossa.Y + 2) == false)
                        if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y - 1) == true)
                            if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y - 1].colore == coloreGiocatore)
                                if (f.matrice[ultimaMossa.X, ultimaMossa.Y + 1].colore != Forza4.Colore.Vuoto)
                                {
                                    return !(Mossa(ultimaMossa.Y + 1).X == -1);
                                }

                }

            if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y - 1) == true)//guardare se la posiz. Alto Sinistra è vuota e c'è sotto la barriera
                if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y - 1].colore == Forza4.Colore.Vuoto)
                {
                    if (f.Valido(ultimaMossa.X - 2, ultimaMossa.Y - 2) == false)
                        if (f.Valido(ultimaMossa.X + 1, ultimaMossa.Y + 1) == true)
                            if (f.matrice[ultimaMossa.X + 1, ultimaMossa.Y + 1].colore == coloreGiocatore)
                                if (f.matrice[ultimaMossa.X, ultimaMossa.Y - 1].colore != Forza4.Colore.Vuoto)
                                {
                                    return !(Mossa(ultimaMossa.Y - 1).X == -1);
                                }

                }
            return false;
        }
        public bool controllo_orizzontale_ulteriore(Point ultimaMossa)
        {
            if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y - 1) == true)
                if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y - 1].colore == coloreGiocatore)
                    if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y - 2) == true)
                        if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y - 2].colore == coloreGiocatore)
                        {
                            return !(Mossa(ultimaMossa.Y).X == -1);
                        }
            if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y + 1) == true)
                if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y + 1].colore == coloreGiocatore)
                    if (f.Valido(ultimaMossa.X - 1, ultimaMossa.Y + 2) == true)
                        if (f.matrice[ultimaMossa.X - 1, ultimaMossa.Y + 2].colore == coloreGiocatore)
                        {
                            return !(Mossa(ultimaMossa.Y).X == -1);
                        }
            return false;
        }
        public bool controllo_diagonale_ulteriore(Point ultimaMossa)
        {
            if (f.Valido(ultimaMossa.X - 2, ultimaMossa.Y - 1) == true)//controllo possibilità diagonale superiore Alto Sinistra, Basso Destra
                if (f.matrice[ultimaMossa.X - 2, ultimaMossa.Y - 1].colore == coloreGiocatore)
                    if (f.Valido(ultimaMossa.X, ultimaMossa.Y + 1) == true)
                        if (f.matrice[ultimaMossa.X, ultimaMossa.Y + 1].colore == coloreGiocatore)
                        {
                            return !(Mossa(ultimaMossa.Y).X == -1);
                        }

            if (f.Valido(ultimaMossa.X - 2, ultimaMossa.Y + 1) == true)//controllo possibilità diagonale superiore Basso Sinistra, Alto Destra
                if (f.matrice[ultimaMossa.X - 2, ultimaMossa.Y + 1].colore == coloreGiocatore)
                    if (f.Valido(ultimaMossa.X, ultimaMossa.Y - 1) == true)
                        if (f.matrice[ultimaMossa.X, ultimaMossa.Y - 1].colore == coloreGiocatore)
                        {
                            return !(Mossa(ultimaMossa.Y).X == -1);
                        }
            return false;
        }
    }
}
