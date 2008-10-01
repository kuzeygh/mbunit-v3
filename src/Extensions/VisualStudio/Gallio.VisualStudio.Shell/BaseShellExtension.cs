// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Abstract base class for shell extensions.
    /// </summary>
    public abstract class BaseShellExtension : IShellExtension
    {
        private IShell shell;

        /// <summary>
        /// Gets the shell, or null if the extension has been shut down.
        /// </summary>
        public IShell Shell
        {
            get { return shell; }
        }

        /// <inheritdoc />
        public void Initialize(IShell shell)
        {
            if (shell == null)
                throw new ArgumentNullException("shell");
            if (this.shell != null)
                throw new InvalidOperationException("The shell extension has already been initialized.");

            this.shell = shell;
            InitializeImpl();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            if (shell != null)
            {
                ShutdownImpl();
                shell = null;
            }
        }

        /// <summary>
        /// Initializes the shell extension.
        /// </summary>
        protected virtual void InitializeImpl()
        {
        }

        /// <summary>
        /// Shuts down the shell extension.
        /// </summary>
        protected virtual void ShutdownImpl()
        {
        }
    }
}