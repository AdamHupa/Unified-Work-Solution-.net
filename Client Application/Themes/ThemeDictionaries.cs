using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace Client_Application.Themes
{
    public static class ResourceKeys
    {

    }


    public static class ResourceIds
    {

    }


    public static /*partial*/ class ThemeDictionaries
    {
        public static Dictionary<string, Uri> ApplicationEmbedded = new Dictionary<string, Uri>()
        {
            // referenced assembly name may contain spaces

            { "Default", new Uri("/Client Application;component/Themes/ThemeDictionary.xaml", UriKind.RelativeOrAbsolute) },
            { "Generic", new Uri("/Client Application;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute) },

            // add here
        };

        public static readonly ResourceDictionary Default = new ResourceDictionary
        {
            Source = new Uri("/Client Application;component/Themes/ThemeDictionary.xaml", UriKind.RelativeOrAbsolute)
        };

        //public static readonly Dictionary<string, Uri> Imported = new Dictionary<string, Uri>();


        public static bool SwitchTheme(string themeKey)
        {
            Uri newUri;
            string key = String.IsNullOrWhiteSpace(themeKey) ? "" : themeKey;

            //if (ThemeDictionaries.Imported.TryGetValue(key, out newUri) == false)
                if (ThemeDictionaries.ApplicationEmbedded.TryGetValue(key, out newUri) == false)
                    return false;

            // remove previous theme
            IEnumerable<ResourceDictionary> applied = Application.Current.Resources.MergedDictionaries
                .Where(resourceDictionary => //ThemeDictionaries.Imported.ContainsValue(resourceDictionary.Source) ||
                                             ThemeDictionaries.ApplicationEmbedded.ContainsValue(resourceDictionary.Source));

            foreach (ResourceDictionary resourceDictionary in applied.ToList())
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);

            // set new theme
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = newUri });
            return true;
        }
    }
}
