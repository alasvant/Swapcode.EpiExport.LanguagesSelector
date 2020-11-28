using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Swapcode.EpiExport.LanguagesSelector
{
    /// <summary>
    /// Export settings.
    /// </summary>
    public class ExportSettings
    {
        private readonly IReadOnlyCollection<string> _languages;

        private readonly bool _excludeIfShortcutIsSet;

        /// <summary>
        /// Creates a new instance of <see cref="ExportSettings"/> using the supplied arguments.
        /// </summary>
        /// <param name="languages">IEnumerable{string} containing language names which should be valid <see cref="System.Globalization.CultureInfo.Name"/> values.</param>
        /// <param name="excludeIfShortcutIsSet">True if content having shortcut type set should be excluded from export package otherwise false</param>
        public ExportSettings(IEnumerable<string> languages, bool excludeIfShortcutIsSet)
        {
            if (languages == null)
            {
                _languages = new ReadOnlyCollection<string>(new List<string>(0));
            }
            else
            {
                // filter out any possible empty values, create a new list and wrap it to read only collection
                _languages = new ReadOnlyCollection<string>(languages.Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
            }

            _excludeIfShortcutIsSet = excludeIfShortcutIsSet;
        }

        /// <summary>
        /// Returns a collection of languages to include to export package.
        /// </summary>
        public IReadOnlyCollection<string> Languages => _languages;

        /// <summary>
        /// Gets a boolean value should content having shortcut type set be excluded from export package.
        /// </summary>
        public bool ExcludeIfShortcutIsSet => _excludeIfShortcutIsSet;
    }
}
