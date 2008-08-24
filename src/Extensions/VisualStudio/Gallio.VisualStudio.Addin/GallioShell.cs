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
using System.Runtime.InteropServices;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Gallio.VisualStudio.Toolkit;
using Gallio.VisualStudio.Toolkit.Actions;

namespace Gallio.VisualStudio.Addin
{
    [ComVisible(true)]
	public class GallioShell : Shell
	{
        private static GallioShell instance;

        /// <summary>
        /// Gets the instance of the add-in.
        /// </summary>
        public static GallioShell Instance
        {
            get { return instance; }
        }

        public override void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            instance = this;

            base.OnConnection(application, connectMode, addInInst, ref custom);
        }

        public override void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            base.OnDisconnection(disconnectMode, ref custom);

            instance = null;
        }
	}
}
