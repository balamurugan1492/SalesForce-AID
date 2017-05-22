using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NHunspell;
using System.IO;
using System.Globalization;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;

namespace Pointel.Windows.Views.Common.Editor.Helper
{
    /// <summary>
    /// This class used to manage custom spell check using HUNSPELL
    /// </summary>
    public class SpellChecker : INotifyPropertyChanged
    {
        #region Declarations
        private Hunspell hunSpell;
        private string text;
        private List<string> words;
        private ObservableCollection<string> misspelledWords = new ObservableCollection<string>();
        private ObservableCollection<string> suggestedWords = new ObservableCollection<string>();
        private List<string> ignoredWords;
        private string selectedMisspelledWord;
        private string selectedSuggestedWord;
        private bool isReplaceEnabled;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Declarations

        #region Properties
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                SpellCheck(value);
                OnPropertyChanged("Text");
            }
        }
        public List<string> Words
        {
            get { return words; }
            set
            {
                words = value;
                OnPropertyChanged("Words");
            }
        }
        public ObservableCollection<string> MisspelledWords
        {
            get { return misspelledWords; }
            set
            {
                misspelledWords = value;
                OnPropertyChanged("MisspelledWords");
            }
        }
        public ObservableCollection<string> SuggestedWords
        {
            get { return suggestedWords; }
            set
            {
                suggestedWords = value;
                OnPropertyChanged("SuggestedWords");
            }
        }
        public List<string> IgnoredWords
        {
            get { return ignoredWords; }
            set
            {
                ignoredWords = value;
                OnPropertyChanged("IgnoredWords");
            }
        }
        public string SelectedMisspelledWord
        {
            get { return selectedMisspelledWord; }
            set
            {
                selectedMisspelledWord = value;
                LoadSuggestions(value);
                OnPropertyChanged("SelectedMisspelledWord");
                OnPropertyChanged("IsReplaceEnabled");
            }
        }
        public string SelectedSuggestedWord
        {
            get { return selectedSuggestedWord; }
            set
            {
                selectedSuggestedWord = value;
                OnPropertyChanged("SelectedSuggestedWord");
                OnPropertyChanged("IsReplaceEnabled");
            }
        }
        public bool IsReplaceEnabled
        {
            get { return isReplaceEnabled = (!string.IsNullOrEmpty(SelectedMisspelledWord) && !string.IsNullOrEmpty(SelectedSuggestedWord)); }
            set
            {
                isReplaceEnabled = value;
                OnPropertyChanged("IsReplaceEnabled");
            }
        }
        #endregion Properties

        #region Constructor
        public SpellChecker()
        {
            _logger.Info("SpellChecker() Constructor Started..");
            try
            {
                Words = new List<string>();
                MisspelledWords = new ObservableCollection<string>();
                IgnoredWords = new List<string>();
            }
            catch (Exception generalException)
            {
                _logger.Error("Spell Checker Open problem : " + generalException.ToString());
            }
            _logger.Info("SpellChecker() Constructor Ended..");
        }
        #endregion Constructor

        #region Destructor
        ~SpellChecker()
        {
            hunSpell = null;
            text = null;
            if (words != null)
                words.Clear();
            words = null;
            if (misspelledWords != null)
                misspelledWords.Clear();
            misspelledWords = null;
            if (suggestedWords != null)
                suggestedWords.Clear();
            suggestedWords = null;
            if (ignoredWords != null)
                ignoredWords.Clear();
            ignoredWords = null;
            selectedMisspelledWord = null;
            selectedSuggestedWord = null;
        }
        #endregion Destructor

        #region OnPropertyChanged Event
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            try
            {
                var handler = this.PropertyChanged;
                if (handler != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in OnPropertyChanged() " + generalException.ToString());
            }
        }
        #endregion OnPropertyChanged Event

        private bool checkignorewords(string mispellWord)
        {
            for (int i = 0; i < IgnoredWords.Count; i++)
            {
                if (mispellWord.Trim().ToLower() == IgnoredWords[i].Trim().ToLower())
                    return false;
            }
            return true;
        }

        #region Callable Methods
        /// <summary>
        /// Spells the check.
        /// </summary>
        /// <param name="content">The content.</param>
        public void SpellCheck(string content)
        {
            try
            {
                ClearLists();
                var tempWords = content.Split(' ');
                foreach (var tempWord in tempWords)
                {
                    Words.Add(tempWord.Trim());
                }

                foreach (var word in Words)
                {
                    if (!hunSpell.Spell(word) && checkignorewords(word))
                        MisspelledWords.Add(word);
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in SpellCheck() " + generalException.ToString());
            }
        }

        /// <summary>
        /// Sets the dictionary.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <returns></returns>
        public bool SetDictionary(AbstractDictionary dict)
        {
            HunspellDictionary hunspellDictionary = dict as HunspellDictionary;
            if (hunspellDictionary != null)
            {
                if (string.IsNullOrEmpty(hunspellDictionary.ZIPFilePath))
                {
                    if (hunSpell != null)
                        hunSpell = null;
                    hunSpell = new Hunspell(hunspellDictionary.AffixFile, hunspellDictionary.DictionaryFile);
                    return true;
                }
                try
                {
                    using (ZipFile zipFile = new ZipFile(hunspellDictionary.ZIPFilePath))
                    {
                        ZipEntry entry = zipFile.GetEntry(hunspellDictionary.AffixFile);
                        Stream inputStream = zipFile.GetInputStream(entry);
                        byte[] array = new byte[entry.Size];
                        int num = inputStream.Read(array, 0, (int)entry.Size);
                        long arg_7E_0 = (long)num;
                        long arg_7D_0 = entry.Size;
                        inputStream.Close();
                        ZipEntry entry2 = zipFile.GetEntry(hunspellDictionary.DictionaryFile);
                        Stream inputStream2 = zipFile.GetInputStream(entry2);
                        byte[] array2 = new byte[entry2.Size];
                        num = inputStream2.Read(array2, 0, (int)entry2.Size);
                        long arg_CB_0 = (long)num;
                        long arg_CA_0 = entry2.Size;
                        inputStream2.Close();
                        if (hunSpell != null)
                            hunSpell = null;
                        this.hunSpell = new Hunspell(array, array2);
                        return true;
                    }
                }
                catch (Exception generalException)
                {
                    _logger.Error("Error occurred in SetDictionary() " + generalException.ToString());
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Finds the dictionaries.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public AbstractDictionary[] FindDictionaries(string path)
        {
            List<AbstractDictionary> list = new List<AbstractDictionary>();
            List<string> list2 = new List<string>();
            try
            {
                list2.AddRange(Directory.GetFiles(path, "*.oxt", SearchOption.TopDirectoryOnly));
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in FindDictionaries() " + generalException.ToString());
            }
            try
            {
                list2.AddRange(Directory.GetFiles(path, "*.zip", SearchOption.TopDirectoryOnly));
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in FindDictionaries() " + generalException.ToString());
            }
            foreach (string current in list2)
            {
                try
                {
                    using (ZipFile zipFile = new ZipFile(current))
                    {
                        ZipEntry entry = zipFile.GetEntry("dictionaries.xcu");
                        if (entry != null)
                        {
                            Stream inputStream = zipFile.GetInputStream(entry);
                            new StreamReader(inputStream);
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load(inputStream);
                            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                            xmlNamespaceManager.AddNamespace("oor", "http://openoffice.org/2001/registry");
                            XmlElement documentElement = xmlDocument.DocumentElement;
                            XmlNodeList xmlNodeList = documentElement.SelectNodes("/oor:component-data/node[@oor:name='ServiceManager']/node[@oor:name='Dictionaries']/node", xmlNamespaceManager);
                            foreach (XmlNode xmlNode in xmlNodeList)
                            {
                                XmlNode xmlNode2 = xmlNode.SelectSingleNode("@oor:name", xmlNamespaceManager);
                                string name = (xmlNode2 != null) ? xmlNode2.Value : null;
                                XmlNode xmlNode3 = xmlNode.SelectSingleNode("prop[@oor:name='Locations']/value", xmlNamespaceManager);
                                string text = (xmlNode3 != null) ? xmlNode3.InnerText : null;
                                XmlNode xmlNode4 = xmlNode.SelectSingleNode("prop[@oor:name='Format']/value", xmlNamespaceManager);
                                string text2 = (xmlNode4 != null) ? xmlNode4.InnerText : null;
                                XmlNode xmlNode5 = xmlNode.SelectSingleNode("prop[@oor:name='Locales']/value", xmlNamespaceManager);
                                string text3 = (xmlNode5 != null) ? xmlNode5.InnerText : null;
                                if (string.Compare(text2, "DICT_SPELL", StringComparison.InvariantCultureIgnoreCase) == 0 && text != null && text != null)
                                {
                                    string[] array = text.Replace("%origin%/", "").Split(new char[]
									{
										' '
									}, StringSplitOptions.RemoveEmptyEntries);
                                    list.Add(new HunspellDictionary
                                    {
                                        Name = name,
                                        ZIPFilePath = current,
                                        AffixFile = array[0],
                                        DictionaryFile = array[1],
                                        Format = text2,
                                        Locales = (text3 != null) ? text3.Split(new char[]
										{
											' '
										}, StringSplitOptions.RemoveEmptyEntries) : null
                                    });
                                }
                            }
                            inputStream.Close();
                        }
                        else
                        {
                            foreach (ZipEntry zipEntry in zipFile)
                            {
                                string extension = Path.GetExtension(zipEntry.Name);
                                if (extension.ToLower() == ".dic")
                                {
                                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(zipEntry.Name);
                                    int num = zipFile.FindEntry(fileNameWithoutExtension + ".aff", true);
                                    if (num >= 0)
                                    {
                                        ZipEntry zipEntry2 = zipFile[num];
                                        if (zipEntry2 != null)
                                        {
                                            list.Add(new HunspellDictionary
                                            {
                                                Name = fileNameWithoutExtension,
                                                ZIPFilePath = current,
                                                AffixFile = fileNameWithoutExtension + ".aff",
                                                DictionaryFile = fileNameWithoutExtension + ".dic",
                                                Format = "DICT_SPELL",
                                                Locales = new string[]
												{
													fileNameWithoutExtension
												}
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception generalException)
                {
                    _logger.Error("Error occurred in FindDictionaries() " + generalException.ToString());
                }
            }
            List<string> list3 = new List<string>();
            try
            {
                list3.AddRange(Directory.GetFiles(path, "*.dic", SearchOption.TopDirectoryOnly));
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred in FindDictionaries() " + generalException.ToString());
            }
            foreach (string current2 in list3)
            {
                try
                {
                    string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(current2);
                    string directoryName = Path.GetDirectoryName(current2);
                    string text4 = Path.Combine(directoryName, fileNameWithoutExtension2 + ".aff");
                    list.Add(new HunspellDictionary
                    {
                        Name = fileNameWithoutExtension2,
                        ZIPFilePath = null,
                        AffixFile = File.Exists(text4) ? text4 : null,
                        DictionaryFile = current2,
                        Format = "DICT_SPELL",
                        Locales = new string[]
						{
							fileNameWithoutExtension2
						}
                    });
                }
                catch (Exception generalException)
                {
                    _logger.Error("Error occurred in FindDictionaries() " + generalException.ToString());
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Loads the suggestions.
        /// </summary>
        /// <param name="misspelledWord">The misspelled word.</param>
        public void LoadSuggestions(string misspelledWord)
        {
            try
            {
                SuggestedWords = new ObservableCollection<string>(hunSpell.Suggest(misspelledWord));
                if (SuggestedWords.Count == 0) SuggestedWords = new ObservableCollection<string> { "(no suggestions)" };
            }
            catch (Exception generalException)
            {
                _logger.Info("Error occurred in LoadSuggestions() " + generalException.ToString());
            }
        }

        /// <summary>
        /// Adds the to dictionary.
        /// </summary>
        /// <param name="word">The word.</param>
        public void AddTodictionary(string word)
        {
            try
            {
                if (hunSpell != null)
                    hunSpell.Add(word);
            }
            catch (Exception generalException)
            {
                _logger.Info("Error occurred in AddTodictionary()" + generalException.ToString());
            }
        }

        /// <summary>
        /// Clears the lists.
        /// </summary>
        public void ClearLists()
        {
            try
            {
                Words.Clear();
                MisspelledWords.Clear();
            }
            catch (Exception generalException)
            {
                _logger.Info("Error occurred in ClearLists() " + generalException.ToString());
            }
        }

        #endregion Callable Methods
    }

    /// <summary>
    /// This class used to manage the HunspellDictionary Properties
    /// </summary>
    public class HunspellDictionary : AbstractDictionary
    {
        public string ZIPFilePath
        {
            get;
            set;
        }
        public string AffixFile
        {
            get;
            set;
        }
        public string DictionaryFile
        {
            get;
            set;
        }
        public string Format
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This class used to manage the AbstractDictionary Properties
    /// </summary>
    public class AbstractDictionary
    {
        public string Name
        {
            get;
            set;
        }
        public string[] Locales
        {
            get;
            set;
        }
        public Func<string> ToStringFormat
        {
            get;
            set;
        }
        public override string ToString()
        {
            if (this.ToStringFormat != null)
            {
                return this.ToStringFormat();
            }
            if (this.Locales != null && this.Locales.Length > 0)
            {
                try
                {
                    CultureInfo cultureInfo = new CultureInfo(this.Locales[0], false);
                    string text = cultureInfo.NativeName;
                    if (text.Length > 0)
                    {
                        string text2 = text[0].ToString();
                        text2 = text2.ToUpperInvariant();
                        text = text2 + text.Substring(1);
                    }
                    return text;
                }
                catch
                {
                }
            }
            return this.Name;
        }
    }
}
