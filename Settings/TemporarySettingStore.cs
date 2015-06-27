/* ****************************************************************************
 * Copyright 2015 Steve Dower
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace IndentGuide {
    class TemporarySettingStore : IVsSettingsReader, IVsSettingsWriter {
        private IDictionary<string, object> Values;

        public TemporarySettingStore() {
            Values = new Dictionary<string, object>();
        }

        public int ReportError(string pszError, uint dwErrorType) {
            return VSConstants.E_NOTIMPL;
        }

        public int WriteCategoryVersion(int nMajor, int nMinor, int nBuild, int nRevision) {
            Values["__nMajor"] = nMajor;
            Values["__nMinor"] = nMinor;
            Values["__nBuild"] = nBuild;
            Values["__nRevision"] = nRevision;
            return VSConstants.S_OK;
        }

        public int WriteSettingAttribute(string pszSettingName, string pszAttributeName, string pszSettingValue) {
            Values[pszSettingName + "__" + pszAttributeName] = pszSettingValue;
            return VSConstants.S_OK;
        }

        public int WriteSettingBoolean(string pszSettingName, int fSettingValue) {
            Values[pszSettingName] = fSettingValue;
            return VSConstants.S_OK;
        }

        public int WriteSettingBytes(string pszSettingName, byte[] pSettingValue, int lDataLength) {
            Values[pszSettingName] = pSettingValue;
            return VSConstants.S_OK;
        }

        public int WriteSettingLong(string pszSettingName, int lSettingValue) {
            Values[pszSettingName] = lSettingValue;
            return VSConstants.S_OK;
        }

        public int WriteSettingString(string pszSettingName, string pszSettingValue) {
            Values[pszSettingName] = pszSettingValue;
            return VSConstants.S_OK;
        }

        public int WriteSettingXml(object pIXMLDOMNode) {
            return VSConstants.E_NOTIMPL;
        }

        public int WriteSettingXmlFromString(string szXML) {
            return VSConstants.E_NOTIMPL;
        }

        public int ReadCategoryVersion(out int pnMajor, out int pnMinor, out int pnBuild, out int pnRevision) {
            try {
                pnMajor = (int)Values["__nMajor"];
                pnMinor = (int)Values["__nMinor"];
                pnBuild = (int)Values["__nBuild"];
                pnRevision = (int)Values["__nRevision"];
                return VSConstants.S_OK;
            } catch {
                pnMajor = 0;
                pnMinor = 0;
                pnBuild = 0;
                pnRevision = 0;
                return VSConstants.E_FAIL;
            }
        }

        public int ReadFileVersion(out int pnMajor, out int pnMinor, out int pnBuild, out int pnRevision) {
            pnMajor = 0;
            pnMinor = 0;
            pnBuild = 0;
            pnRevision = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int ReadSettingAttribute(string pszSettingName, string pszAttributeName, out string pbstrSettingValue) {
            try {
                pbstrSettingValue = (string)Values[pszSettingName + "__" + pszAttributeName];
                return VSConstants.S_OK;
            } catch {
                pbstrSettingValue = null;
                return VSConstants.E_FAIL;
            }
        }

        public int ReadSettingBoolean(string pszSettingName, out int pfSettingValue) {
            try {
                pfSettingValue = (int)Values[pszSettingName];
                return VSConstants.S_OK;
            } catch {
                pfSettingValue = 0;
                return VSConstants.E_FAIL;
            }
        }

        public int ReadSettingBytes(string pszSettingName, ref byte pSettingValue, out int plDataLength, int lDataMax) {
            pSettingValue = 0;
            plDataLength = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int ReadSettingLong(string pszSettingName, out int plSettingValue) {
            try {
                plSettingValue = (int)Values[pszSettingName];
                return VSConstants.S_OK;
            } catch {
                plSettingValue = 0;
                return VSConstants.E_FAIL;
            }
        }

        public int ReadSettingString(string pszSettingName, out string pbstrSettingValue) {
            try {
                pbstrSettingValue = (string)Values[pszSettingName];
                return VSConstants.S_OK;
            } catch {
                pbstrSettingValue = null;
                return VSConstants.E_FAIL;
            }
        }

        public int ReadSettingXml(string pszSettingName, out object ppIXMLDOMNode) {
            ppIXMLDOMNode = null;
            return VSConstants.E_NOTIMPL;
        }

        public int ReadSettingXmlAsString(string pszSettingName, out string pbstrXML) {
            pbstrXML = null;
            return VSConstants.E_NOTIMPL;
        }
    }
}
