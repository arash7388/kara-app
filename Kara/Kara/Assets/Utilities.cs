using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kara.Assets
{
    public enum DeviceState
    {
        DeviceIsOff = 0,
        GPSIsOff = 1,
        NetworkIsOff = 2,
        GPSAndNetworkAreOff = 3,
        LocationNotAvailable = 4,
        LocationWithTooMuchError = 5,
        GoodLocation = 6
    }

    public enum DatePresentation
    {
        Shamsi = 1,
        Joulian = 2
    }

    public class DecimalToPersianDigitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        
        {
            return value.ToString().ToPersianDigits();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDecimal(((string)value).ToLatinDigits());
        }
    }

    public static class StringExt
    {
        public static string ToSafeString(this object input)
        {
            if (input == null)
                return "";

            return input.ToString();
        }

        public static decimal ToSafeDecimal(this object input)
        {
            if (input == null)
                return 0;

            if (decimal.TryParse(input.ToSafeString(), out decimal d))
                return d;
            else
                return 0;
        }

        public static Guid ToSafeGuid(this object input)
        {
            if (input.ToSafeString() == null)
                return Guid.Empty;

            return Guid.Parse(input.ToString());
        }
    }


    public class MaskedBehavior : Behavior<Entry>
    {
        private string _mask = "";
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                SetPositions();
            }
        }
                
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }
               

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        IDictionary<int, char> _positions;

        void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
                if (Mask[i] != 'X')
                    list.Add(i, Mask[i]);

            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            var text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
                return;

            if (text.Length > _mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in _positions)
                if (text.Length >= position.Key + 1)
                {
                    var value = position.Value.ToString();
                    if (text.Substring(position.Key, 1) != value)
                        text = text.Insert(position.Key, value);
                }

            if (entry.Text != text)
                entry.Text = text.ToPersianDigits();
        }
    }

    //public static class ObjectCopier
    //{
    //    /// <summary>
    //    /// Perform a deep Copy of the object.
    //    /// </summary>
    //    /// <typeparam name="T">The type of object being copied.</typeparam>
    //    /// <param name="source">The object instance to copy.</param>
    //    /// <returns>The copied object.</returns>
    //    public static T Clone<T>(T source)
    //    {
    //        if (!typeof(T).IsSerializable)
    //        {
    //            throw new ArgumentException("The type must be serializable.", "source");
    //        }

    //        // Don't serialize a null object, simply return the default for that object
    //        if (Object.ReferenceEquals(source, null))
    //        {
    //            return default(T);
    //        }

    //        IFormatter formatter = new BinaryFormatter();
    //        Stream stream = new MemoryStream();
    //        using (stream)
    //        {
    //            formatter.Serialize(stream, source);
    //            stream.Seek(0, SeekOrigin.Begin);
    //            return (T)formatter.Deserialize(stream);
    //        }
    //    }
    //}
}
