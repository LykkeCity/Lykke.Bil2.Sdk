using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Lykke.Sdk.Settings;
using Lykke.SettingsReader;

namespace Lykke.Bil2.Sdk.Services
{
    public abstract class SettingsRenderer
    {
        protected static readonly IReadOnlyCollection<Regex> SanitizingRegexps = new []
        {
            // Azure storage connection string
            new Regex(@"(^|;|\s)(AccountKey\s*=\s*)(?<secure>.*?)($|;|\s)", RegexOptions.Compiled),
            // RabbitMq connection string
            new Regex(@"(amqp:\/\/)(?<secure>.*?)(@)", RegexOptions.Compiled),
            // Connection string with "Password=XXX" pattern. Postgre for example
            new Regex(@"(^|;|\s)(Password\s*=\s*)(?<secure>.*?)($|;|\s)"), 
        };
    }

    /// <inheritdoc cref="ISettingsRenderer" />
    public class SettingsRenderer<TAppSettings> : 
        SettingsRenderer,
        ISettingsRenderer

        where TAppSettings : class, IAppSettings
    {
        private readonly IReloadingManager<TAppSettings> _settings;

        public SettingsRenderer(IReloadingManager<TAppSettings> settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <inheritdoc />
        public IDictionary<string, string> RenderSettings()
        {
            var settings = _settings.CurrentValue;
            var renderedSettings = new Dictionary<string, string>();
            var type = settings.GetType();

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(ShouldRenderProperty))
            {
                foreach (var item in Render(
                    IsSecureProperty(property),
                    property.PropertyType, 
                    property.GetValue(settings), 
                    property.Name))
                {
                    renderedSettings.Add(item.Key, item.Value);
                }
            }

            return renderedSettings;
        }

        private IReadOnlyDictionary<string, string> Render(
            bool isSecure,
            Type type,
            object value, 
            string fullName)
        {
            var result = new Dictionary<string, string>();

            if (IsLeaf(type, value))
            {
                var renderedValue = RenderValue(type, value);
                var sanitizedValue = SanitizeValue(isSecure, type, renderedValue);

                result.Add(fullName, sanitizedValue);
            }
            else if (IsDictionary(type, value))
            {
                var dictionary = ((IDictionary) value);

                foreach (var key in dictionary.Keys)
                {
                    var element = dictionary[key];
                    var elementType = element != null
                        ? key.GetType()
                        : typeof(object);
                    
                    foreach (var renderedItem in Render(isSecure, elementType, key, $"{fullName}.[{key}]"))
                    {
                        result.Add(renderedItem.Key, renderedItem.Value);
                    }
                }
            }
            else if (IsCollection(type, value))
            {
                var index = 0;

                foreach (var element in (IEnumerable)value)
                {
                    var elementType = element != null
                        ? element.GetType()
                        : value.GetType().GetElementType();
                    
                    foreach (var renderedItem in Render(isSecure, elementType, element, $"{fullName}.[{index++}]"))
                    {
                        result.Add(renderedItem.Key, renderedItem.Value);
                    }
                }
            }
            else
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(ShouldRenderProperty))
                {
                    foreach (var renderedItem in Render(
                        IsSecureProperty(property),
                        property.PropertyType, 
                        property.GetValue(value), 
                        $"{fullName}.{property.Name}"))
                    {
                        result.Add(renderedItem.Key, renderedItem.Value);
                    }
                }
            }

            return result;
        }

        private bool ShouldRenderProperty(PropertyInfo property)
        {
            return property.CanRead;
        }

        private bool IsSecureProperty(PropertyInfo property)
        {
            return property.GetCustomAttribute<SecureSettingsAttribute>() != null;
        }

        private bool IsLeaf(Type type, object value)
        {
            if (value == null)
            {
                return true;
            }

            if(type == typeof(string))
            {
                return true;
            }

            if (type.IsPrimitive)
            {
                return true;
            }

            if (type.IsValueType)
            {
                return true;
            }

            return false;
        }

        private static bool IsCollection(Type type, object value)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        private static bool IsDictionary(Type type, object value)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        private static string RenderValue(Type type, object value)
        {
            return value?.ToString();
        }

        private static string SanitizeValue(bool isSecure, Type type, string value)
        {
            if (isSecure)
            {
                return "*";
            }

            if (type == typeof(string))
            {
                var result = value;

                foreach (var regex in SanitizingRegexps)
                {
                    result = regex.Replace(result, match =>
                    {
                        var sb = new StringBuilder();

                        foreach (var matchGroup in match.Groups.OrderBy(g => g.Index).Skip(1))
                        {
                            if (matchGroup.Name != "secure")
                            {
                                sb.Append(matchGroup.Value);
                            }
                            else
                            {
                                sb.Append("*");
                            }
                        }

                        return sb.ToString();
                    });
                }

                return result;
            }

            return value;
        }
    }
}
