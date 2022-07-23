using System.Collections;
using FluentAssertions;

namespace ElgatoWaveSDK.Tests.TestUtils
{
    internal static class ModelUtils
    {
        public static void Verify(Type classType, object? obj, Dictionary<string, object> values)
        {
            foreach (var field in classType.GetProperties().Where(c => c.SetMethod != null))
            {
                var setValue = values[field.Name];
                setValue.Should().NotBeNull();

                var actualValue = field.GetValue(obj);

                actualValue.Should().NotBeNull();
                actualValue.Should().BeEquivalentTo(setValue);
            }
        }

        public static (object?, Dictionary<string, object> values) GenerateValues(Type objType)
        {
            var values = new Dictionary<string, object>();
            var obj = Activator.CreateInstance(objType);
            if (obj != null)
            {
                foreach (var field in objType.GetProperties().Where(c => c.SetMethod != null))
                {
                    object? value = null;
                    var type = Nullable.GetUnderlyingType(field.PropertyType) ?? field.PropertyType;
                    if (type.IsPrimitive || type == typeof(string))
                    {
                        value = GeneratePrimitiveValue(type);
                    }
                    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var innerType = type.GetGenericArguments().First();
                        var tempValue = Activator.CreateInstance(type);
                        var q = new Random().Next(1, 5);
                        for (int i = 0; i < q; i++)
                        {
                            object? listValue = null;
                            if (innerType.IsPrimitive || innerType == typeof(string))
                            {
                                listValue = GeneratePrimitiveValue(innerType);
                            }
                            else
                            {
                                listValue = GenerateValues(innerType).Item1;
                            }

                            if (listValue != null)
                            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                                _ = ((IList)tempValue).Add(listValue);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                            }
                        }

                        value = tempValue;
                    }
                    else
                    {
                        value = GenerateValues(type).Item1;
                    }

                    if (value != null)
                    {
                        values.Add(field.Name, value);
                        field.SetValue(obj, value);
                    }
                }
            }

            return (obj, values);
        }

        private static object? GeneratePrimitiveValue(Type primitiveType)
        {
            if (!primitiveType.IsPrimitive && primitiveType != typeof(string))
            {
                throw new Exception("Type is not primitive");
            }

            var random = new Random();

            if (primitiveType == typeof(string))
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
            }

            if (primitiveType == typeof(bool))
            {
                return random.Next(0, 1) == 1;
            }

            if (primitiveType == typeof(int))
            {
                return random.Next(int.MaxValue);
            }
            if (primitiveType == typeof(double))
            {
                return random.NextDouble();
            }
            if (primitiveType == typeof(long) || primitiveType == typeof(long))
            {
                return random.NextInt64();
            }
            if (primitiveType == typeof(float))
            {
                return random.NextSingle();
            }

            return null;
        }
    }
}
