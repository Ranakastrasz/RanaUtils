using System;
using System.Reflection;
using System.Text.Json;

namespace RanaUtils.IO
{
    public static class JsonUtility
    {
        private static readonly JsonSerializerOptions OPTIONS = new()
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = true
        };

        public static T? FromJson<T>(string newJson)
        {
            if (string.IsNullOrWhiteSpace(newJson))
                return default;

            if (newJson.Trim() == "null")
                return default;

            return JsonSerializer.Deserialize<T>(newJson, OPTIONS);
        }

        public static string ToJson<T>(T? objectToSerialize)
        {
            return JsonSerializer.Serialize(objectToSerialize, OPTIONS);
        }

        public static void FromJsonOverwrite(string json, object objectToOverwrite)
        {
            if (objectToOverwrite is null)
                throw new ArgumentNullException(nameof(objectToOverwrite));

            if (string.IsNullOrWhiteSpace(json))
                return;

            Type targetType = objectToOverwrite.GetType();
            object? parsed = JsonSerializer.Deserialize(json, targetType, OPTIONS);

            if (parsed is null)
                return;

            foreach (PropertyInfo property in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanWrite || property.GetIndexParameters().Length > 0)
                    continue;

                PropertyInfo? sourceProperty = parsed.GetType().GetProperty(
                    property.Name,
                    BindingFlags.Instance | BindingFlags.Public);

                if (sourceProperty is null || !sourceProperty.CanRead)
                    continue;

                object? value = sourceProperty.GetValue(parsed);
                property.SetValue(objectToOverwrite, value);
            }

            foreach (FieldInfo field in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                FieldInfo? sourceField = parsed.GetType().GetField(
                    field.Name,
                    BindingFlags.Instance | BindingFlags.Public);

                if (sourceField is null)
                    continue;

                field.SetValue(objectToOverwrite, sourceField.GetValue(parsed));
            }
        }
    }
}
