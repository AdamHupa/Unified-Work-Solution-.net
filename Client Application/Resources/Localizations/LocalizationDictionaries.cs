using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace Client_Application.Resources.Localizations
{
    public static class LocalizationDictionaries
    {
        public static Dictionary<string, Uri> ApplicationEmbedded = new Dictionary<string, Uri>()
        {
            { "",      new Uri("/Client Application;component/Resources/Localizations/Localization.xaml",       UriKind.RelativeOrAbsolute) },
            { "en-GB", new Uri("/Client Application;component/Resources/Localizations/Localization.en-GB.xaml", UriKind.RelativeOrAbsolute) },
            { "pl-PL", new Uri("/Client Application;component/Resources/Localizations/Localization.pl-PL.xaml", UriKind.RelativeOrAbsolute) },

            // add here
        };


        public static readonly Dictionary<string, Uri> Imported = new Dictionary<string, Uri>();


        public static string GetApplicationsPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static bool SwitchLocalization(string inFiveCharLocalization)
        {
            Uri newUri;
            string key = String.IsNullOrWhiteSpace(inFiveCharLocalization) ? "" : inFiveCharLocalization;

            if (LocalizationDictionaries.Imported.TryGetValue(key, out newUri) == false)
                if (LocalizationDictionaries.ApplicationEmbedded.TryGetValue(key, out newUri) == false)
                   return false;

            // remove previous localization
            IEnumerable<ResourceDictionary> applied = Application.Current.Resources.MergedDictionaries
                .Where(resourceDictionary => LocalizationDictionaries.Imported.ContainsValue(resourceDictionary.Source) ||
                                             LocalizationDictionaries.ApplicationEmbedded.ContainsValue(resourceDictionary.Source));

            foreach (ResourceDictionary resourceDictionary in applied.ToList())
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);

            // set new localization
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = newUri });
            return true;
        }
        
    }
}
