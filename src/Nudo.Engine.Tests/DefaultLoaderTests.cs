﻿using System;
using System.IO;
using Nudo.Engine.Loader;
using Nudo.Engine.Logging;
using Shouldly;
using Xunit;

namespace Nudo.Engine.Tests.Loader
{
    public class DefaultLoaderTests
    {
        private readonly DefaultLoader _loader;
        private NudoSettings _settings;

        public DefaultLoaderTests()
        {
            _settings = new NudoSettings { Output = new StringWriter() };
            _loader = new DefaultLoader(new DefaultLog(_settings));
        }

        [Fact]
        public void ShouldLoadSparkMakefiles()
        {
            var options = new Options
            {
                Makefile = Path.Combine("Files", "makefile.shade")
            };
            var builder = _loader.Load(options);
            builder.ShouldNotBe(null);
        }

        [Fact]
        public void ShouldFailIfFileNotFound()
        {
            var options = new Options
            {
                Makefile = Path.Combine("Files", "no-such-file.shade")
            };
            Should.Throw<Exception>(() => _loader.Load(options));
        }

        [Fact]
        public void DependenciesShouldBeRegistered()
        {
            var options = new Options
            {
                Makefile = Path.Combine("Files", "DependenciesShouldBeRegistered.shade")
            };

            var builder = _loader.Load(options);
            builder.Targets.Count.ShouldBe(2);

            builder.Targets["default"].Description.ShouldBe("Default dependency");
            builder.Targets["default"].Dependencies.ShouldBe(new[] { "another" });

            builder.Targets["another"].Description.ShouldBe("Another dependency");
            builder.Targets["another"].Dependencies.Count.ShouldBe(0);
        }
    }
}
