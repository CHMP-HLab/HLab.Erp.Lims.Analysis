using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using HLab.Erp.Lims.Analysis.Module.Stats;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

using Npgsql;

namespace Outils;

/// <summary>
/// Logique d'interaction pour Requeteur.xaml
/// </summary>
public partial class StatQueryView : UserControl, IView<QueryViewModel> , IViewClassDocument
{
    public StatQueryView()
    {
        InitializeComponent();

        if (DesignerProperties.GetIsInDesignMode(this))
            return;

        G_Requete.Visibility = System.Windows.Visibility.Collapsed;
        RafraichieRequetes();

        this.DataContextChanged += StatQueryView_DataContextChanged;
    }

    void StatQueryView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if(e.OldValue is QueryViewModel oldVm)
        {
            oldVm.Columns.CollectionChanged -= Columns_CollectionChanged;
            DG_Resultats.Columns.Clear();
        }
        if(e.NewValue is QueryViewModel vm)
        {
            vm.Columns.CollectionChanged += Columns_CollectionChanged;
        }
    }

    void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch(e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach(var name in e.NewItems.OfType<string>()) AddColumn(name);
                break;
            case NotifyCollectionChangedAction.Remove:
                break;
            case NotifyCollectionChangedAction.Reset:
                DG_Resultats.Columns.Clear();
                break;
        }
    }

    void AddColumn(string name)
    {
        DataGridTextColumn colonne = new();
        colonne.Header = name;
        colonne.Binding = new Binding(name);
        colonne.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
        DG_Resultats.Columns.Add(colonne);
    }

    int IdRequete = int.MinValue;

    public event EventHandler OnObjetSelect;


    /********************************************************************************************************************************************************************************************************************************************************************************
    * 
    * 
    * 
    ***********************************************************************************************************************************************************************************************************************************************************************************/



    /********************************************************************************************************************************************************************************************************************************************************************************
    * 
    * 
    * 
    ***********************************************************************************************************************************************************************************************************************************************************************************/

    void DG_Resultats_CellMouseMove(object sender, MouseEventArgs e)
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

    void DG_Resultats_MouseLeave(object sender, MouseEventArgs e)
    {
        Pop_Cellule.IsOpen = false;
    }


    /********************************************************************************************************************************************************************************************************************************************************************************
    * 
    * 
    * 
    ***********************************************************************************************************************************************************************************************************************************************************************************/

    void CB_Requete_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

    void CK_VoirRequete_Checked(object sender, RoutedEventArgs e)
    {
        G_Requete.Visibility = CK_VoirRequete.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }


    /********************************************************************************************************************************************************************************************************************************************************************************
    * 
    * 
    * 
    ***********************************************************************************************************************************************************************************************************************************************************************************/

    void TB_LibelleParametre1_TextChanged(object sender, TextChangedEventArgs e)
    {
        TB_Parametre1.Tag = TB_LibelleParametre1.Text;
        TB_Parametre1.Visibility = TB_LibelleParametre1.Text.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }

    void TB_LibelleParametre2_TextChanged(object sender, TextChangedEventArgs e)
    {
        TB_Parametre2.Tag = TB_LibelleParametre2.Text;
        TB_Parametre2.Visibility = TB_LibelleParametre2.Text.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }

    void TB_LibelleParametre3_TextChanged(object sender, TextChangedEventArgs e)
    {
        TB_Parametre3.Tag = TB_LibelleParametre3.Text;
        TB_Parametre3.Visibility = TB_LibelleParametre3.Text.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }

    void TB_LibelleParametre4_TextChanged(object sender, TextChangedEventArgs e)
    {
        TB_Parametre4.Tag = TB_LibelleParametre4.Text;
        TB_Parametre4.Visibility = TB_LibelleParametre4.Text.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }


    /********************************************************************************************************************************************************************************************************************************************************************************
    * 
    * 
    * 
    ***********************************************************************************************************************************************************************************************************************************************************************************/

    void BT_Enregistrer_Click(object sender, RoutedEventArgs e)
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

    void RafraichieRequetes()
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

    void BT_Nouveau_Click(object sender, RoutedEventArgs e)
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

    void DG_Resultats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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


    /********************************************************************************************************************************************************************************************************************************************************************************
    * 
    * Rend la vleur compatible pour du CSV
    * 
    ***********************************************************************************************************************************************************************************************************************************************************************************/



    /********************************************************************************************************************************************************************************************************************************************************************************
    * 
    * 
    * 
    ***********************************************************************************************************************************************************************************************************************************************************************************/

    void BT_Imprimer_Click(object sender, RoutedEventArgs e)
    {

    }
}