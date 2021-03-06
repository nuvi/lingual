using System;
using System.Collections.Generic;
using System.Globalization;
using Lingual.Infrastructure;
using System.IO;
using System.Linq;

namespace Lingual
{
    public interface ILocaleDirectoryLoader
    {
        Dictionary<CultureInfo, ICultureTranslator> ParseCultureTranslators(string directoryPath);
    }

    public class LocaleDirectoryLoader : ILocaleDirectoryLoader
    {
        public Dictionary<CultureInfo, ICultureTranslator> ParseCultureTranslators(string directoryPath)
        {
            return directoryPath
                .If(Directory.Exists)
                .Let(directory => Directory.GetFiles(directory)
                    .Where(fileName => Path.GetExtension(fileName) == ".json")
                    .Select(fileName => new 
                    {
                        translator = CultureTranslator.FromFile(fileName),
                        culture = CultureFromLocaleFile(fileName)
                    })
                    .Where(i => i.culture != null)
                    .ToDictionary(i => i.culture, i => i.translator))
                .Recover(() => new Dictionary<CultureInfo,ICultureTranslator>());
        }
        public CultureInfo CultureFromLocaleFile(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName)
                .TryLet(shortFileName => new CultureInfo(shortFileName));
        }
	}
}

