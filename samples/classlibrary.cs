using System.Reflection;
using Avalanche.Localization;
using Avalanche.Utilities;
using Microsoft.Extensions.DependencyInjection;
using static System.Console;

class classlibrary
{
    public static void Run()
    {
        {
            ILocalizableText text = Localization.Default.LocalizableTextCached["Namespace.Apples"];
        }
        {
            ILocalizableText text = Facade.Localization.LocalizableTextCached["Namespace.Apples"];
        }

    }

        public class Facade
        {
            /// <summary>Library specific localization</summary>
            static ILocalization? localization;
            /// <summary>Get localization with customizations</summary>
            public static ILocalization Localization
            {
                get
                {
                    // Get reference
                    ILocalization? _localization = localization;
                    // Return reference
                    if (_localization != null) return _localization;
                    // 
                    lock (typeof(Facade))
                    {
                        // Get reference again
                        _localization = localization;
                        // Return reference
                        if (_localization != null) return _localization;
                        // Configure
                        _localization = Avalanche.Localization.Localization.Default
                            .AddFileSystemWithPattern(
                                new LocalizationFileSystemEmbedded(typeof(Facade).Assembly),
                                "library/library.embedded.{Key}",
                                "library/library.embedded.{Key}");
                        // Return
                        return localization = _localization;
                    }
                }
            }
        }
    public class Facade2
    {
        /// <summary>Library specific localization</summary>
        static Lazy<ILocalization> localization = new Lazy<ILocalization>(() => Avalanche.Localization.Localization.CreateDefault());
        /// <summary>Get localization with customizations</summary>
        public static ILocalization Localization => localization.Value;
    }

    public class MyService
    {
        ITextLocalizer<MyService> localizer;

        public MyService(ITextLocalizer<MyService> localizer)
        {
            this.localizer = localizer;
        }
    }
}
