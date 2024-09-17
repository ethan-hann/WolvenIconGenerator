using Newtonsoft.Json;
using System.ComponentModel;

namespace WIG.Lib.Models
{
    public class RadioExtCustomIcon : INotifyPropertyChanged, ICloneable, IEquatable<RadioExtCustomIcon>
    {
        private string _inkAtlasPart = "custom_texture_part";
        private string _inkAtlasPath = "path\\to\\custom\\atlas.inkatlas";

        /// <summary>
        ///     Points to the <c>.inkatlas</c> that holds the icon texture.
        /// </summary>
        [JsonProperty("inkAtlasPath")]
        public string InkAtlasPath
        {
            get => _inkAtlasPath;
            set
            {
                _inkAtlasPath = value;
                OnPropertyChanged(nameof(InkAtlasPath));
            }
        }

        /// <summary>
        ///     Specifies which part of the <c>.inkatlas</c> should be used for the icon.
        /// </summary>
        [JsonProperty("inkAtlasPart")]
        public string InkAtlasPart
        {
            get => _inkAtlasPart;
            set
            {
                _inkAtlasPart = value;
                OnPropertyChanged(nameof(InkAtlasPart));
            }
        }

        public object Clone()
        {
            return new RadioExtCustomIcon()
            {
                InkAtlasPath = InkAtlasPath,
                InkAtlasPart = InkAtlasPart
            };
        }

        public bool Equals(RadioExtCustomIcon? other)
        {
            if (other == null) return false;
            return InkAtlasPath == other.InkAtlasPath &&
                   InkAtlasPart == other.InkAtlasPart;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RadioExtCustomIcon);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InkAtlasPath, InkAtlasPart);
        }
    }
}
