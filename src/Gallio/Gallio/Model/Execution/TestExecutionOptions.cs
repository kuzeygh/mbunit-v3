// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Model.Filters;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Provides options that control how test execution occurs.
    /// </summary>
    [Serializable]
    public sealed class TestExecutionOptions
    {
        private FilterSet<ITest> filterSet = FilterSet<ITest>.Empty;
        private bool skipDynamicTests;
        private bool skipTestExecution;
        private bool exactFilter;

        /// <summary>
        /// Gets or sets the filter set.
        /// </summary>
        /// <value>Defaults to an empty filter set.</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public FilterSet<ITest> FilterSet
        {
            get { return filterSet; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                filterSet = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the filter exactly specifies all tests to select.
        /// If false, then children of the selected tests are also included.
        /// </summary>
        /// <value>Defaults to false.</value>
        public bool ExactFilter
        {
            get { return exactFilter; }
            set { exactFilter = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether to skip running tests that use dynamic data items.
        /// </para>
        /// <para>
        /// This flag can be useful in combination with <see cref="SkipTestExecution" />
        /// to enumerate non-dynamic tests only.
        /// </para>
        /// </summary>
        /// <value>Defaults to <c>false</c></value>
        public bool SkipDynamicTests
        {
            get { return skipDynamicTests; }
            set { skipDynamicTests = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether to skip the execution of tests.
        /// </para>
        /// <para>
        /// The test runner will go through most of the motions of running tests but will skip
        /// the actual execution phase.  This option can be used to enumerate data-driven test
        /// steps without running them.  It can also be used to verify that the execution
        /// environment is sane without doing most of the work of running the tests.
        /// </para>
        /// </summary>
        /// <value>Defaults to <c>false</c></value>
        public bool SkipTestExecution
        {
            get { return skipTestExecution; }
            set { skipTestExecution = value; }
        }

        /// <summary>
        /// Creates a copy of the options.
        /// </summary>
        /// <returns>The copy</returns>
        public TestExecutionOptions Copy()
        {
            TestExecutionOptions copy = new TestExecutionOptions();

            copy.filterSet = filterSet;
            copy.skipDynamicTests = skipDynamicTests;
            copy.skipTestExecution = skipTestExecution;

            return copy;
        }
    }
}