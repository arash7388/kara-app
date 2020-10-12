using System;
using System.Collections.Generic;
using System.Linq;
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

    public class NumberInPersianDigitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                return ((int)value).ToString().ReplaceLatinDigits();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(((string)value).ReplacePersianDigits());
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
