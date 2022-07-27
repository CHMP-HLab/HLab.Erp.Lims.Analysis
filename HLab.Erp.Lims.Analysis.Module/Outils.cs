/* 21/05/2013 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using HLab.Base.Wpf;

namespace Outils
{
   //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
   // Langue
   public enum Langue
   {
      /// <summary>
      /// 
      /// </summary>         
      Neutre = 0,

      /// <summary>
      /// 
      /// </summary>
      Anglais = 1,

      /// <summary>
      /// 
      /// </summary>
      Francais = 2,
   };   
   
   public static class O
   {
      public static double Moyenne(params double[] v)
      {
         double somme = 0.0;
         for(int i=0; i<v.Length; i++)
            somme += v[i];
         return somme / v.Length;
      }

      public static double EcartType(params double[] v)
      {
         double moyenne = Moyenne(v);
         double somme = 0.0;
         for(int i=0; i<v.Length; i++)
         {
            double delta = v[i] - moyenne;
            somme += delta * delta;
         }
         return Math.Sqrt(somme/(v.Length-1));
      }

      public static double Mini(params double[] v)
      {
         double mini = double.MaxValue;
         bool na = true;
         for(int i=0; i<v.Length; i++)
            if(!double.IsInfinity(v[i]) && !double.IsNaN(v[i]) && v[i] < mini)
            {
               na = false;
               mini = v[i];
            }

         if(na)
            return double.NaN;

         return mini;
      }

      public static double Maxi(params double[] v)
      {
         double maxi = double.MinValue;
         bool na = true;
         for(int i=0; i<v.Length; i++)
            if(!double.IsInfinity(v[i]) && !double.IsNaN(v[i]) && v[i] > maxi)
            {
               na = false;
               maxi = v[i];
            }

         if(na)
            return double.NaN;

         return maxi;
      }
      
      public static int CompteLigneString(String texte)
      {
         int taille = texte.Length;
         int nb = taille==0?0:1;
         for(int i=0; i<taille; i++)
            if(texte[i] == '\n')
               nb++;
         return nb;
      }

      public static Color ExtraireCouleur(object valeur)
      {
         if(valeur == null)
            return Colors.Red;

         if(valeur is SolidColorBrush)
            return ((SolidColorBrush)valeur).Color;
         else if(valeur is LinearGradientBrush)
         {
            if(((LinearGradientBrush)valeur).GradientStops.Count < 1)
               return Colors.Red;
            else
               return ((LinearGradientBrush)valeur).GradientStops[0].Color;
         }
         else
            return Colors.Red;
      }

      public static Color Luminance(Color couleur, int gain)
      {
         // Rouge
         int r = couleur.R + gain;
         if(r < 0)
            r = 0;
         if(r > 255)
            r = 255;

         // Vert
         int g = couleur.G + gain;
         if(g < 0)
            g = 0;
         if(g > 255)
            g = 255;

         // Bleu
         int b = couleur.B + gain;
         if(b < 0)
            b = 0;
         if(b > 255)
            b = 255;

         return Color.FromArgb(couleur.A, (byte)r, (byte)g, (byte)b);
      }

      public static Color Luminance2(Color couleur, int gain)
      {
         double t = 0.0;
         double s = 0.0;
         double l = 0.0;
         RVB_TSL(couleur.R, couleur.G, couleur.B, ref t, ref s, ref l);

         l += (double)gain / 255.0;
         if(l < 0.0) l = 0.0;
         if(l > 1.0) l = 1.0;

         // Color TSL_RVB(double t, double s, double l)

         double r;
         double v;
         double b;
         double var_1;
         double var_2;

         if(s == 0.0)                       //HSL from 0 to 1
         {
            r = l * 255.0;                      //RGB results from 0 to 255
            v = l * 255.0;
            b = l * 255.0;
         }
         else
         {
            if(l < 0.5)
               var_2 = l * (1.0 + s);
            else
               var_2 = (l + s) - (s * l);

            var_1 = 2.0 * l - var_2;

            r = 255.0 * Hue_2_RGB(var_1, var_2, t + (1.0 / 3.0));
            v = 255.0 * Hue_2_RGB(var_1, var_2, t);
            b = 255.0 * Hue_2_RGB(var_1, var_2, t - (1.0 / 3.0));
         }

         return Color.FromArgb(couleur.A, (byte)r, (byte)v, (byte)b);
      }

      public static Color Alpha(Color couleur, int alpha)
      {
         return Color.FromArgb((byte)alpha, couleur.R, couleur.G, couleur.B);
      }

      public static Color TSL_RVB(double t, double s, double l)
      {
         double r;
         double v;
         double b;
         double var_1;
         double var_2;

         if(s == 0.0)                       //HSL from 0 to 1
         {
            r = l * 255.0;                      //RGB results from 0 to 255
            v = l * 255.0;
            b = l * 255.0;
         }
         else
         {
            if(l < 0.5)
               var_2 = l * (1.0 + s);
            else
               var_2 = (l + s) - (s * l);

            var_1 = 2.0 * l - var_2;

            r = 255.0 * Hue_2_RGB(var_1, var_2, t + (1.0 / 3.0));
            v = 255.0 * Hue_2_RGB(var_1, var_2, t);
            b = 255.0 * Hue_2_RGB(var_1, var_2, t - (1.0 / 3.0));
         }

         /*
         double y = (0.3 * r + 0.59 * v + 0.11 * b) / 255.0;
         r *= l / y;
         v *= l / y;
         b *= l / y;
         if(r > 255.0) r = 255.0;
         if(v > 255.0) v = 255.0;
         if(b > 255.0) b = 255.0;
         */


         return Color.FromArgb((byte)255, (byte)r, (byte)v, (byte)b);
      }

      public static double Hue_2_RGB(double v1, double v2, double vH)             //Function Hue_2_RGB
      {
         if(vH < 0.0) vH += 1.0;
         if(vH > 1.0) vH -= 1.0;
         if((6.0 * vH) < 1.0) return (v1 + (v2 - v1) * 6.0 * vH);
         if((2.0 * vH) < 1.0) return (v2);
         if((3.0 * vH) < 2.0) return (v1 + (v2 - v1) * ((2.0 / 3.0) - vH) * 6.0);
         return v1;
      }

      public static void RVB_TSL(int r, int v, int b, ref double T, ref double S, ref double L)
      {
         double var_R = (r / 255.0);                     //RGB from 0 to 255
         double var_V = (v / 255.0);
         double var_B = (b / 255.0);

         double var_Min = Math.Min(Math.Min(var_R, var_V), var_B);    //Min. value of RGB
         double var_Max = Math.Max(Math.Max(var_R, var_V), var_B);    //Max. value of RGB
         double del_Max = var_Max - var_Min;             //Delta RGB value

         L = (var_Max + var_Min) / 2.0;
         T = 0.0;
         S = 0.0;

         if(del_Max != 0.0)                                   //Chromatic data...
         {
            if(L < 0.5)
               S = del_Max / (var_Max + var_Min);
            else
               S = del_Max / (2.0 - var_Max - var_Min);

            double del_R = (((var_Max - var_R) / 6.0) + (del_Max / 2.0)) / del_Max;
            double del_V = (((var_Max - var_V) / 6.0) + (del_Max / 2.0)) / del_Max;
            double del_B = (((var_Max - var_B) / 6.0) + (del_Max / 2.0)) / del_Max;

            if(var_R == var_Max) T = del_B - del_V;
            else if(var_V == var_Max) T = (1.0 / 3.0) + del_R - del_B;
            else if(var_B == var_Max) T = (2.0 / 3.0) + del_V - del_R;

            if(T < 0.0)
               T += 1.0;

            if(T > 1.0)
               T -= 1.0;
         }
      }

      public static int CSI(System.String str)
      {
         int valeur = 0;
         try
         {
            valeur = Convert.ToInt32(str);
         }
         catch(Exception)
         {
         }
         return valeur;
      }

      public static Int64 CSI64(System.String str)
      {
         Int64 valeur = 0;
         try
         {
            valeur = Convert.ToInt64(str);
         }
         catch(Exception)
         {
         }
         return valeur;
      }

      public static double CSD(System.String str)
      {
         double valeur = 0;
         str = str.Replace('.', ',');
         try
         {
            valeur = Convert.ToDouble(str);
         }
         catch(Exception)
         {
         }
         return valeur;
      }

      public static String CT(double valeur)
      {
         return valeur.ToString().Replace(',', '.');
      }

      public static String CT(double valeur, int decimale)
      {
         String txt = "";
         String format = "";
         //format = "";

         if(decimale < 0)
         {
            format = "{0:0.";
            for(int i = 0; i < -decimale; i++)
               format += "0";
            format += "}";
            //format ="{0:0.00}";
            //format.Format( "%%.%df", -decimale );
            if(valeur != (double)((int)valeur))
               txt = String.Format(format, valeur);
            else
               txt = String.Format("{0:0.}", valeur);
         }
         else
         {
            format = "{0:0.";
            for(int i = 0; i < decimale; i++)
               format += "0";
            format += "}";
            //format.Format( "%%.%df", decimale );
            txt = String.Format(format, valeur);
         }
         return txt;
      }

      public static String PremiereLettreMaj(String texte)
      {
         if(texte == null || texte.Length == 0)
            return texte;

         if(texte.Length == 1)
            return texte.ToUpper();

         return texte[0].ToString().ToUpper() + texte.Substring(1).ToLower();
      }

      public static String FormatDate(String strDate)
      {
         if(strDate.Length < 6)
            return "";

         return strDate.Substring(6, 2) + " / " + strDate.Substring(4, 2) + " / " + strDate.Substring(0, 4);
      }

      public static String ChoixLangue(String texte, Langue langue)
      {
         // Choix de la langue
         if(langue == Outils.Langue.Anglais)
            return Regex.Replace(Regex.Replace(texte, @"\{FR=[\s|!-\|~-■]*}", ""), @"\{US=([\s|!-\|~-■]*)}", "$1"); // En anglais

         return Regex.Replace(Regex.Replace(texte, @"\{US=[\s|!-\|~-■]*}", ""), @"\{FR=([\s|!-\|~-■]*)}", "$1"); // En français
      }

      public static int DateToInt(DateTime date)
      {
         if(date == DateTime.MinValue)
            return 0;

         return date.Year * 10000 + date.Month * 100 + date.Day;
      }

      public static DateTime DateToInt(int intdate)
      {
         try
         {
            int annee = intdate / 10000;
            int mois = (intdate - annee * 10000) / 100;
            int jour = intdate - annee * 10000 - mois * 100;

            if(annee < 1 || annee > 9999)
               return DateTime.MinValue;

            if(mois < 1 || mois > 12)
               return DateTime.MinValue;

            if(jour < 1 || jour > 31)
               return DateTime.MinValue;

            return new DateTime(annee, mois, jour);
         }
         catch {}
         
         return DateTime.MinValue;
      }

      public static int Semaine(DateTime dt)
      {
         DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
         System.Globalization.Calendar cal = dfi.Calendar;
         return cal.GetWeekOfYear(dt, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
      }
      
      public static bool IsFlowDocumentVide(FlowDocument document)
      {
         if(document.Blocks.Count == 0) return true;
         TextPointer startPointer = document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
         TextPointer endPointer = document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
         return startPointer.CompareTo(endPointer) == 0;
      }

      public static FlowDocument CloneFlowDocument(FlowDocument document)
      {
         FlowDocument clone = new FlowDocument();
         using(MemoryStream ms = new MemoryStream())
         {
            new TextRange(document.ContentStart, document.ContentEnd).Save(ms, DataFormats.Rtf);
            new TextRange(clone.ContentStart, clone.ContentEnd).Load(ms, DataFormats.Rtf);
         }
         return clone;
      }

      /*public static void AjouteFlowDocument(FlowDocument docFinal, FlowDocument docAajouter)
      {
         using(MemoryStream ms = new MemoryStream())
         {
            new TextRange(docAajouter.ContentStart, docAajouter.ContentEnd).Save(ms, DataFormats.Rtf);
            new TextRange(docFinal.ContentStart, docFinal.ContentEnd).Load(ms, DataFormats.Rtf);
         }



         if(from != null)
         {
            TextRange range = new TextRange(block.ContentStart, block.ContentEnd);

            MemoryStream stream = new MemoryStream();

            System.Windows.Markup.XamlWriter.Save(range, stream);

            range.Save(stream, DataFormats.XamlPackage);

            TextRange textRange2 = new TextRange(to.ContentEnd, to.ContentEnd);

            textRange2.Load(stream, DataFormats.XamlPackage);
         }
      }*/


      public static String XmlChamps(XmlNode xn, params String[] champs)
      {
         int nbChamps = champs.Length;
         if(nbChamps < 1)
            return "";

         return XmlChampsRec(xn, nbChamps, 0, champs);
      }


      public static String XmlChampsRec(XmlNode xn, int nbChamps, int index, params String[] champs)
      {
         if(xn[champs[index]] == null)
            return "";

         if(index == nbChamps - 1)
            return xn[champs[index]].InnerText;

         return XmlChampsRec(xn[champs[index]], nbChamps, index + 1, champs);
      }

      public static String FormatTaille(Int64 taille)
      {
         String[] unite = { "o", "Ko", "Mo", "Go", "To" };
         int u = 0;
         double t = (double)taille;

         while(u < 5 && t > 1024.0)
         {
            t /= 1024.0;
            u++;
         }

         int r = 0;
         if(t < 10.0)
            r = 2;
         else if(t < 100.0)
            r = 1;

         return Math.Round(t, r).ToString() + " " + unite[u];
      }

      const string CharList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

      /// <summary>
      /// Encode the given number into a Base36 string
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public static String EncodeBase36(long input)
      {
         if(input < 0) throw new ArgumentOutOfRangeException("input", input, "input cannot be negative");

         char[] clistarr = CharList.ToCharArray();
         var result = new Stack<char>();
         while(input != 0)
         {
            result.Push(clistarr[input % 36]);
            input /= 36;
         }
         return new string(result.ToArray());
      }

      /// <summary>
      /// Decode the Base36 Encoded string into a number
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public static Int64 DecodeBase36(string input)
      {
         var reversed = input.ToLower().Reverse();
         long result = 0;
         int pos = 0;
         foreach(char c in reversed)
         {
            result += CharList.IndexOf(c) * (long)Math.Pow(36, pos);
            pos++;
         }
         return result;
      }

      public static byte[] SHA256Bytes(byte[] data)
      {
         try
         {
            SHA256Managed hash = new SHA256Managed();
            return hash.ComputeHash(data);
         }
         catch
         {
            return null;//throw;
         }
      }

      public static String SHA256(byte[] data)
      {
         try
         {
            SHA256Managed hash = new SHA256Managed();
            byte[] hashBytes = hash.ComputeHash(data);
            return Convert.ToBase64String(hashBytes);
         }
         catch
         {
            return "";//throw;
         }
      }

      public static String SHA256(String file)
      {
         if(File.Exists(file))
         {
            FileStream st = null;
            try
            {
               SHA256Managed hash = new SHA256Managed();
               st = System.IO.File.Open(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
               byte[] hashBytes = hash.ComputeHash(st);
               return Convert.ToBase64String(hashBytes);

               /*
               System.Security.Cryptography.sh.MD5CryptoServiceProvider check;
               st = System.IO.File.Open(file, System.IO.FileMode.Open, System.IO.FileAccess.Read);
               check = new System.Security.Cryptography.s.SHA256CryptoServiceProvider();
               byte[] somme = check.ComputeHash(st);
               string ret = "";
               foreach(byte a in somme)
               {
                  if(a < 16)
                     ret += "0" + a.ToString("X");
                  else
                     ret += a.ToString("X");
               }
               return ret;*/
            }
            catch
            {
               return "";//throw;
            }
            finally
            {
               if(st != null)
                  st.Close();
            }
         }
         else
         {
            return "";//            throw new System.IO.FileNotFoundException( "Fichier non trouvé.", file );
         }
      }
      /*
      public static void ChargeAttente()
      {
         Thread thread = new Thread(new ThreadStart(ThreadLanceAttente));
         thread.SetApartmentState(ApartmentState.STA);
         thread.IsBackground = true;
         thread.Start();
      }

      private static void ThreadLanceAttente()
      {
         Attente attente = new Attente();
         attente.Show();
         //tempWindow.Show();
         System.Windows.Threading.Dispatcher.Run();
      }
      */

      public static T ChercheParent<T>(DependencyObject enfant) where T : DependencyObject
      {
         //get parent item
         DependencyObject parentObject = VisualTreeHelper.GetParent(enfant);

         //we've reached the end of the tree
         if(parentObject == null) return null;

         //check if the parent matches the type we're looking for
         T parent = parentObject as T;
         if(parent != null)
         {
            return parent;
         }
         else
         {
            return ChercheParent<T>(parentObject);
         }
      }

      public static bool? OuvreFenetre(Window parent, Window enfant)
      {
         //Thread thread = new Thread(new ThreadStart(delegate()
         //   {
         //      parent.Dispatcher.Invoke(DispatcherPriority.Render,
         //        new Action(
         //          delegate()
         //          {
   
         //             //parent.ShowInTaskbar = false;              
         //             DoubleAnimation anim = new DoubleAnimation(0.5, (Duration)TimeSpan.FromSeconds(0.1));
         //             parent.BeginAnimation(UIElement.OpacityProperty, anim);
         //          }
         //      ));
         //   }
         //));

         //thread.Start();
         enfant.Owner = parent;
         //enfant.Loaded += new RoutedEventHandler(enfant_Loaded);
         
         bool? resultat = enfant.ShowDialog();
         //parent.Opacity = 1.0;         
         //parent.ShowInTaskbar = true;

         //DoubleAnimation anim2 = new DoubleAnimation(1.0, (Duration)TimeSpan.FromSeconds(0.1));
         //parent.BeginAnimation(UIElement.OpacityProperty, anim2);

         return resultat;
      }

      public static void Enlight(this TextBox textBox, bool valeur)
      {
         TextBoxEx.SetEnlightened(textBox, valeur);
      }

        public static  BitmapImage BytesToBitmap(byte[] data, int index = 0, int taille = 0)
        {
            // Charge l'image d'origine
            MemoryStream ms = null;
            if (index > 0 || taille > 0)
                ms = new MemoryStream(data, index, taille);
            else
                ms = new MemoryStream(data);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public static object GetIcon(string name)
        {
            if (name.EndsWith(".xaml"))
            {
                var uri = new Uri("/Lims;component/Images/" + name, UriKind.Relative);
                Stream stream = Application.GetResourceStream(uri).Stream;
                FrameworkElement el = XamlReader.Load(stream) as FrameworkElement;

                return el;
            }

            if (name.EndsWith(".png"))
            {
                var bitmap = new BitmapImage(new Uri("/Lims;component/Images/" + name, UriKind.Relative));

                return new Image { Source = bitmap };
            }

            return null;
        }

    }


    /***************************************************************************************************************************************************************************************************************************************
    * CLASSE Http 
    *                                                                                                                                                                           */

    public static class Http
   {
      public enum Methode
      {
         Get = 0,
         Post = 1
      };

      public enum TypeReponse
      {
         Stream = 0,
         Bytes = 1,
         String = 2,
         Xml = 3,
         Fichier = 4
      };

      public static CookieContainer _Cookie = new CookieContainer();
      public static String _Domaine = "";
      //public static Form _Login = null;    // Page de login
      public static bool _Compression = false;   // Compression par défaut

      //public static event EventHandler OnDownload;
      public static event EventHandler<HttpReceptionEventArgs> OnReception;

      public class HttpReceptionEventArgs : EventArgs
      {
         public HttpReceptionEventArgs(Int64 position)
         {
            _Position = position;
         }

         Int64 _Position;

         public Int64 Position
         {
            get { return _Position; }
            set { _Position = value; }
         }
      }

      //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

      // String

      public static String GetString(String adresse, String options)
      { return (String)Envoie(adresse, options, null, Methode.Get, _Compression, TypeReponse.String, null); }

      public static String PostString(String adresse, String options)
      { return (String)Envoie(adresse, options, null, Methode.Post, _Compression, TypeReponse.String, null); }

      public static String GetString(String adresse, String options, bool compression)
      { return (String)Envoie(adresse, options, null, Methode.Get, compression, TypeReponse.String, null); }

      public static String PostString(String adresse, String options, bool compression)
      { return (String)Envoie(adresse, options, null, Methode.Post, compression, TypeReponse.String, null); }

      public static String PostString(String adresse, byte[] bytes)
      { return (String)Envoie(adresse, "", bytes, Methode.Post, _Compression, TypeReponse.String, null); }

      // Xml
      public static XmlDocument GetXml(String adresse, String options)
      { return (XmlDocument)Envoie(adresse, options, null, Methode.Get, _Compression, TypeReponse.Xml, null); }

      public static XmlDocument PostXml(String adresse, String options)
      { return (XmlDocument)Envoie(adresse, options, null, Methode.Post, _Compression, TypeReponse.Xml, null); }

      public static XmlDocument GetXml(String adresse, String options, bool compression)
      { return (XmlDocument)Envoie(adresse, options, null, Methode.Get, compression, TypeReponse.Xml, null); }

      public static XmlDocument PostXml(String adresse, String options, bool compression)
      { return (XmlDocument)Envoie(adresse, options, null, Methode.Post, compression, TypeReponse.Xml, null); }

      public static XmlDocument PostXml(String adresse, byte[] bytes)
      { return (XmlDocument)Envoie(adresse, "", bytes, Methode.Post, _Compression, TypeReponse.Xml, null); }

      // Fichier

      public static bool GetFichier(String adresse, String options, String fichier)
      { return (bool)Envoie(adresse, options, null, Methode.Get, false, TypeReponse.Fichier, fichier); }

      public static String PostFichierString(String adresse, byte[] bytes, String fichier)
      { return (String)Envoie(adresse, "", bytes, Methode.Post, _Compression, TypeReponse.String, fichier); }

      //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

      //public static Object Envoie(String adresse, String options, Methode methode, bool compression, TypeReponse typeReponse, String fichier)
      public static Object Envoie(String adresse, String options, byte[] bytes, Methode methode, bool compression, TypeReponse typeReponse, String fichier)
      {
         try
         {
            // Prépare la requête -----------------------------------------------------------------

            HttpWebRequest requete;
            if(methode == Methode.Post || options == "")
               requete = (HttpWebRequest)WebRequest.Create(_Domaine + adresse);
            else
               requete = (HttpWebRequest)WebRequest.Create(_Domaine + adresse + "?" + options);

            requete.ContentType = "application/x-www-form-urlencoded"; //multipart/form-data"; //; // 
            requete.CookieContainer = _Cookie;

            // _Cookie.GetCookies

            if(methode == Methode.Post)
            {
               requete.Method = "POST";
               if(bytes == null)
               {
                  // Contenu du POST
                  UTF8Encoding encoding = new UTF8Encoding();
                  byte[] data = encoding.GetBytes(options);

                  // Envoie le post
                  requete.ContentLength = data.Length;
                  Stream streamPost = requete.GetRequestStream();
                  streamPost.Write(data, 0, data.Length);
                  streamPost.Close();
               }
               else
               {
                  // Envoie le post
                  requete.ContentLength = bytes.Length;
                  Stream streamPost = requete.GetRequestStream();
                  streamPost.Write(bytes, 0, bytes.Length);
                  streamPost.Close();
               }
            }
            else
               requete.Method = "GET";


            // Réponse ----------------------------------------------------------------------------------

            HttpWebResponse response = (HttpWebResponse)requete.GetResponse();
            Stream streamRecue = response.GetResponseStream();
            Stream stream;
            /*
            Console.WriteLine("Count : " + _Cookie.Count.ToString());            
            Console.WriteLine("PerDomainCapacity : " + _Cookie.PerDomainCapacity.ToString());
            
            foreach(Cookie cookieValue in _Cookie.GetCookies( new Uri("https://cfe-gui.adjutorium.local/alpha/")))
            {
               Console.WriteLine("Cookie 1 : " + cookieValue.ToString());
            }

            foreach(Cookie cookieValue in response.Cookies)
            {
               Console.WriteLine("Cookie: " + cookieValue.ToString());
            }
            */
            // Décompresse
            if(compression)
            {
               GZipStream decompresse = new GZipStream(streamRecue, CompressionMode.Decompress, true);
               stream = decompresse;
               /*  // Transforme en String
                 StreamReader streamReponse = new StreamReader(decompresse, Encoding.UTF8);
                 retour = streamReponse.ReadToEnd();
                 streamReponse.Close();*/
            }

            // Sans décompression
            else
            {
               stream = streamRecue;
               /*  // Transforme en String
                 StreamReader streamReponse = new StreamReader(streamRecue, Encoding.UTF8);
                 retour = streamReponse.ReadToEnd();
                 streamReponse.Close();*/
            }


            // Interpretation --------------------------------------------------------------------------

            // Stream
            if(typeReponse == TypeReponse.Stream)
            {
               byte[] buffer = new byte[65535];
               using(MemoryStream ms = new MemoryStream())
               {
                  while(true)
                  {
                     int read = stream.Read(buffer, 0, buffer.Length);
                     if(read <= 0)
                        break;
                     ms.Write(buffer, 0, read);
                  }
                  response.Close();
                  return ms;
               }
            }

            // Byte[]
            else if(typeReponse == TypeReponse.Bytes)
            {
               byte[] buffer = new byte[65535];
               using(MemoryStream ms = new MemoryStream())
               {
                  while(true)
                  {
                     int read = stream.Read(buffer, 0, buffer.Length);
                     if(read <= 0)
                        break;
                     ms.Write(buffer, 0, read);
                  }
                  response.Close();
                  return ms.ToArray();
               }
            }

            // String
            else if(typeReponse == TypeReponse.String)
            {
               StreamReader streamReponse = new StreamReader(stream, Encoding.UTF8);
               String retour = streamReponse.ReadToEnd();
               streamReponse.Close();
               response.Close();
               return retour;
            }

            // Xml
            else if(typeReponse == TypeReponse.Xml)
            {
               StreamReader streamReponse = new StreamReader(stream, Encoding.UTF8);
               String retour = streamReponse.ReadToEnd();
               streamReponse.Close();
               response.Close();
               XmlDocument xmlDoc = new XmlDocument();
               xmlDoc.LoadXml(retour);
               return xmlDoc;
            }

            // Fichier
            else if(typeReponse == TypeReponse.Fichier)
            {
               // Ecrit le fichier
               FileStream fs = new FileStream(fichier, FileMode.Create);
               byte[] read = new byte[65536];
               int count = stream.Read(read, 0, read.Length);
               Int64 position = 0;
               while(count > 0)
               {
                  fs.Write(read, 0, count);
                  count = stream.Read(read, 0, read.Length);
                  position += (Int64)count;

                  if(OnReception != null)
                     OnReception(null, new HttpReceptionEventArgs(position));

                  /*L_Telechargement.Text = String.Format("Téléchargement " + (100 * position / _TailleFichier).ToString() + " %");
                  PB_Telechargement.Value = (int)(1000 * position / _TailleFichier);
                  L_Telechargement.Refresh();
                  PB_Telechargement.Refresh();*/
               }

               /*L_Telechargement.Text = String.Format("Téléchargement terminé");
               PB_Telechargement.Value = 1000;
               Refresh();*/

               // Ferme tout
               fs.Close();
               stream.Close();
               response.Close();

               return true;
            }



            //return true;
         }
         catch(WebException we)
         {
            Console.WriteLine("This program is expected to throw WebException on successful run.\n\nException Message :" + we.Message);
            if(we.Status == WebExceptionStatus.ProtocolError)
            {
               Console.WriteLine("Status Code : {0}", ((HttpWebResponse)we.Response).StatusCode);
               Console.WriteLine("Status Description : {0}", ((HttpWebResponse)we.Response).StatusDescription);
            }
            //return false;
         }
         catch(Exception ex)
         {
            Console.WriteLine(ex.Message);
            //return false;
         }

         return null;
      }

   }


   /***************************************************************************************************************************************************************************************************************************************
    * CLASSE Chrono 
    * Mesure et affiche le temps écoulé entre deux points du code                                                                                                                                                                          */

   public class Chrono
   {
       System.Diagnostics.Stopwatch m_StopWatch;

      //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      public Chrono()
      {
         m_StopWatch = new System.Diagnostics.Stopwatch();
         m_StopWatch.Start();
      }

      //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      public void Start()
      {
         m_StopWatch.Reset();
         m_StopWatch.Start();
      }

      //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      public void StopStart()
      {
         m_StopWatch.Stop();
         Console.WriteLine(m_StopWatch.Elapsed.ToString());
         m_StopWatch.Reset();
         m_StopWatch.Start();
      }

      //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      public void Stop()
      {
         m_StopWatch.Stop();
         Console.WriteLine(m_StopWatch.Elapsed.ToString());
      }

      public void StopMb()
      {
         m_StopWatch.Stop();
         MessageBox.Show(m_StopWatch.Elapsed.ToString());
      }

      ~Chrono()
      {
      }
   }

   [ValueConversion(typeof(Brush), typeof(Brush))]
   public class BrushFondConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         /* if(value == null)
             return Brushes.Red;

          Color couleur = ((SolidColorBrush)value).Color;*/

         Color couleur = O.ExtraireCouleur(value);

         double t = 0.0;
         double s = 0.0;
         double l = 0.0;
         O.RVB_TSL(couleur.R, couleur.G, couleur.B, ref t, ref s, ref l);

         //return new LinearGradientBrush(couleur, Luminance2(couleur, 100), new Point(0, 0), new Point(1, 1));
         //return new LinearGradientBrush(O.TSL_RVB(t, s, 0.6), O.TSL_RVB(t, s, 0.9), new Point(0, 0), new Point(1, 1));

         //LinearGradientBrush brush = new LinearGradientBrush(O.TSL_RVB(t, s, 0.74), O.TSL_RVB(t, s, 0.76), new Point(0, 0), new Point(5, 5));
         //brush.MappingMode         = BrushMappingMode.Absolute;
         //brush.SpreadMethod        = GradientSpreadMethod.Reflect;

         GradientStopCollection gs = new GradientStopCollection();
         gs.Add(new GradientStop(O.TSL_RVB(t, s, 0.755), 0.3333));
         gs.Add(new GradientStop(O.TSL_RVB(t, s, 0.745), 0.6666));
         //gs.Add(new GradientStop(O.TSL_RVB(t, s, 0.74), 0.75));

         LinearGradientBrush brush = new LinearGradientBrush(gs, new Point(0, 0), new Point(3, 3));
         brush.MappingMode         = BrushMappingMode.Absolute;
         brush.SpreadMethod        = GradientSpreadMethod.Reflect;

         return brush;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   [ValueConversion(typeof(Brush), typeof(Brush))]
   public class CouleurLabelFondConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         /*if(value == null)
            return Brushes.Red;

         Color couleur;

         if(value is SolidColorBrush)
            couleur = ((SolidColorBrush)value).Color;
         else if(value is LinearGradientBrush)
         {
            if(((LinearGradientBrush)value).GradientStops.Count < 1)
               return Brushes.Red;
            else
               couleur = ((LinearGradientBrush)value).GradientStops[0].Color;
         }
         else
            return Brushes.Red;
         */
         Color couleur = O.ExtraireCouleur(value);
         double t = 0.0;
         double s = 0.0;
         double l = 0.0;
         O.RVB_TSL(couleur.R, couleur.G, couleur.B, ref t, ref s, ref l);

         return new SolidColorBrush(O.TSL_RVB(t, s, 0.3));
      }



      /*
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value == null)
            return Brushes.Red;

         Color couleur = ((SolidColorBrush)value).Color;

         double t = 0.0;
         double s = 0.0;
         double l = 0.0;
         O.RVB_TSL(couleur.R, couleur.G, couleur.B, ref t, ref s, ref l);

         return new SolidColorBrush(O.TSL_RVB(t, s, 0.3));
      }
      */

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   [ValueConversion(typeof(Brush), typeof(Brush))]
   public class SelectionBrushConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         /* if(value == null)
             return Brushes.Red;

          Color couleur = ((SolidColorBrush)value).Color;*/

         Color couleur = O.ExtraireCouleur(value);

         double t = 0.0;
         double s = 0.0;
         double l = 0.0;
         O.RVB_TSL(couleur.R, couleur.G, couleur.B, ref t, ref s, ref l);

         //return new LinearGradientBrush(couleur, Luminance2(couleur, 100), new Point(0, 0), new Point(1, 1));
         return new LinearGradientBrush(O.TSL_RVB(t, s, 0.4), O.TSL_RVB(t, s, 0.2), new Point(0, 0), new Point(0, 1));
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   public class TitresListViewFondConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         /*if(value == null)
            return Brushes.Red;

         Color couleur = ((SolidColorBrush)value).Color;
         */
         Color couleur = O.ExtraireCouleur(value);

         double t = 0.0;
         double s = 0.0;
         double l = 0.0;
         O.RVB_TSL(couleur.R, couleur.G, couleur.B, ref t, ref s, ref l);

         //return new LinearGradientBrush(couleur, Luminance2(couleur, 100), new Point(0, 0), new Point(1, 1));
         return new LinearGradientBrush(O.TSL_RVB(t, s, 0.4), O.TSL_RVB(t, s, 0.2), new Point(0, 0), new Point(0, 1));
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   //[ValueConversion(typeof(String), typeof(Canvas))]
   //public class IconeCanvasConverter : IValueConverter
   //{
   //   public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
   //   {
   //      if(value == null)
   //         return new Canvas();

   //      return (Canvas)Application.LoadComponent(new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ";component/Icones/" + (String)value + ".xaml", UriKind.Relative));
   //   }

   //   public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
   //   {
   //      return null;
   //   }
   //}

   [ValueConversion(typeof(string), typeof(bool))]
   public class StringVideConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value == null || (value as string).Length <= 0)
            return true;

         return false;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   //[ValueConversion(typeof(FlowDocument), typeof(bool))]
   //public class FlowDocumentVideConverter : IValueConverter
   //{
   //   public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
   //   {
   //      return O.IsFlowDocumentVide((FlowDocument)value);
   //   }

   //   public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
   //   {
   //      return null;
   //   }
   //}


   [ValueConversion(typeof(string), typeof(bool))]
   public class StringTitreConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value == null || (value as string).Length <= 0)
            return "???";


         string titre = "";
         char[] caracteres = (value as string).ToUpper().ToCharArray();

         if(caracteres[caracteres.GetLength(0) - 1] == ' ')
            return "";
         foreach(char c in caracteres)
            titre += c + "   ";

         return titre.Trim();
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   [ValueConversion(typeof(string), typeof(bool))]
   public class StringElargieConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value == null || (value as string).Length <= 0)
            return value;

         return " " + (value as string) + " ";
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   [ValueConversion(typeof(string), typeof(string))]
   public class AffichageDateConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value != null)
         {
            string txt = value.ToString();

            if(txt.Length < 8 || txt == "0")
               return "";

            return txt.Substring(6, 2) + " / " + txt.Substring(4, 2) + " / " + txt.Substring(0, 4);
         }
         else
            return "";
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }


   [ValueConversion(typeof(int), typeof(DateTime))]
   public class ConvertionIntToDateTime : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value != null)
         {
            if(value is int)
            {
               try
               {
                  int i = (int)value;
                  int annee = i / 10000;
                  int mois = (i - annee * 10000) / 100;
                  int jour = i - annee * 10000 - mois * 100;

                  if(annee < 1 || annee > 9999)
                     return DateTime.MinValue;

                  if(mois < 1 || mois > 12)
                     return DateTime.MinValue;

                  if(jour < 1 || jour > 31)
                     return DateTime.MinValue;

                  return new DateTime(annee, mois, jour);
               }
               catch
               {
                  return DateTime.MinValue;
               }
            }
            else if(value is DateTime)
               return value;
         }

         return DateTime.MinValue;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value != null)
         {
            if(value is DateTime)
            {
               DateTime dt = (DateTime)value;
               if(dt == DateTime.MinValue)
                  return 0;
               else
                  return dt.Year * 10000 + dt.Month * 100 + dt.Day;
            }
            else if(value is int)
               return value;
         }

         return 0;
      }
   }

   [ValueConversion(typeof(object), typeof(string))]
   public class AffichageDoubleNonNulConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value != null)
         {
            if(System.Convert.ToDouble(value) == 0.0)
               return "";
            else
               return value.ToString();
         }
         else
            return "";
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   [ValueConversion(typeof(int), typeof(String))]
   public class NbEditionTextConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value is DBNull || !(value is int) || (int)value == 0)
            return "";

         return (Math.Abs((int)value) >> 14).ToString();
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   [ValueConversion(typeof(int), typeof(Brush))]
   public class NbEditionBrushConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value is DBNull || !(value is int) || (int)value == 0)
            return Brushes.Transparent;
         //return new LinearGradientBrush(Color.FromRgb(180, 180, 180), Color.FromRgb(100, 100, 100), 90.0);
         // Archive
         if((int)value < 0)
            return new LinearGradientBrush(Color.FromRgb(180, 180, 180), Color.FromRgb(100, 100, 100), 90.0);

         // Décode la date butoire
         int date = (int)value - ((Math.Abs((int)value)) >> 14)*16384;

         // C'est bon pour la date butoire
         DateTime dt = DateTime.Now;
         if(date == 0 || date > (dt.Day-1) + (dt.Month-1)*31 + (dt.Year-2000)*31*12)
            return new LinearGradientBrush(Color.FromRgb(180, 255, 180), Color.FromRgb(100, 255, 100), 90.0);

         // La date butoire est passée
         return new LinearGradientBrush(Color.FromRgb(255, 180, 180), Color.FromRgb(255, 100, 100), 90.0);

         //int annee = date / (31*12);
         //int mois = (date - annee*12*31)/31;
         //int jour = date - (annee*12 + mois)*31 + 1;
         //mois++;
         //annee += 2000;

         //Console.WriteLine(new DateTime(annee, mois, jour));

         //// Date butoire passée ou non ?
         //if(new DateTime(annee, mois, jour) > DateTime.Now )
         //   if( DateTime
         //   return new LinearGradientBrush(Color.FromRgb(255, 180, 180), Color.FromRgb(255, 100, 100), 90.0);
         //else
         //   return new LinearGradientBrush(Color.FromRgb(180, 255, 180), Color.FromRgb(100, 255, 100), 90.0);
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }

   [ValueConversion(typeof(int), typeof(Visibility))]
   public class IntToVisibleConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         Console.WriteLine(value);
         if(!(value is int) || (int)value == 0)
            return System.Windows.Visibility.Collapsed;

         if((int)value == 1)
            return System.Windows.Visibility.Hidden;
         else
            return System.Windows.Visibility.Visible;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return 1;
      }
   }

   //[ValueConversion(typeof(bool), typeof(bool))]
   //public class NegatingConverter : IValueConverter
   //{
   //   public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
   //   {
   //      return !((bool)value);
   //   }

   //   public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
   //   {
   //      return null;
   //   }
   //}
   [ValueConversion(typeof(String), typeof(BitmapImage))]
   public class PngConverter2 : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value == null)
            return null;//return new BitmapImage(new Uri("/Lims;component/Images/Annuler.png", UriKind.Relative));//


         if(value is byte[])
         //{
            //int taille = BitConverter.ToInt32((byte[])value, 0);
            return O.BytesToBitmap((byte[])value, 0, ((byte[])value).Length);
        // }

         //return new BitmapImage(new Uri("/Images/"+ (String)value +".png", UriKind.Relative));

         //return new BitmapImage(new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ";component/Images/"+ (String)value +".png", UriKind.Relative));
         //return new BitmapImage(new Uri("/Images/"+ (String)value +".png", UriKind.Relative));


         //return new BitmapImage(new Uri("/Lims;component/Images/Analyse.png", UriKind.Relative));
         //(String)

         return new BitmapImage(new Uri("/Lims;component/Images/"+ value.ToString() +".png", UriKind.Relative));
         //return new BitmapImage(new Uri("pack://application:,,,/Images/"+ value.ToString() +".png", UriKind.Absolute));



         //System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ";component/Images/"
         //return new BitmapImage(new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ";component/Images/" + value.ToString() +".png", UriKind.Relative));
         //return new BitmapImage(new Uri("/Images/"+ value.ToString() +".png", UriKind.Relative));
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }




    [ValueConversion(typeof(String), typeof(FrameworkElement))]
   public class PngConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if(value == null)
            return null;


         if(value is byte[] bytes)
            return O.BytesToBitmap(bytes, 0, bytes.Length);

         var name = value.ToString();
         if (!name.Contains(".")) name += ".png";

         return O.GetIcon(name);
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return null;
      }
   }
}