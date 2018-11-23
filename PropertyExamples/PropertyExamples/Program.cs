using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PropertyExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            ValidationHelperResult ValidResult = ValidationHelper.CheckModel(new Product
            {
                Id = 1,
                Price = 1,
                ProductName = ""
            });

            Console.WriteLine(ValidResult.IsValid + ", " + ValidResult.Desription.ToString());

            Console.ReadKey();
        }

        public class ValidationHelperResult
        {
            public bool IsValid { get; set; }
            public string Desription { get; set; }
        }

        public static class ValidationHelper
        {
            public static ValidationHelperResult CheckModel<T>(T obj)
            {
                PropertyInfo[] propertyInfos =
                    obj.GetType().GetProperties().ToArray();

                foreach (var property in propertyInfos)
                {
                    object[] attrs = property.GetCustomAttributes(false);
                    foreach (var attr in attrs)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            NotNullAttribute attrNotNull = attr as NotNullAttribute;
                            if (attrNotNull != null)
                            {
                                if (property.GetValue(obj) == null || string.IsNullOrEmpty(property.GetValue(obj).ToString()))
                                {
                                    return new ValidationHelperResult
                                    {
                                        IsValid = false,
                                        Desription = property.Name + " null or empty"
                                    };
                                }
                            }
                        }

                        if (property.PropertyType == typeof(int))
                        {
                            LengthIntervalAttribute attrLength = attr as LengthIntervalAttribute;
                            if (attrLength != null)
                            {
                                var min = attrLength.MinLength;
                                var max = attrLength.MaxLength;

                                if (property.GetValue(obj) == null) continue;

                                if (!(Convert.ToInt32(property.GetValue(obj)) > min && Convert.ToInt32(property.GetValue(obj)) < max))
                                {
                                    return new ValidationHelperResult
                                    {
                                        IsValid = false,
                                        Desription = property.Name + " exceed length"
                                    };
                                }
                            }
                        }
                    }

                }
                return new ValidationHelperResult
                {
                    IsValid = true,
                    Desription = "Success"
                };
            }
        }

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class NotNullAttribute : Attribute
        {

        }

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
        public class LengthIntervalAttribute : Attribute
        {
            public readonly int MaxLength;
            public readonly int MinLength;

            public LengthIntervalAttribute(int minLength, int maxLength)
            {
                this.MaxLength = maxLength;
                this.MinLength = minLength;
            }
        }


        public class Product
        {
            public int Id { get; set; }
            [NotNull]
            public string ProductName { get; set; }
            [LengthInterval(5, 10)]
            public int Price { get; set; }
        }


    }
}
