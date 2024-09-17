using AetherUtils.Core.Logging;

namespace WIG.Lib.Utility
{
    public sealed class ImageUtils
    {
        /// <summary>
        /// Load an image from the specified file.
        /// </summary>
        /// <param name="file">The file to load as an image.</param>
        /// <returns>The <see cref="Image"/> represented by the path or <c>null</c> if the image couldn't be loaded.</returns>
        /// <exception cref="InvalidOperationException">Occurs if the image to load was not a PNG file.</exception>
        public static Image? LoadImage(string file)
        {
            try
            {
                if (!IsPngFile(file))
                    throw new InvalidOperationException("The file is not a PNG file.");

                return Image.FromFile(file);
            }
            catch (Exception e)
            {
                AuLogger.GetCurrentLogger<ImageUtils>("LoadImage").Error(e.Message);
            }
            return null;
        }

        /// <summary>
        /// Determines if the specified file is a PNG file.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns><c>true</c> if the file is a .png; <c>false</c> otherwise.</returns>
        public static bool IsPngFile(string file) => Path.GetExtension(file).Equals(".png", StringComparison.OrdinalIgnoreCase);
    }
}
