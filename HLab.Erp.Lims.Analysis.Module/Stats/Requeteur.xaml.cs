using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

using Npgsql;

namespace Outils
{


    /// <summary>
    /// Logique d'interaction pour Requeteur.xaml
    /// </summary>
    public partial class Requeteur : UserControl, IView<Requete> , IViewClassDocument
    {
        DataService _data;
        public Requeteur(DataService data)
        {
            _data = data;

            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            G_Requete.Visibility = System.Windows.Visibility.Collapsed;
            RafraichieRequetes();
        }

        private Requete Requete => DataContext as Requete;

        private int IdRequete = int.MinValue;

        public event EventHandler OnObjetSelect;


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Executer_Click(object sender, RoutedEventArgs e)
        {
            // Démarre le chronomètre
            Stopwatch chrono = new Stopwatch();
            chrono.Start();

            try
            {
                // Requete
                String requete = TB_Requeteur.Text;// new TextRange(RTB_Requeteur.Document.ContentStart, RTB_Requeteur.Document.ContentEnd).Text;
                requete = requete.Replace("{1}", TB_Parametre1.Text);
                requete = requete.Replace("{2}", TB_Parametre2.Text);
                requete = requete.Replace("{3}", TB_Parametre3.Text);
                requete = requete.Replace("{4}", TB_Parametre4.Text);
                requete = requete.Replace("\r", "").Replace("\n", " ");


                // Execute la requete
                //Sql.Lit lit = new Sql.Lit(requete);
                using var con = new NpgsqlConnection(_data.ConnectionString);
                DG_Resultats.Columns.Clear();
                con.Open();
                using var cmd = new NpgsqlCommand(Requete.Query, con);
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                    throw new Exception("Résultat vide");

                var cols = reader.GetColumnSchema();
                for (int i = 1; i < cols.Count; i++)
                {
                    //Console.WriteLine(lit._Proprietes[i]);
                    DataGridTextColumn colonne = new DataGridTextColumn();
                    colonne.Header = cols[i].ColumnName;
                    colonne.Binding = new Binding(cols[i].ColumnName);
                    colonne.Width = new DataGridLength(1.0 / cols.Count, DataGridLengthUnitType.Star);
                    DG_Resultats.Columns.Add(colonne);
                }

                var list = new ObservableCollection<Ligne>();

                while (reader.Read())
                {
                    var ligne = new Ligne{
                        _Proprietes=cols.Select(c => c.ColumnName).ToArray(),
                        _Valeurs = new String[cols.Count]
                        };
                    for (int i = 1; i < cols.Count; i++)
                    {
                        ligne._Valeurs[i] = reader.GetFieldValue<string>(i);
                    }
                    list.Add(ligne);
                }

                DG_Resultats.ItemsSource = list;
                L_NbResultats.Content = reader.RecordsAffected.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERREUR dans la requete", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Arrête le chronomètre
            chrono.Stop();
            Console.WriteLine(chrono.Elapsed.TotalMilliseconds.ToString("0,0.00") + " ms");
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void DG_Resultats_CellMouseMove(object sender, MouseEventArgs e)
        {

            if(sender is DataGridCell d && d.Content is TextBlock tb)
            {

                String texte = ((TextBlock)((DataGridCell)sender).Content).Text;


                if (texte.Length == 0)
                {
                    Pop_Cellule.IsOpen = false;
                    return;
                }

                if (TB_Cellule.Text != texte)
                    TB_Cellule.Text = texte;

                Point pt = Mouse.GetPosition(DG_Resultats);
                Pop_Cellule.HorizontalOffset = pt.X + 20.0;
                Pop_Cellule.VerticalOffset = pt.Y + 20.0;

                if (Pop_Cellule.IsOpen == false)
                    Pop_Cellule.IsOpen = true;
            }
        }

        private void DG_Resultats_MouseLeave(object sender, MouseEventArgs e)
        {
            Pop_Cellule.IsOpen = false;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void CB_Requete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            IdRequete = 0;//_SqlRequetes.Int32b("IdRequete");
            TB_Libelle.Text = "";//_SqlRequetes.String("Libelle");

            // Paramètres
            String[] parametres = null;//_SqlRequetes.String("Parametres").Split('|');
            TB_LibelleParametre1.Text = "";
            TB_LibelleParametre2.Text = "";
            TB_LibelleParametre3.Text = "";
            TB_LibelleParametre4.Text = "";
            int n = 0;
            foreach (String parametre in parametres)
            {
                if (parametre.Length > 0)
                {
                    n++;
                    switch (n)
                    {
                        case 1: TB_LibelleParametre1.Text = parametre; break;
                        case 2: TB_LibelleParametre2.Text = parametre; break;
                        case 3: TB_LibelleParametre3.Text = parametre; break;
                        case 4: TB_LibelleParametre4.Text = parametre; break;
                    }
                }
            }

            TB_LibelleParametre1_TextChanged(null, null);
            TB_LibelleParametre2_TextChanged(null, null);
            TB_LibelleParametre3_TextChanged(null, null);
            TB_LibelleParametre4_TextChanged(null, null);

            //TB_Requeteur.Text = _SqlRequetes.String("Requete"); 
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void CK_VoirRequete_Checked(object sender, RoutedEventArgs e)
        {
            G_Requete.Visibility = CK_VoirRequete.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void TB_LibelleParametre1_TextChanged(object sender, TextChangedEventArgs e)
        {
            TB_Parametre1.Tag = TB_LibelleParametre1.Text;
            TB_Parametre1.Visibility = TB_LibelleParametre1.Text.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void TB_LibelleParametre2_TextChanged(object sender, TextChangedEventArgs e)
        {
            TB_Parametre2.Tag = TB_LibelleParametre2.Text;
            TB_Parametre2.Visibility = TB_LibelleParametre2.Text.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void TB_LibelleParametre3_TextChanged(object sender, TextChangedEventArgs e)
        {
            TB_Parametre3.Tag = TB_LibelleParametre3.Text;
            TB_Parametre3.Visibility = TB_LibelleParametre3.Text.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void TB_LibelleParametre4_TextChanged(object sender, TextChangedEventArgs e)
        {
            TB_Parametre4.Tag = TB_LibelleParametre4.Text;
            TB_Parametre4.Visibility = TB_LibelleParametre4.Text.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Crée la transaction
            //Sql.Transaction(() =>
            //{
            // // Enregistre la requête
            // Sql.Sauve sav = new Sql.Sauve("Requete");
            //    sav["Libelle"] = TB_Libelle.Text;
            //    sav["Parametres"] = TB_LibelleParametre1.Text + "|" + TB_LibelleParametre2.Text + "|" + TB_LibelleParametre3.Text + "|" + TB_LibelleParametre4.Text;
            //    sav["Requete"] = TB_Requeteur.Text; // new TextRange(RTB_Requeteur.Document.ContentStart, RTB_Requeteur.Document.ContentEnd).Text;
            // sav["TaillesColonnes"] = "";
            //    int idRequete = sav.AutoSauve("IdRequete", IdRequete);

            // // Réattribue l'IdEchantillon si c'était un nouveau
            // IdRequete = idRequete;
            //});

            // Rafraichie la liste des requêtes
            int id = IdRequete;
            RafraichieRequetes();
//            _SqlRequetes.Positionne("IdRequete", id);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void RafraichieRequetes()
        {
            //_SqlRequetes = new Sql.Lit("SELECT IdRequete, Libelle, Requete, Parametres, TaillesColonnes FROM Requete ORDER BY Libelle");
            //CB_Requete.ItemsSource = _SqlRequetes;

            TB_LibelleParametre1_TextChanged(null, null);
            TB_LibelleParametre2_TextChanged(null, null);
            TB_LibelleParametre3_TextChanged(null, null);
            TB_LibelleParametre4_TextChanged(null, null);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Nouveau_Click(object sender, RoutedEventArgs e)
        {
            IdRequete = int.MinValue;
            TB_Libelle.Text = "";
            TB_LibelleParametre1.Text = "";
            TB_LibelleParametre2.Text = "";
            TB_LibelleParametre3.Text = "";
            TB_LibelleParametre4.Text = "";
            TB_LibelleParametre1_TextChanged(null, null);
            TB_LibelleParametre2_TextChanged(null, null);
            TB_LibelleParametre3_TextChanged(null, null);
            TB_LibelleParametre4_TextChanged(null, null);
            TB_Requeteur.Text = "";
            //new TextRange(RTB_Requeteur.Document.ContentStart, RTB_Requeteur.Document.ContentEnd).Text = "";
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public String NomId
        {
            get
            {
                return null;
                //return ((Sql.Lit)DG_Resultats.ItemsSource)._Proprietes[0];
            }
        }

        public int Id
        {
            get
            {
                return 0;
                //return ((Sql.Lit)DG_Resultats.ItemsSource).Int32b(0);
            }
        }

        private void DG_Resultats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DG_Resultats.CurrentCell == null || !(DG_Resultats.CurrentCell.Item is ""/*Sql.Ligne*/))
                return;

            if (OnObjetSelect != null)
                OnObjetSelect(this, new EventArgs());
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Excel_Click(object sender, RoutedEventArgs e)
        {
            // Vérifie qu'il y a bien quelque chose à exporter
            if (DG_Resultats.Items.Count == 0)
            {
                MessageBox.Show("Aucun résultat à exporter !", "Exportation Excel", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Emplacement du fichier
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Export"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "Spérateur point-virgule (.csv)|*.csv"; // Filter files by extension
            if (dlg.ShowDialog() != true)
                return;

            // Les colonnes
            StringBuilder contenu = new StringBuilder(10000000);
            int nbColonnes = DG_Resultats.Columns.Count;
            for (int c = 0; c < nbColonnes - 1; c++)
                contenu.Append(ValeurPourCsv(DG_Resultats.Columns[c].Header) + ";");
            contenu.Append(ValeurPourCsv(DG_Resultats.Columns[nbColonnes - 1].Header) + "\r\n");


            // Les lignes
            //foreach (Sql.Ligne ligne in DG_Resultats.Items)
            //{
            //    for (int c = 1; c < nbColonnes; c++)
            //        contenu.Append(ValeurPourCsv(ligne[c]) + ";");
            //    contenu.Append(ValeurPourCsv(ligne[nbColonnes]) + "\r\n");
            //}

            // Enregistre le fichier
            File.WriteAllText(dlg.FileName, contenu.ToString(), Encoding.UTF8);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Rend la vleur compatible pour du CSV
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private string ValeurPourCsv(object value)
        {
            if (value == null) return "";
            //if(value is Nullable && ((INullable)value).IsNull) return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("dd/MM/yyyy");
                return ((DateTime)value).ToString("dd/MM/yyyy HH:mm:ss");
            }
            string output = value.ToString();

            if (output.Contains(";") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';

            return output;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * 
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Imprimer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
