using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

using Npgsql;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    using H = H<QueryViewModel>;

    public class QueryViewModel : ViewModel<Requete>
    {
        private DataService _data;
        public QueryViewModel(DataService data)
        {
            _data = data;

            H.Initialize(this);
        }

        public ObservableCollection<DataObject> Items {get;} = new ObservableCollection<DataObject>();
        public ObservableCollection<string> Columns {get;} = new ObservableCollection<string>();

        public string Param1 {get; set;}
        public string Param2 {get; set;}
        public string Param3 {get; set;}
        public string Param4 {get; set;}

        private void Export()
        {
            // Vérifie qu'il y a bien quelque chose à exporter
            if (Items.Count == 0)
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
            int nbColonnes = Columns.Count;
            for (int c = 0; c < nbColonnes - 1; c++)
                contenu.Append(CsvValue(Columns[c]) + ";");
            contenu.Append(CsvValue(Columns[nbColonnes - 1]) + "\r\n");

            // Les lignes
            foreach (var ligne in Items)
            {
                for (int c = 1; c < nbColonnes; c++)
                    contenu.Append(CsvValue(ligne[c]) + ";");
                contenu.Append(CsvValue(ligne[nbColonnes]) + "\r\n");
            }

            // Enregistre le fichier
            File.WriteAllText(dlg.FileName, contenu.ToString(), Encoding.UTF8);
        }

        private string CsvValue(object value)
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

        private string SetParam(string source, int num,string value)
        {
           Regex x = new Regex(@$"(/\*{num}\*/])(.*?)(/\*\*/)");
           return x.Replace(source, "$1"+value+"$3");
        }

        ICommand RunCommand {get; } = H.Command(c => c.Action(e => e.Run()));

        private void Run()
        {
            try
            {

                // Requete
                string requete = Model.Query;// new TextRange(RTB_Requeteur.Document.ContentStart, RTB_Requeteur.Document.ContentEnd).Text;
                requete = SetParam (requete, 1, Param1);
                requete = SetParam (requete, 2, Param2);
                requete = SetParam (requete, 3, Param3);
                requete = SetParam (requete, 4, Param4);

                requete = requete.Replace("\r", "").Replace("\n", " ");

                // Execute la requete
                //Sql.Lit lit = new Sql.Lit(requete);
                using var con = new NpgsqlConnection(_data.ConnectionString);
                Columns.Clear();
                con.Open();
                using var cmd = new NpgsqlCommand(Model.Query, con);
                var reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                    throw new Exception("Résultat vide");

                var cols = reader.GetColumnSchema();
                for (int i = 1; i < cols.Count; i++)
                {
                    Columns.Add(cols[i].ColumnName);
                }
                /*
                    DataGridTextColumn colonne = new DataGridTextColumn();
                    colonne.Header = cols[i].ColumnName;
                    colonne.Binding = new Binding(cols[i].ColumnName);
                    colonne.Width = new DataGridLength(1.0 / cols.Count, DataGridLengthUnitType.Star);
                    DG_Resultats.Columns.Add(colonne);
                 
                 */

                while (reader.Read())
                {
                    var ligne = new DataObject{
                        _Proprietes=cols.Select(c => c.ColumnName).ToArray(),
                        _Valeurs = new string[cols.Count]
                        };
                    for (int i = 1; i < cols.Count; i++)
                    {
                        ligne._Valeurs[i] = reader.GetFieldValue<string>(i);
                    }
                    Items.Add(ligne);
                }

                //TODO : L_NbResultats.Content = reader.RecordsAffected.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERREUR dans la requete", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


    public class DataObject : DynamicObject
        {
            public object[] _Valeurs;
            public string[] _Proprietes;

            public object this[string champ]
            {
                get
                {
                    // Recherche l'index du champ
                    for (int i = 0; i < _Proprietes.Length; i++)
                    {
                        // Si le nom de la propriété est trouvée, donne la valeur
                        if (_Proprietes[i] == champ)
                            return _Valeurs[i];
                    }

                    // Si il ne le trouve pas
                    return null;
                    //throw new Exception("Ligne : Champ innexistant !");
                }

                set
                {
                    // Recherche l'index du champ
                    for (int i = 0; i < _Proprietes.Length; i++)
                    {
                        // Si le nom de la propriété est trouvée, attribue nouvelle la valeur
                        if (_Proprietes[i] == champ)
                            _Valeurs[i] = value;
                    }
                }
            }

            public object this[int index]
            {
                get
                {
                    return _Valeurs[index];
                }

                set
                {
                    _Valeurs[index] = value;
                }
            }


            /********************************************************************************************************************************************************************************************************************************************************************************
            * 
            * Demande d'une valeur de proprieté (pour binding par exemple)
            * 
            ***********************************************************************************************************************************************************************************************************************************************************************************/

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                // Recherche l'index du champ
                for (int i = 0; i < _Proprietes.Length; i++)
                {
                    // Si le nom de la propriété est trouvée
                    if (_Proprietes[i] == binder.Name)
                    {
                        // Donne la valeur
                        result = _Valeurs[i];
                        return true;
                    }
                }

                // La propriété n'a pas été trouvée
                result = null;
                return false;
            }


            /********************************************************************************************************************************************************************************************************************************************************************************
            * 
            * Attribution d'une valeur de proprieté (pour binding par exemple)
            * 
            ***********************************************************************************************************************************************************************************************************************************************************************************/
            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                // Recherche l'index du champ
                for (int i = 0; i < _Proprietes.Length; i++)
                {
                    // Si le nom de la propriété est trouvée
                    if (_Proprietes[i] == binder.Name)
                    {
                        // Donne la valeur
                        _Valeurs[i] = value;
                        return true;
                    }
                }

                // La propriété n'a pas été trouvée
                return true;
            }

        }




}
