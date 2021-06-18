/********************************************************************************************************************************************************************************************************************************************************************************
* 
*
* 
* Langue : ex: {FR=Texte}{US=Text}
* Table Xaml : Nom - VARCHAR(255), Page - MEDIUMTEXT, Element - MEDIUMTEXT
* 
***********************************************************************************************************************************************************************************************************************************************************************************/

using PdfSharp.Pdf;
using PdfSharp.Xps.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Xml;
using HLab.Base;
using Nito.Disposables.Internals;
using Type = System.Type;


namespace Outils
{
    /// <summary>
    /// Logique d'interaction pour Impression.xaml
    /// </summary>
    public partial class Print : Window
    {
        /*##################################################################################################################################################################################################################################################################################################################################################################
        ## 
        ## DECLARATIONS
        ##
        ####################################################################################################################################################################################################################################################################################################################################################################*/

        public static String CheminDocument = null;
        public static MailAddress Expediteur = null;
        public static String SmtpServeur = null;
        public static int SmtpPort = 25;
        public static String SmtpUtilisateur = null;
        public static String SmtpMotDePasse = null;
        public ElementInterne Element = new ElementInterne();

        private String _Langue = null;
        private String _XamlPage = null;
        private Hashtable _XamlElement = new Hashtable();
        private String _XamlElementCourant = null;
        private List<Page> _Page = new List<Page>();
        private List<UIElement> _Elements = new List<UIElement>();
        private List<ItemImprimante> _Imprimante = new List<ItemImprimante>();
        private int _PageCourante = 0;
        private Page _PageFrame = new Page();
        private String _Dossier = null;
        private String _CheminDefaut = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private ObservableCollection<ItemAdresse> _Destinataire = new ObservableCollection<ItemAdresse>();

        private const double _CoeffRichTextBox = 2.6 / 12.0;


        /*##################################################################################################################################################################################################################################################################################################################################################################
        ## 
        ## PROPRIETES
        ##
        ####################################################################################################################################################################################################################################################################################################################################################################*/

        public string XamlPage => _XamlPage;
        public Hashtable XamlElement => _XamlElement;


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public object this[String champ]
        {
            set => RemplaceChamp(ref _XamlPage, champ, value?.ToString() ?? "");
        }

        public void SetData(object data)
        {
            var fields = Fields(_XamlPage).ToList();

            foreach (var field in fields)
            {

                if (field.Contains('|'))
                {
                    var p1 = field.Split('|');
                    if (GetValue(data, p1[0], out var value))
                    {
                        var p2 = p1[1].Split("=>");
                        if(p2[0]==value)
                            RemplaceChamp(ref _XamlPage, field,p2[1]);
                        else
                        {
                            RemplaceChamp(ref _XamlPage, field,"");
                        }
                    }
                }
                else
                {
                    if (GetValue(data, field, out var value))
                    {
                        RemplaceChamp(ref _XamlPage, field,value);
                    }
                }
            }
        }

        private static bool GetValue(object data, string[] path, out string value)
        {
            var i = 0;
            while (data != null && i<path.Length)
            {
                var property = data.GetType().GetProperty(path[i]);
                if (property == null)
                {
                    value = $"{{{string.Join('.',path)}}}";
                    return false;
                }
                data = property.GetValue(data);
                i++;
            }

            value = data switch
            {
                string s => s,
                DateTime d => d.ToString("dd/MM/yyyy"),
                _ => data?.ToString() ?? ""
            };
            return true;
        }

        private static bool GetValue(object data, string path, out string value) => GetValue(data,path.Split('.'), out value);

        private static IEnumerable<string> Fields(string source)
        {
            foreach (var match in Regex.Matches(source, @"\{(.*?)\}").WhereNotNull())
            {
                if (match.Groups.Count > 1)
                {
                    var field = match.Groups[1].Value;
                    yield return field;
                }
            }
        }

        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private int AffichePage
        {
            set
            {
                if (value == _PageCourante)
                    return;

                if (value < 1)
                    value = 1;

                if (value > _Page.Count)
                    value = _Page.Count;

                // Reattribue le contenu de la précédement affichée
                if (_PageCourante > 0)
                    ReattribuePage();

                // Change l'index de la page courante
                _PageCourante = value;
                TB_Page.Double = _PageCourante;

                // Création de la nouvelle page
                _PageFrame.Width = _Page[_PageCourante - 1].Width;
                _PageFrame.Height = _Page[_PageCourante - 1].Height;
                _PageFrame.Resources = _Page[_PageCourante - 1].Resources;

                // Transfert du contenu de la page à afficher dans la page de la frame pour éviter les soucis de mise en page suite l'utilisation de Frame
                object contenu = _Page[_PageCourante - 1].Content;
                _Page[_PageCourante - 1].Content = null;
                _PageFrame.Content = contenu;
            }
            get
            {
                return _PageCourante;
            }
        }


