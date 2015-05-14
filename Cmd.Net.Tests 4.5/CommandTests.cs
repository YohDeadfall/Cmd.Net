using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cmd.Net.Tests
{
    [TestClass]
    public class CommandTests
    {
        #region Nested Types

        private sealed class IntCollectionConverter : TypeConverter
        {
            #region TypeConverter Members

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(IEnumerable<string>) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                IEnumerable<string> enumerable = value as IEnumerable<string>;

                if (enumerable != null)
                {
                    return enumerable
                        .Select(s => int.Parse(s))
                        .ToArray();
                }

                return base.ConvertFrom(context, culture, value);
            }

            #endregion
        }

        private sealed class UltimateQuestionAnswerConverter : TypeConverter
        {
            #region Fields

            public const string UltimateQuestion = "Ultimate Question of Life, The Universe, and Everything";
            public const int UltimateQuestionAnswer = 42;

            #endregion

            #region TypeConverter Members

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                string question = value as string;

                if (question == UltimateQuestion)
                { return UltimateQuestionAnswer; }

                return base.ConvertFrom(context, culture, value);
            }

            #endregion
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void ArgumentWithTypeConverter()
        {
            new DelegateCommand(new Action<int>(ArgumentWithTypeconverterCommand))
                .Execute("\"" + UltimateQuestionAnswerConverter.UltimateQuestion + "\"");
        }

        [TestMethod]
        public void CollectionArgumentWithoutTypeConverter()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                stringBuilder
                    .Append(i)
                    .Append(' ');
            }

            new DelegateCommand(new Action<int[]>(CollectionArgumentWithoutTypeConverterCommand))
                .Execute(stringBuilder.ToString());
        }

        [TestMethod]
        public void CollectionArgumentWithTypeConverter()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                stringBuilder
                    .Append(i)
                    .Append(' ');
            }

            new DelegateCommand(new Action<int[]>(CollectionArgumentWithTypeConverterCommand))
                .Execute(stringBuilder.ToString());
        }

        [TestMethod]
        public void NamedArgument()
        {
            new DelegateCommand(new Action<string>(NamedArgumentCommand))
                .Execute("/question:\"" + UltimateQuestionAnswerConverter.UltimateQuestion + "\"");
        }

        [TestMethod]
        public void UnnamedArgument()
        {
            new DelegateCommand(new Action<string>(UnnamedArgumentCommand))
                .Execute("\"" + UltimateQuestionAnswerConverter.UltimateQuestion + "\"");
        }

        #endregion

        #region Private Methods

        [Verb("ArgumentWithTypeconverterCommand")]
        private static void ArgumentWithTypeconverterCommand([TypeConverter(typeof(UltimateQuestionAnswerConverter))] int answer)
        {
            Assert.AreEqual(UltimateQuestionAnswerConverter.UltimateQuestionAnswer, answer);
        }

        [Verb("CollectionArgumentWithoutTypeConverterCommand")]
        private static void CollectionArgumentWithoutTypeConverterCommand(int[] values)
        {
            Assert.AreNotEqual(null, values);
            Assert.AreEqual(4, values.Length);

            for (int i = 0; i < values.Length; i++)
            { Assert.AreEqual(i, values[i]); }
        }

        [Verb("CollectionArgumentWithTypeConverterCommand")]
        private static void CollectionArgumentWithTypeConverterCommand([TypeConverter(typeof(IntCollectionConverter))] int[] values)
        {
            Assert.AreNotEqual(null, values);
            Assert.AreEqual(4, values.Length);

            for (int i = 0; i < values.Length; i++)
            { Assert.AreEqual(i, values[i]); }
        }

        [Verb("NamedArgumentCommand")]
        private static void NamedArgumentCommand([Argument("question")] string question)
        {
            Assert.AreEqual(UltimateQuestionAnswerConverter.UltimateQuestion, question);
        }

        [Verb("UnnamedArgumentCommand")]
        private static void UnnamedArgumentCommand([Argument()] string question)
        {
            Assert.AreEqual(UltimateQuestionAnswerConverter.UltimateQuestion, question);
        }

        #endregion
    }
}
