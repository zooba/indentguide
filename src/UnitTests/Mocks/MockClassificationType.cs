/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;

namespace TestUtilities.Mocks {
    public class MockClassificationType : IClassificationType, ILayeredClassificationType {
        public MockClassificationType(ClassificationLayer layer, string name, IClassificationType[] bases) {
            Layer = layer;
            Classification = name;
            BaseTypes = bases;
        }

        public IEnumerable<IClassificationType> BaseTypes { get; }

        public string Classification { get; }

        public ClassificationLayer Layer { get; }

        public bool IsOfType(string type) {
            if (type == Classification) {
                return true;
            }

            foreach (var baseType in BaseTypes) {
                if (baseType.IsOfType(type)) {
                    return true;
                }
            }
            return false;
        }
    }
}
