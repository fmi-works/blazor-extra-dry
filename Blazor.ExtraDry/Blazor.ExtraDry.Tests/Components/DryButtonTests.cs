﻿#nullable enable

using Blazor.ExtraDry.Models;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazor.ExtraDry.Tests.Components {
    public class DryButtonTests : IDisposable {

        [Fact]
        public void DryButtonNoCommandAttributeDefaults()
        {
            var commandInfo = new CommandInfo(this, ParameterlessMethod);
            var fragment = context.RenderComponent<DryButton>(("Command", commandInfo));

            var button = fragment.Find("button");
            var span = button.FirstChild;

            Assert.Equal(nameof(CommandContext.Alternate).ToLowerInvariant(), button.ClassName);
            Assert.Null(button.Attributes["disabled"]);
            Assert.Equal("Parameterless Method", span.TextContent); // Note spacing introduced.
        }

        [Fact]
        public void DryButtonExecutesParameterless()
        {
            var commandInfo = new CommandInfo(this, ParameterlessMethod);
            var fragment = context.RenderComponent<DryButton>(("Command", commandInfo));

            fragment.Find("button").Click();

            Assert.Equal(nameof(ParameterlessMethod), LastMethodClicked);
        }

        private void ParameterlessMethod()
        {
            LastMethodClicked = nameof(ParameterlessMethod);
        }


        [Fact]
        public void DecoratedAttributeDefaults()
        {
            var commandInfo = new CommandInfo(this, DecoratedParameterlessMethod);
            var fragment = context.RenderComponent<DryButton>(("Command", commandInfo));

            var button = fragment.Find("button");
            var span = fragment.Find("span");
            var icon = fragment.Find("i");

            Assert.Equal(nameof(CommandContext.Primary).ToLowerInvariant(), button.ClassName);
            Assert.Null(button.Attributes["disabled"]);
            Assert.Equal("Click Me", span.TextContent);
            Assert.Contains("plus", icon.ClassName);
        }

        [Command(CommandContext.Primary, Icon = "plus", Name = "Click Me")]
        private void DecoratedParameterlessMethod()
        {
            LastMethodClicked = nameof(DecoratedParameterlessMethod);
        }

        [Fact]
        public async Task DryButtonExecutesParameterMethod()
        {
            var commandInfo = new CommandInfo(this, AsyncMethod);
            var fragment = context.RenderComponent<DryButton>(("Command", commandInfo));

            await fragment.Find("button").ClickAsync(new MouseEventArgs());

            Assert.Equal(nameof(AsyncMethod), LastMethodClicked);
        }

        private async Task AsyncMethod()
        {
            await Task.Delay(1); // delay before setting name, ensures it's called with await and not running just sync portion of method.
            LastMethodClicked = nameof(AsyncMethod);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            context?.Dispose();
        }

        private readonly TestContext context = new();

        private string LastMethodClicked { get; set; } = string.Empty;
        
    }
}
