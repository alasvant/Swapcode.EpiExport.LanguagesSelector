using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using EPiServer.DataAbstraction;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace Swapcode.EpiExport.LanguagesSelector.WebControls
{
    /// <summary>
    /// Control to display the system enabled languages as a check box list.
    /// </summary>
    public class LanguagesCheckBoxList : CheckBoxList
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(LanguagesCheckBoxList));

        /// <summary>
        /// Gets or sets the selected languages. Note this should be a comma separated list of language ids. Sample: en,fi,sv
        /// </summary>
        public string SelectedLanguages
        {
            get
            {
                EnsureLanguageItems();

                if (Items.Count == 0)
                {
                    _logger.Debug("SelectedLanguages getter, returning an empty string as there are no selected items in the languages list.");
                    return string.Empty;
                }

                string values = string.Join(",", Items.Cast<ListItem>().Where(x => x.Selected).Select(z => z.Value));

                _logger.Debug($"SelectedLanguages getter, selected values string: '{values}'.");

                return values;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _logger.Debug($"SelectedLanguages setter, setting value to: '{value}'.");

                    EnsureLanguageItems();

                    // split the language string
                    var splitted = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (splitted.Length > 0)
                    {
                        for (int i = 0; i < Items.Count; i++)
                        {
                            var li = Items[i];

                            if (splitted.Contains(li.Value, StringComparer.OrdinalIgnoreCase))
                            {
                                li.Selected = true;
                            }
                        }
                    }
                }
                else
                {
                    _logger.Debug("SelectedLanguages setter, value is null, empty or whitespaces.");
                }
            }
        }

        /// <summary>
        /// Loads system enabled languages to the Items collection.
        /// </summary>
        private void EnsureLanguageItems()
        {
            if (Items.Count == 0)
            {
                try
                {
                    var listOfEnabledLanguages = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>().ListEnabled();

                    foreach (var language in listOfEnabledLanguages)
                    {
                        Items.Add(new ListItem { Text = language.Name, Value = language.LanguageID });
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("There was an error populating the list with system enabled languages.", ex);
                }
            }
        }

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            EnsureLanguageItems();

            return base.LoadPostData(postDataKey, postCollection);
        }

        protected override void CreateChildControls()
        {
            EnsureLanguageItems();

            base.CreateChildControls();
        }
    }
}
