﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace Blazor.ExtraDry {

    public class ContentLayout {

        public Collection<ContentSection> Sections { get; set; } = new Collection<ContentSection>();

    }

    public class ContentSection {

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SectionLayout Layout { get; set; } = SectionLayout.Single;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContentTheme Theme { get; set; } = ContentTheme.Light;

        public Collection<ContentContainer> Containers { get; set; } = new Collection<ContentContainer>();

        [JsonIgnore]
        public IEnumerable<ContentContainer> DisplayContainers {
            get {
                while(Containers.Count < ContainerCount) {
                    Containers.Add(new ContentContainer());
                }
                return Containers.Take(ContainerCount);
            }
        }

        private int ContainerCount => Layout switch {
            SectionLayout.Single => 1,
            SectionLayout.Triple => 3,
            SectionLayout.Quadruple => 4,
            _ => 2,
        };

    }

    public class ContentContainer {

        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContentAlignment Alignment { get; set; } = ContentAlignment.TopLeft;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContentPadding Padding { get; set; } = ContentPadding.None;

        /// <summary>
        /// The HTML for the content element.  This HTML should be kept very clean, most tags and styles are invalid.
        /// </summary>
        public string Html { get; set; } = string.Empty;

    }

    public enum ContentAlignment {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public enum ContentTheme {
        Light,
        Dark,
        Accent,
        Banner,
    }

    public enum SectionLayout {

        [Display(Name="single")]
        Single,

        [Display(Name = "double")]
        Double,

        [Display(Name = "triple")]
        Triple,

        [Display(Name = "double left")]
        DoubleWeightedLeft,

        [Display(Name = "double right")]
        DoubleWeightedRight,

        [Display(Name = "quadruple")]
        Quadruple,
    }

    public enum ContentPadding {
        Single,
        Double,
        None,
    }
}