        /*##################################################################################################################################################################################################################################################################################################################################################################
        ## 
        ## METHODES
        ##
        ####################################################################################################################################################################################################################################################################################################################################################################*/


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Constructeur
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public Print(String nomXaml, string template, String langue)
        {
            _Langue = langue;

            InitializeComponent();
            Element._Impression = this;

            // Prépare la liste des imprimantes
            LocalPrintServer printServer = new LocalPrintServer();
            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });

            // Imprimante par défaut
            String defaut = printServer.DefaultPrintQueue.FullName;
            int indexDefaut = 0;

            // Récupère les imprimantes
            int i = 0;
            foreach (PrintQueue pq in printQueuesOnLocalServer)
            {
                if (pq.FullName == defaut)
                {
                    indexDefaut = i;
                    _Imprimante.Add(new ItemImprimante(pq, true));
                }
                else
                    _Imprimante.Add(new ItemImprimante(pq));
                i++;
            }

            // Affiche les imprimantes
            LB_Imprimantes.ItemsSource = _Imprimante;
            LB_Imprimantes.SelectedIndex = indexDefaut;

            // Ajoute le premier document
            if (nomXaml != "")
                AjoutePage(nomXaml, template);

            F_Apercu.Content = _PageFrame;
            LB_Destinataire.ItemsSource = _Destinataire;
            RB_Pdf.IsChecked = true;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Ajoute un module utilisateur externe
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void AjouteModule(UIElement module)
        {
            DockPanel.SetDock(module, Dock.Top);
            DP_Panneau.Children.Insert(0, module);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Vide toutes les pages
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void Vide()
        {
            _XamlPage = null;
            _XamlElementCourant = null;
            _PageCourante = 0;
            _Page.Clear();
            _Elements.Clear();
            _XamlElement.Clear();
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * nom          : Nom du document
        * dossier      : Dossier où tout doit être archiver
        * destinataire : Destinataire(s) de l'email. Si plusieurs, les séparer par des retours à la ligne
        * objet        : Object du message
        * message      : Corps du message
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public bool Apercu(string nom = "Impression", string dossier = null, string obj = "", FlowDocument message = null)
        {
            // Récupère les valeurs
            TB_Nom.Text = nom;
            _Dossier = dossier;
            _Destinataire.Add(new ItemAdresse());
            TB_Objet.Text = obj;
            TB_Chemin.Text = _CheminDefaut;
            if (message != null)
                RTB_Message.Document = message;

            // Finalise pour afficher les pages
            Finalise();


            // Affiche la fenêtre
            return ShowDialog() == true ? true : false;
        }

        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Finalise pour afficher les pages
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void Finalise()
        {
            // Calcule le dernier élément
            CalculerElement();

            // Calcule la page en cours
            CalculerPage();

            // Remplace les totaux de numéros de pages
            foreach (Page page in _Page)
            {
                if(page.Content is DependencyObject dependency)
                    PagesTotales(dependency);
            }

            // Applique le nombre de page total
            T_Pages.Text = "/ " + _Page.Count;

            // Affiche la première page
            AffichePage = 1;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void AjoutePage(String nomXaml, string page)
        {
            // Calcule le dernier élément
            CalculerElement();

            // Calcule la page en cours
            CalculerPage();

            // Charge le fichier xaml de la page

            const string sStart = "<!--Detail.Start-->";
            const string sEnd = "<!--Detail.End-->";

            var detailStart = page.IndexOf(sStart);
            var detailStop = page.IndexOf(sEnd) + sEnd.Length;

            var element = page.Substring(detailStart, detailStop - detailStart);

            page = page.Replace(element, "");

            element = element.Replace(sStart, "").Replace(sEnd, "");
            _XamlPage = page;

            // Distingue les différents éléments
            _XamlElement.Clear();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml("<Grid xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" + element + "</Grid>");

            foreach (XmlNode xn in xd.FirstChild)
            {
                if (xn.Attributes != null)
                {
                    XmlAttribute xa = xn.Attributes["Name"];
                    _XamlElement.Add(xa == null ? "" : xa.InnerText, xn.OuterXml);
                }
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void CalculerPage()
        {
            if (_XamlPage == null)
                return;

            // Applique la langue
            _XamlPage = Langue(_XamlPage, _Langue);

            // Cree la page -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // Charge la première page avec son numéro de page
            Page page = CreatePage(_XamlPage.Replace("{Page}", (_Page.Count + 1).ToString()));

            // Si il n'y a pas d'éléménts
            if (_Elements.Count == 0)
            {
                // Ajoute cette page dans la liste
                _Page.Add(page);
                return;
            }

            // Index de l'élément à placer
            int iElement = 0;

            // Recherche le panneau contenant les éléments dynamiques
            Panel panelContenu = ((Panel)page.FindName("PanelContenu"));
            double hPanel = panelContenu.RenderSize.Height;
            double cumul = 0.0;
            int iDernier = -1;


            // Place les éléments -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            while (iElement < _Elements.Count)
            {
                // Ajoute l'élément courant si il n'a pas déjà été ajouté
                if (iElement != iDernier)
                {
                    // Ajoute l'élément courant
                    panelContenu.Children.Add(_Elements[iElement]);

                    // Recalcule son placement
                    _Elements[iElement].UpdateLayout();

                    // L'ajoute sa hauteur aux précédents
                    if (_Elements[iElement] is RichTextBox)
                        cumul += _Elements[iElement].RenderSize.Height * _CoeffRichTextBox;
                    else
                        cumul += _Elements[iElement].RenderSize.Height;
                }

                // Vérifie si il passe
                if (cumul <= hPanel)
                    iElement++;

                // Ca ne passe plus
                else
                {
                    // Le pied de page n'a pas encore été retiré
                    if (iDernier == -1)
                    {
                        // Retire les éléments uniquement sur la derniere page
                        CacheDernier(page);
                        page.UpdateLayout();
                        hPanel = panelContenu.RenderSize.Height;
                        iDernier = iElement;
                    }

                    // Les éléments uniquement sur la derniere page ont déjà été retirés
                    else
                    {
                        // Retire l'élément qui ne passe pas
                        panelContenu.Children.Remove(_Elements[iElement]);

                        // Ne l'inclut pas dans la prochaine page si c'est un séparateur
                        if (((FrameworkElement)_Elements[iElement]).Name == "Separateur")
                            iElement++;

                        // Ajoute cette page dans la liste
                        _Page.Add(page);

                        // Charge la page avec son numéro de page et cache "PremierePageUniquement" car ce n'est pas la première page
                        page = CreatePage(_XamlPage.Replace("{Page}", (_Page.Count + 1).ToString()).Replace("Tag=\"PremierePageUniquement\"", "Visibility=\"Collapsed\""));

                        // Recherche le panneau contenant les éléments dynamiques
                        panelContenu = ((Panel)page.FindName("PanelContenu"));
                        hPanel = panelContenu.RenderSize.Height;
                        cumul = 0.0;
                        iDernier = -1;
                    }
                }
            }

            // Ajoute cette page dans la liste
            _Page.Add(page);

            // Si ce n'était pas la dernière page, l'ajoute
            if (iDernier > 0)
                _Page.Add(CreatePage(_XamlPage.Replace("{Page}", (_Page.Count + 1).ToString()).Replace("Tag=\"PremierePageUniquement\"", "Visibility=\"Collapsed\"")));

            // Vide la liste des éléments
            _Elements.Clear();
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void AjouteElement(UIElement element)
        {
            // Calcul le dernier élément
            CalculerElement();

            // Ajoute les elements
            if (element != null)
                _Elements.Add(element);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void AjouteElement(UIElement[] elements)
        {
            // Calcul le dernier élément
            CalculerElement();

            // Ajoute les elements
            if (elements != null)
                _Elements.AddRange(elements);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Rien si il n'y a qu'un element unique dans l'xaml, sinon le nom de l'élémént
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void AjouteElement(String element = "")
        {
            // Calcul le dernier élément
            CalculerElement();

            // Charge l'élément
            _XamlElementCourant = (String)_XamlElement[element];
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Rien si il n'y a qu'un element unique dans l'xaml, sinon le nom de l'élémént
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void AjouteElement(FlowDocument document)
        {
            // Calcul le dernier élément
            CalculerElement();

            foreach (Block block in document.Blocks)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //Console.WriteLine(block.GetType());

                    // Copie le code rtf du block en mémoire
                    new TextRange(block.ContentStart, block.ContentEnd).Save(ms, DataFormats.Rtf);

                    // Crée un nouveau paragraphe en fonction du block en mémoire
                    Paragraph paragraphe = new Paragraph();
                    new TextRange(paragraphe.ContentStart, paragraphe.ContentEnd).Load(ms, DataFormats.Rtf);
                    paragraphe.Margin = block.Margin;

                    // Crée le Flowdocument avec cette copie de paragraphe
                    FlowDocument doc = new FlowDocument();
                    doc.Blocks.Add(paragraphe);

                    // Paragraphe vide servant pour appliquer le margin du paragraphe principale = bidouille encore !
                    Paragraph marge = new Paragraph();
                    marge.Margin = new Thickness();
                    marge.FontSize = 0.01;
                    marge.LineHeight = 0.01;
                    doc.Blocks.Add(marge);

                    // Crée et ajoute à la liste un nouveau RichTextBox contenant ce flowdocument
                    RichTextBox rtb = new RichTextBox();
                    rtb.BorderThickness = new Thickness(0.0);
                    rtb.Document = doc;
                    rtb.LayoutTransform = new ScaleTransform(_CoeffRichTextBox, _CoeffRichTextBox);
                    rtb.Margin = new Thickness(3, 0, 0, 0);
                    _Elements.Add(rtb);
                }
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void CalculerElement()
        {
            if (_XamlElementCourant == null)
                return;

            // Applique la langue
            _XamlElementCourant = Langue(_XamlElementCourant, _Langue);

            // Cree l'UIElement depuis l'Xaml
            _Elements.Add((UIElement)XamlReader.Parse(_XamlElementCourant));
            _XamlElementCourant = null;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void CacheDernier(DependencyObject depObj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                if (child != null && child is FrameworkElement)
                {
                    object tag = ((FrameworkElement)child).Tag;
                    if (tag != null && tag.ToString() == "DernierePageUniquement")
                        ((FrameworkElement)child).Visibility = Visibility.Collapsed;
                }

                CacheDernier(child);
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void PagesTotales(DependencyObject dependency)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependency); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dependency, i);

                if (child is TextBlock textBlock)
                    textBlock.Text = textBlock.Text.Replace("{NbPages}", _Page.Count.ToString());

                PagesTotales(child);
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private Page CreatePage(string xaml)
        {
            try
            {
                using var s = new StringReader(xaml);
                var xmlReader = XmlReader.Create(s);
                if (XamlReader.Load(xmlReader) is Page page)
                {
                    //page.Measure(new Size(210, 297));
                    //page.Arrange(new Rect(new Point(0, 0), new Size(210, 297)));

                    return page;
                }

                return new Page { Content = new TextBlock { Text = xaml } };

            }
            catch (XamlParseException ex)
            {
                return new Page { Content = new TextBlock { Text = ex.Message } };
            }

            // Applique les redimensionnement pour connaitre les tailles
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Remplace un champ par sa valeur formatée
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public static void RemplaceChamp(ref String xaml, String champ, String valeur)
        {
            valeur = valeur.Replace("&", "&amp;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;").Replace("\r", "").Replace("\n", "&#10;");
            xaml = xaml.Replace("{" + champ + "}", valeur);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Cache un element grace à son tag
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void Cache(String tag)
        {
            CacheTag(ref _XamlPage, tag);
        }

        public void CacheElement(String tag)
        {
            CacheTag(ref _XamlElementCourant, tag);
        }

        public static void CacheTag(ref String xaml, String tag)
        {
            xaml = xaml.Replace("Tag=\"" + tag + "\"", "Visibility=\"Collapsed\"");
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Remplace les éléments de la langue choisie
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public static String Langue(String texte, String Langue)
        {
            if (texte == null) return "";
            return Regex.Replace(Regex.Replace(texte, @"\{" + Langue + @"=([\s|!-\|~-■]*)}", "$1"), @"\{..=[\s|!-\|~-■]*}", "");
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void G_Font_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double largeur = e.NewSize.Height * 210.0 / 297.0;
            if (largeur > e.NewSize.Width - 100.0)
                largeur = e.NewSize.Width - 100.0;

            G_Font.ColumnDefinitions[1].Width = new GridLength(largeur);

            //F_Apercu.RenderTransform = new ScaleTransform(largeur/210.0, largeur/210.0);
            F_Apercu.LayoutTransform = new ScaleTransform(largeur / 210.0, largeur / 210.0);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Imprimer_Click(object sender, RoutedEventArgs e)
        {
            LogText += "Impression du certificat d'analyse" + TB_Chemin.Text;
            // Ferme la fenêtre
            DialogResult = true;
            Close();

            // Reattribue le contenu de la précédement affichée
            ReattribuePage();

            // Lance l'impression
            PrintQueue imprimante = LB_Imprimantes.SelectedItem == null ? LocalPrintServer.GetDefaultPrintQueue() : ((ItemImprimante)LB_Imprimantes.SelectedItem)._Pq;
            imprimante.CurrentJobSettings.Description = _Page[0].Title;
            XpsDocumentWriter xpsdw = PrintQueue.CreateXpsDocumentWriter(imprimante);
            if (RB_Tout.IsChecked == true)
                xpsdw.Write(new Pagination(_Page));
            else if (RB_PageCourante.IsChecked == true)
                xpsdw.Write(new Pagination(_Page, (int)TB_Page.Double));
            else
                xpsdw.Write(new Pagination(_Page, (int)TB_PageDebut.Double, (int)TB_PageFin.Double));

            // Archive le fichier
            Fichier();
        }

        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void ReattribuePage()
        {
            object contenu = _PageFrame.Content;
            _PageFrame.Content = null;
            _Page[_PageCourante - 1].Content = contenu;
            _Page[_PageCourante - 1].UpdateLayout();
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private MemoryStream Fichier(bool pdf = false)
        {
            // Crée le fichier Xps en mémoire
            MemoryStream ms = new MemoryStream();
            Package package = Package.Open(ms, FileMode.OpenOrCreate);
            XpsDocument xps = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xps);
            writer.Write(new Pagination(_Page));
            xps.Close();
            package.Close();

            // Si il faut archiver
            if (_Dossier != null)
            {
                try
                {
                    // Chemin du fichier
                    if (!Directory.Exists(CheminDocument + _Dossier))
                        Directory.CreateDirectory(CheminDocument + _Dossier);

                    // Ecrit le fichier
                    File.WriteAllBytes(CheminDocument + _Dossier + "\\" + TB_Nom.Text + ".xps", ms.ToArray());
                }
                catch
                {
                    MessageBox.Show("Impossible de créer le fichier " + CheminDocument + _Dossier + "\\" + TB_Nom.Text + ".xps", "Erreur d'archivage", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Retourne le flux
            if (pdf)
                return XpsToPdf(ms);
            else
                return ms;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public static MemoryStream XpsToPdf(Stream flux)
        {
            PdfSharp.Xps.XpsModel.FixedDocument fixedDocument = PdfSharp.Xps.XpsModel.XpsDocument.Open(flux).GetDocument();
            PdfDocument pdfDocument = new PdfDocument();
            PdfRenderer renderer = new PdfRenderer();

            int pageIndex = 0;
            foreach (PdfSharp.Xps.XpsModel.FixedPage page in fixedDocument.Pages)
            {
                if (page == null)
                    continue;

                PdfPage pdfPage = renderer.CreatePage(pdfDocument, page);
                renderer.RenderPage(pdfPage, page);
                pageIndex++;
            }

            MemoryStream ms = new MemoryStream();
            pdfDocument.Save(ms, false);
            return ms;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public void AjouteDestinataire(String adresse, String nom)
        {
            _Destinataire.Add(new ItemAdresse(adresse, nom));
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void Adresse_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Cherche si il y a des adresses vides
            ItemAdresse vide = null;
            int nbVide = 0;
            foreach (ItemAdresse destinataire in _Destinataire)
            {
                if (destinataire.Adresse == "" && destinataire.Nom == "")
                {
                    nbVide++;
                    if (vide == null)
                        vide = destinataire;
                }
            }

            // Si il n'y en a pas, en ajoute une
            if (nbVide == 0)
                _Destinataire.Add(new ItemAdresse());

            // Si il y en a trop, supprime la première
            if (nbVide > 1)
                _Destinataire.Remove(vide);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public String LogText = "";

        private void BT_Email_Click(object sender, RoutedEventArgs e)
        {
            // Prépare le message
            MailMessage mail = new MailMessage();
            mail.From = Expediteur;
            mail.Subject = TB_Objet.Text;

            // Vérifie les adresses mail
            foreach (ItemAdresse destinataire in _Destinataire)
            {
                if (destinataire.Adresse == "" || !EmailTools.IsValid(destinataire.Adresse))
                {
                    MessageBox.Show("E-mail " + destinataire.Adresse + " non valide !", "Envoie impossible", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                mail.To.Add(new MailAddress(destinataire.Adresse, destinataire.Nom));
            }

            LogText += "Envoi par eMail à :";
            foreach (var m in mail.To)
            {
                LogText += " " + m.Address;
            }

            // Ferme la fenêtre
            DialogResult = true;
            Close();

            // Reattribue le contenu de la précédement affichée
            ReattribuePage();

            // Corps du message
            MemoryStream msc = new MemoryStream();
            new TextRange(RTB_Message.Document.ContentStart, RTB_Message.Document.ContentEnd).Save(msc, DataFormats.Xaml);
            mail.IsBodyHtml = true;
            mail.Body = HTMLConverter.HtmlFromXamlConverter.ConvertXamlToHtml("<FlowDocument>" + Encoding.UTF8.GetString(msc.ToArray()) + "</FlowDocument>");
            msc.Close();

            // Joint le document
            MemoryStream ms = Fichier((bool)RB_Pdf.IsChecked);
            ms.Position = 0;
            mail.Attachments.Add(new Attachment(ms, TB_Nom.Text + (RB_Pdf.IsChecked == true ? ".pdf" : ".xps"), System.Net.Mime.MediaTypeNames.Application.Pdf));

            // Joint les autres fichiers
            foreach (ItemPj pj in LB_PieceJointe.Items)
                mail.Attachments.Add(new Attachment(pj.CheminComplet));

            // Prépare la connexion au serveur Smtp
            SmtpClient smtp = new SmtpClient(SmtpServeur, SmtpPort);
            smtp.Credentials = new NetworkCredential(SmtpUtilisateur, SmtpMotDePasse);
            //smtp.EnableSsl = true;
            //smtp.Timeout = 100;

            try
            {
                // Envoie le message
                smtp.Send(mail);

                // Enregistre l'email
                if (_Dossier != null)
                {
                    Assembly assembly = typeof(SmtpClient).Assembly;
                    Type _mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

                    using (FileStream _fileStream = new FileStream(CheminDocument + _Dossier + DateTime.Now.ToString("yyyyMMddHHmmss_") + TB_Nom.Text + ".eml", FileMode.Create))
                    {
                        // Get reflection info for MailWriter contructor
                        ConstructorInfo _mailWriterContructor = _mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(Stream) }, null);

                        // Construct MailWriter object with our FileStream
                        object _mailWriter = _mailWriterContructor.Invoke(new object[] { _fileStream });

                        // Get reflection info for Send() method on MailMessage
                        MethodInfo _sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                        // Call method passing in MailWriter
                        _sendMethod.Invoke(mail, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { _mailWriter, true, true }, null);

                        // Finally get reflection info for Close() method on our MailWriter
                        MethodInfo _closeMethod = _mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                        // Call close method
                        _closeMethod.Invoke(_mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur d'envoie de l'email : " + ex.Message);
            }

            Console.WriteLine("email ok");
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Fichier_Click(object sender, RoutedEventArgs e)
        {
            if (TB_Chemin.Text != null)
                LogText += "Sauvegarde fichier : " + TB_Chemin.Text;
            // Ferme la fenêtre
            DialogResult = true;
            Close();

            // Reattribue le contenu de la précédement affichée
            ReattribuePage();

            // Enregistre le fichier
            if (TB_Chemin.Text != null)
            {
                // Chemin du fichier
                if (!Directory.Exists(TB_Chemin.Text))
                    Directory.CreateDirectory(TB_Chemin.Text);

                // Ecrit le fichier
                File.WriteAllBytes(TB_Chemin.Text + "\\" + TB_Nom.Text + (RB_Pdf.IsChecked == true ? ".pdf" : ".xps"), Fichier((bool)RB_Pdf.IsChecked).ToArray());
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void LB_PieceJointe_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void LB_PieceJointe_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] fichiers = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String fichier in fichiers)
                    LB_PieceJointe.Items.Add(new ItemPj(fichier));

                if (LB_PieceJointe.Items.Count > 0)
                    T_InfoPj.Visibility = System.Windows.Visibility.Collapsed;
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void SupprimerPj_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(sender);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Precedent_Click(object sender, RoutedEventArgs e)
        {
            AffichePage--;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void BT_Suivante_Click(object sender, RoutedEventArgs e)
        {
            AffichePage++;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void TB_Page_DoubleChange(object sender, RoutedEventArgs e)
        {
            AffichePage = (int)TB_Page.Double;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void TB_PageSelection_DoubleChange(object sender, RoutedEventArgs e)
        {
            RB_Plage.IsChecked = true;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private void RB_Pdf_Checked(object sender, RoutedEventArgs e)
        {
            BT_Fichier.Tag = "Pdf";
        }

        private void RB_Xps_Checked(object sender, RoutedEventArgs e)
        {
            BT_Fichier.Tag = "Xps";
        }


        /*##################################################################################################################################################################################################################################################################################################################################################################
        ## 
        ## CLASSES
        ##
        ####################################################################################################################################################################################################################################################################################################################################################################*/

        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        public class ElementInterne
        {
            public Print _Impression = null;

            public object this[String champ]
            {
                set
                {
                    if (_Impression._XamlElementCourant != null)
                        RemplaceChamp(ref _Impression._XamlElementCourant, champ, value.ToString());
                }
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private class ItemImprimante
        {
            public PrintQueue _Pq = null;
            BitmapImage _Defaut = null;

            public String Nom
            {
                get
                {
                    return _Pq.Name;
                }
            }

            public BitmapImage Defaut
            {
                get
                {
                    return _Defaut;
                }
            }

            public ItemImprimante(PrintQueue pq, bool defaut = false)
            {
                _Pq = pq;

                if (defaut)
                    _Defaut = new BitmapImage(new Uri("/Images/Valider.png", UriKind.Relative));
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private class ItemAdresse
        {
            public String Adresse { get; set; }
            public String Nom { get; set; }

            public ItemAdresse(String adresse = "", String nom = "")
            {
                Adresse = adresse;
                Nom = nom;
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private class ItemPj
        {
            //BitmapImage _TypeFichier = null;

            public String CheminComplet = null;
            public String Nom { get; set; }
            public String Taille { get; set; }

            //public BitmapImage TypeFichier
            //{
            //   get
            //   {
            //      return _TypeFichier;
            //   }
            //}

            public ItemPj(String nom)
            {
                CheminComplet = nom;
                FileInfo fi = new FileInfo(nom);
                Nom = fi.Name;
                Taille = O.FormatTaille(fi.Length);
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        *
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        private class Pagination : DocumentPaginator
        {
            private List<Page> _Page = null;
            private int _PremierePage = 0;
            private int _NbPages = 0;

            public Pagination(List<Page> page, int premierePage = 0, int dernierePage = 0)
            {
                _Page = page;

                if (premierePage == 0 && dernierePage == 0)
                {
                    _PremierePage = 1;
                    _NbPages = _Page.Count;
                }
                else
                {
                    if (premierePage < 1)
                        premierePage = 1;
                    if (premierePage > _Page.Count)
                        premierePage = _Page.Count;

                    if (dernierePage < premierePage)
                        dernierePage = premierePage;
                    if (dernierePage > _Page.Count)
                        dernierePage = _Page.Count;

                    _PremierePage = premierePage;
                    _NbPages = dernierePage - premierePage + 1;
                }
            }

            public override bool IsPageCountValid
            {
                get { return true; }
            }

            public override int PageCount
            {
                get
                {
                    return _NbPages;
                }
            }

            public override Size PageSize { get; set; }

            public override IDocumentPaginatorSource Source
            {
                get { return null; }
            }

            public override DocumentPage GetPage(int pageNumber)
            {
                // Récupère la page
                Page page = _Page[pageNumber + _PremierePage - 1];
                PageSize = new Size(page.Width * 96.0 / 25.4, page.Height * 96.0 / 25.4);

                // Réajuste les dimensions de la page en fonction de la surface d'impression
                page.LayoutTransform = new ScaleTransform(PageSize.Width / page.Width, PageSize.Height / page.Height);
                page.Measure(PageSize);
                page.Arrange(new Rect(new Point(0, 0), PageSize));

                // Retourne la page à imprimer
                return new DocumentPage(page, PageSize, new Rect(new Point(0, 0), PageSize), new Rect(new Point(0, 0), PageSize));
            }
        }
    }
}
