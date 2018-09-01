using CsWebChat.WpfClient.ChatModule.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CsWebChat.WpfClient.ChatModule.Converter
{
    class ConnectionStateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = String.Empty;

            try
            {
                var connectionState = (ConnectionState)value;
                result = Enum.GetName(typeof(ConnectionState), connectionState);
            } catch
            {
                result = "Invalid conversion type";
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
