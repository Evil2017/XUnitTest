﻿using System;
using System.Collections.Generic;
using Xunit.Sdk;
using ExcelDataReader;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace XUnitTest.ServiceTest
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExcelDataAttribute : DataAttribute
    {
        public ExcelDataAttribute(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            if (testMethod == null)
                throw new ArgumentNullException("testMethod is null");

            ParameterInfo[] pars = testMethod.GetParameters();
            var p1 = AppDomain.CurrentDomain;
            var p2 = Directory.GetCurrentDirectory();
            var p3 = AppDomain.CurrentDomain.BaseDirectory;
            var fileName = Path.Combine(p3, FileName);
            return DataSource(FileName, pars.Select(par => par.ParameterType).ToArray());
        }

        private IEnumerable<object[]> DataSource(string fileName, Type[] parameterTypes)
        {

            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    foreach (DataRow row in result.Tables[0].Rows)
                        yield return ConvertParameters(row.ItemArray, parameterTypes);
                }
            }
        }

        private static object[] ConvertParameters(object[] values, Type[] parameterTypes)
        {
            object[] result = new object[values.Length];

            for (int idx = 0; idx < values.Length; idx++)
                result[idx] = ConvertParameter(values[idx], idx >= parameterTypes.Length ? null : parameterTypes[idx]);

            return result;
        }

        /// <summary>
        /// Converts a parameter to its destination parameter type, if necessary.
        /// </summary>
        /// <param name="parameter">The parameter value</param>
        /// <param name="parameterType">The destination parameter type (null if not known)</param>
        /// <returns>The converted parameter value</returns>
        private static object ConvertParameter(object parameter, Type parameterType)
        {
            if ((parameter is double || parameter is float) &&
                (parameterType == typeof(int) || parameterType == typeof(int?)))
            {
                int intValue;
                string floatValueAsString = parameter.ToString();

                if (Int32.TryParse(floatValueAsString, out intValue))
                    return intValue;
            }

            return parameter;
        }
    }
}
