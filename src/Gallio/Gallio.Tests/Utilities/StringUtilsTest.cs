// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
{
    [TestFixture]
    [TestsOn(typeof(StringUtils))]
    public class StringUtilsTest
    {
        [Test]
        [Row("string", 0, "")]
        [Row("string", 3, "str")]
        [Row("string", 5, "strin")]
        [Row("string", 6, "string")]
        [Row("string", 7, "string")]
        [Row("string", 100, "string")]
        public void Truncate(string str, int maxLength, string expectedResult)
        {
            Assert.AreEqual(expectedResult, StringUtils.Truncate(str, maxLength));
        }

        [Test]
        [Row("string", 0, "")]
        [Row("string", 1, "s")]
        [Row("string", 2, "st")]
        [Row("string", 3, "str")]
        [Row("string", 4, "s...")]
        [Row("string", 5, "st...")]
        [Row("string", 6, "string")]
        [Row("string", 7, "string")]
        [Row("string", 100, "string")]
        public void TruncateWithEllipsis(string str, int maxLength, string expectedResult)
        {
            Assert.AreEqual(expectedResult, StringUtils.TruncateWithEllipsis(str, maxLength));
        }

        [Test]
        [Row(0x0, '0')]
        [Row(0x9, '9')]
        [Row(0xa, 'a')]
        [Row(0xf, 'f')]
        [Row(0x10, '0')]
        public void ToHexDigit(int value, char expectedResult)
        {
            Assert.AreEqual(expectedResult, StringUtils.ToHexDigit(value));
        }

        [Test]
        [Row(' ', "' '")]
        [Row('.', "'.'")]
        [Row('(', "'('")]
        [Row('5', "'5'")]
        [Row('a', "'a'")]
        [Row('\0', "'\\0'")]
        [Row('\a', "'\\a'")]
        [Row('\b', "'\\b'")]
        [Row('\f', "'\\f'")]
        [Row('\n', "'\\n'")]
        [Row('\r', "'\\r'")]
        [Row('\t', "'\\t'")]
        [Row('\v', "'\\v'")]
        [Row('\'', "'\\\''")]
        [Row('\"', "'\\\"'")]
        [Row('\\', "'\\\\'")]
        [Row('\ufeff', "'\\ufeff'")]
        public void ToCharLiteral(char value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, StringUtils.ToCharLiteral(value));
        }

        [Test]
        [Row(' ', " ")]
        [Row('.', ".")]
        [Row('(', "(")]
        [Row('5', "5")]
        [Row('a', "a")]
        [Row('\0', "\\0")]
        [Row('\a', "\\a")]
        [Row('\b', "\\b")]
        [Row('\f', "\\f")]
        [Row('\n', "\\n")]
        [Row('\r', "\\r")]
        [Row('\t', "\\t")]
        [Row('\v', "\\v")]
        [Row('\'', "\\\'")]
        [Row('\"', "\\\"")]
        [Row('\\', "\\\\")]
        [Row('\ufeff', "\\ufeff")]
        public void ToUnquotedCharLiteral(char value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, StringUtils.ToUnquotedCharLiteral(value));
        }

        [Test]
        [Row("", "\"\"")]
        [Row("abcdef", "\"abcdef\"")]
        [Row("\0\a\b\f\n\r\t\v\'\"\\", "\"\\0\\a\\b\\f\\n\\r\\t\\v\\'\\\"\\\\\"")]
        public void ToStringLiteral(string value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, StringUtils.ToStringLiteral(value));
        }

        [Test]
        [Row("", "")]
        [Row("abcdef", "abcdef")]
        [Row("\0\a\b\f\n\r\t\v\'\"\\", "\\0\\a\\b\\f\\n\\r\\t\\v\\'\\\"\\\\")]
        public void ToUnquotedStringLiteral(string value, string expectedResult)
        {
            Assert.AreEqual(expectedResult, StringUtils.ToUnquotedStringLiteral(value));
        }
    }
}