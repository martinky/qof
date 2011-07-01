using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;

namespace QuickOpenFile
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog(string rootKey)
        {
            InitializeComponent();
            this.rootKey = rootKey;
            LoadOptions();
        }

        private void LoadOptions()
        {
            Options opt = new Options();
            opt.Load(rootKey);
            uxCheckSpaceAsWildcard.Checked = opt.SpaceAsWildcard;
            uxCheckSearchInTheMiddle.Checked = opt.SearchInTheMiddle;
            uxCheckCamelCase.Checked = opt.UseCamelCase;
            uxCheckIgnoreExternalDependencies.Checked = opt.IgnoreExternalDependencies;
            uxCheckIgnorePatterns.Checked = opt.IgnorePatterns.Length > 0;
            uxTextIgnorePatterns.Text = String.Join(";", opt.IgnorePatterns);
            uxCheckLimitResults.Checked = opt.ResultsLimit > 0;
            uxTextLimitResults.Text = opt.ResultsLimit > 0 ? opt.ResultsLimit.ToString() : "";
            uxCheckOpenMulti.Checked = opt.ShowCheckboxes;

            uxCheckIgnorePatterns_CheckedChanged(this, EventArgs.Empty);
            uxCheckLimitResults_CheckedChanged(this, EventArgs.Empty);
        }

        private void SaveOptions()
        {
            Options opt = new Options();
            opt.LoadDefaults();
            opt.SpaceAsWildcard = uxCheckSpaceAsWildcard.Checked;
            opt.SearchInTheMiddle = uxCheckSearchInTheMiddle.Checked;
            opt.UseCamelCase = uxCheckCamelCase.Checked;
            opt.IgnoreExternalDependencies = uxCheckIgnoreExternalDependencies.Checked;
            if (uxCheckIgnorePatterns.Checked)
            {
                opt.IgnorePatterns = uxTextIgnorePatterns.Text.Split(new Char[] {';'});
                opt.IgnorePatterns = opt.IgnorePatterns.Where(s => s.Trim().Length > 0).ToArray();
            }
            else
                opt.IgnorePatterns = new string[0];
            if (uxCheckLimitResults.Checked)
            {
                try
                {
                    opt.ResultsLimit = Int32.Parse(uxTextLimitResults.Text);
                }
                catch (FormatException)
                {
                    opt.ResultsLimit = -1;
                }
            }
            else
                opt.ResultsLimit = -1;
            opt.ShowCheckboxes = uxCheckOpenMulti.Checked;
            opt.Save(rootKey);
        }

        private string rootKey;

        private void OptionsDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            //TODO: go to QoF help webpage - the VS Gallery page
        }

        private void uxOk_Click(object sender, EventArgs e)
        {
            SaveOptions();
            //Close(
        }

        private void uxCheckIgnorePatterns_CheckedChanged(object sender, EventArgs e)
        {
            uxTextIgnorePatterns.Enabled = uxCheckIgnorePatterns.Checked;
        }

        private void uxCheckLimitResults_CheckedChanged(object sender, EventArgs e)
        {
            uxTextLimitResults.Enabled = uxCheckLimitResults.Checked;
            if (uxTextLimitResults.Enabled && uxTextLimitResults.Text.Length == 0)
                uxTextLimitResults.Text = "100";
        }
    }

    /// <summary>
    /// Loads and stores the plugin options from/to windows registry.
    /// </summary>
    public class Options
    {
        public bool SpaceAsWildcard;
        public bool SearchInTheMiddle;
        public bool UseCamelCase;
        public bool IgnoreExternalDependencies;
        public string[] IgnorePatterns;
        public int ResultsLimit;
        public bool ShowCheckboxes;
        public bool AsynchronousSearch;     // changing this only takes effect after VS is restarted
        public int ShortKeystrokeDelay;
        public int LongKeystrokeDelay;

        public Options()
        { }

        public void LoadDefaults()
        {
            SpaceAsWildcard = true;
            SearchInTheMiddle = true;
            UseCamelCase = false;
            IgnoreExternalDependencies = false;
            IgnorePatterns = new string[0];
            ResultsLimit = -1;
            ShowCheckboxes = false;
            AsynchronousSearch = false;
            ShortKeystrokeDelay = 300;
            LongKeystrokeDelay = 450;
        }

        public bool Load(string rootKey)
        {
            LoadDefaults();
            string key = rootKey + "\\" + keySubkey;
            try
            {
                SpaceAsWildcard = (int)Registry.GetValue(key, keySpaceAsWildcard, SpaceAsWildcard ? 1 : 0) == 0 ? false : true;
                SearchInTheMiddle = (int)Registry.GetValue(key, keySearchInTheMiddle, SearchInTheMiddle ? 1 : 0) == 0 ? false : true;
                UseCamelCase = (int)Registry.GetValue(key, keyUseCamelCase, UseCamelCase ? 1 : 0) == 0 ? false : true;
                IgnoreExternalDependencies = (int)Registry.GetValue(key, keyIgnoreExternalDependencies, IgnoreExternalDependencies ? 1 : 0) == 0 ? false : true;
                IgnorePatterns = ((string)Registry.GetValue(key, keyIgnorePatterns, String.Join(";", IgnorePatterns))).Split(new Char[] { ';' });
                IgnorePatterns = IgnorePatterns.Where(s => s.Trim().Length > 0).ToArray();
                ResultsLimit = (int)Registry.GetValue(key, keyResultsLimit, ResultsLimit);
                ShowCheckboxes = (int)Registry.GetValue(key, keyShowCheckboxes, ShowCheckboxes ? 1 : 0) == 0 ? false : true;
                AsynchronousSearch = (int)Registry.GetValue(key, keyAsynchronousSearch, AsynchronousSearch ? 1 : 0) == 0 ? false : true;
                ShortKeystrokeDelay = (int)Registry.GetValue(key, keyShortKeystrokeDelay, ShortKeystrokeDelay);
                LongKeystrokeDelay = (int)Registry.GetValue(key, keyLongKeystrokeDelay, LongKeystrokeDelay);

                Debug.Print("QuickOpenFile.Options.Load(): Successfuly loaded from: " + key);
            }
            catch (Exception)
            {
                Debug.Print("QuickOpenFile.Options.Load(): Failed to load options from registry key: " + key);
                return false;
            }
            return true;
        }

        public bool Save(string rootKey)
        {
            string key = rootKey + "\\" + keySubkey;
            try
            {
                Registry.SetValue(key, keySpaceAsWildcard, SpaceAsWildcard ? 1 : 0, RegistryValueKind.DWord);
                Registry.SetValue(key, keySearchInTheMiddle, SearchInTheMiddle ? 1 : 0, RegistryValueKind.DWord);
                Registry.SetValue(key, keyUseCamelCase, UseCamelCase ? 1 : 0, RegistryValueKind.DWord);
                Registry.SetValue(key, keyIgnoreExternalDependencies, IgnoreExternalDependencies ? 1 : 0, RegistryValueKind.DWord);
                Registry.SetValue(key, keyIgnorePatterns, String.Join(";", IgnorePatterns), RegistryValueKind.String);
                Registry.SetValue(key, keyResultsLimit, ResultsLimit, RegistryValueKind.DWord);
                Registry.SetValue(key, keyShowCheckboxes, ShowCheckboxes ? 1 : 0, RegistryValueKind.DWord);
                Registry.SetValue(key, keyAsynchronousSearch, AsynchronousSearch ? 1 : 0, RegistryValueKind.DWord);
                Registry.SetValue(key, keyShortKeystrokeDelay, ShortKeystrokeDelay, RegistryValueKind.DWord);
                Registry.SetValue(key, keyLongKeystrokeDelay, LongKeystrokeDelay, RegistryValueKind.DWord);

                Debug.Print("QuickOpenFile.Options.Save(): Successfuly saved to: " + key);
            }
            catch (Exception)
            {
                Debug.Print("QuickOpenFile.Options.Save(): Failed to save options to registry key: " + key);
                return false;
            }
            return true;
        }

        private const string keySubkey = "QuickOpenFile";
        private const string keySpaceAsWildcard = "SpaceAsWildcard";
        private const string keySearchInTheMiddle = "SearchInTheMiddle";
        private const string keyUseCamelCase = "UseCamelCase";
        private const string keyIgnoreExternalDependencies = "IgnoreExternalDependencies";
        private const string keyIgnorePatterns = "IgnorePatterns";
        private const string keyResultsLimit = "ResultsLimit";
        private const string keyShowCheckboxes = "ShowCheckboxes";
        private const string keyAsynchronousSearch = "AsynchronousSearch";
        private const string keyShortKeystrokeDelay = "ShortKeystrokeDelay";
        private const string keyLongKeystrokeDelay = "LongKeystrokeDelay";
    }
}
