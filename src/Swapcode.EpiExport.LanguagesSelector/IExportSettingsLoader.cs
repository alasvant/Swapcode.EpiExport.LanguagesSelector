namespace Swapcode.EpiExport.LanguagesSelector
{
    /// <summary>
    /// Interface definition for a service that can load the <see cref="ExportSettings"/>.
    /// </summary>
    public interface IExportSettingsLoader
    {
        /// <summary>
        /// Gets the export settings.
        /// </summary>
        /// <returns>Loaded <see cref="ExportSettings"/> instance.</returns>
        /// <remarks><para>This method could throw an exception.</para></remarks>
        ExportSettings GetExportSettings();
    }
}
