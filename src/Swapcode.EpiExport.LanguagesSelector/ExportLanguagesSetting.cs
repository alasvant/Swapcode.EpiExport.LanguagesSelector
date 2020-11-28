using System.Web.UI.WebControls;
using EPiServer.PlugIn;
using Swapcode.EpiExport.LanguagesSelector.WebControls;

namespace Swapcode.EpiExport.LanguagesSelector
{
    /// <summary>
    /// Episerver UI plugin for persisting export languages.
    /// </summary>
    [GuiPlugIn(Area = PlugInArea.None, DisplayName = "Export languages", Description = "Allows selecting what languages to be exported in Episerver Export.")]
    public class ExportLanguagesSetting
    {
        // Note: Initially was just about languages so that's why the class name is named like it is

        /// <summary>
        /// Gets or sets the selected language ids as a comma separated list. Sample: en,fi,sv
        /// </summary>
        [PlugInProperty("Language ids", "Select languages to be used in Episerver export", AdminControl = typeof(LanguagesCheckBoxList), AdminControlValue = "SelectedLanguages")]
        public string SelectedLanguageIds { get; set; }

        /// <summary>
        /// Gets or set the exclude content from export if content has shortcut type set.
        /// </summary>
        [PlugInProperty("Exclude shortcut", "Select this checkbox to exclude content from export with shortcut type set (Like 'Shortcut to another content item').", AdminControl = typeof(CheckBox), AdminControlValue = "Checked")]
        public bool ExcludeContentWithShortcut { get; set; }
    }
}
