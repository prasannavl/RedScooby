// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Windows.ApplicationModel.Resources;

namespace RedScooby.Utilities
{
    public class WinRtResourceManager : ResourceManager
    {
        private readonly ResourceLoader resourceLoader;

        private WinRtResourceManager(string baseName, Assembly assembly) : base(baseName, assembly)
        {
            resourceLoader = ResourceLoader.GetForViewIndependentUse(baseName);
        }

        public override string GetString(string name, CultureInfo culture)
        {
            return resourceLoader.GetString(name);
        }

        public static void InjectIntoResxGeneratedApplicationResourcesClass(Type resxGeneratedApplicationResourcesClass)
        {
            var field = resxGeneratedApplicationResourcesClass.GetRuntimeFields()
                .FirstOrDefault(m => m.Name == "resourceMan");

            if (field != null)
                field.SetValue(null,
                    new WinRtResourceManager(resxGeneratedApplicationResourcesClass.FullName,
                        resxGeneratedApplicationResourcesClass.GetTypeInfo().Assembly));
        }
    }
}
