﻿using ExtraDry.Server.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Xunit;

namespace ExtraDry.Core.Tests.Server.Internal {

    public class ModelDescrptionTests {

        [Fact]
        public void StabilizerPropertyIsKeyAttribute()
        {
            var modelDescription = new ModelDescription(typeof(KeyAttributeEntity));

            Assert.NotNull(modelDescription);
            Assert.NotNull(modelDescription.StabilizerProperty);
            Assert.Equal("PrimaryKey", modelDescription.StabilizerProperty.ExternalName);
        }

        [Fact]
        public void StabilizerPropertyIsIdConvention()
        {
            var modelDescription = new ModelDescription(typeof(IdConventionEntity));

            Assert.NotNull(modelDescription);
            Assert.NotNull(modelDescription.StabilizerProperty);
            Assert.Equal("Id", modelDescription.StabilizerProperty.ExternalName);
        }

        [Fact]
        public void StabilizerPropertyIsClassNameConvention()
        {
            var modelDescription = new ModelDescription(typeof(ClassNameConventionEntity));

            Assert.NotNull(modelDescription);
            Assert.NotNull(modelDescription.StabilizerProperty);
            Assert.Equal("ClassNameConventionEntityId", modelDescription.StabilizerProperty.ExternalName);
        }

        [Fact]
        public void StabilizerPropertyIsMissing()
        {
            var modelDescription = new ModelDescription(typeof(NoImplicitStabilizer));

            var exception = Assert.Throws<DryException>(() => modelDescription.StabilizerProperty);

            Assert.NotNull(exception);
            Assert.Equal("Unable to Sort. 0x0F3F241C", exception.UserMessage);
        }

        [Fact]
        public void StabilizerPropertyIsCompositeKey()
        {
            var modelDescription = new ModelDescription(typeof(CompositeKeyAttributeEntity));

            var exception = Assert.Throws<DryException>(() => modelDescription.StabilizerProperty);

            Assert.NotNull(exception);
            Assert.Equal("Unable to Sort. 0x0F3F241D", exception.UserMessage);
        }

        [Fact]
        public void SortPropertiesFiltered()
        {
            var modelDescription = new ModelDescription(typeof(SortPropertiesEntity));

            Assert.Equal(2, modelDescription.SortProperties.Count);
            Assert.Equal("Name", modelDescription.SortProperties[0].ExternalName);
            Assert.Equal("ExternalName", modelDescription.SortProperties[1].ExternalName);
        }

        [Fact]
        public void FilterPropertiesFiltered()
        {
            var modelDescription = new ModelDescription(typeof(SortPropertiesEntity));

            Assert.Equal(2, modelDescription.FilterProperties.Count);
            Assert.Equal("Name", modelDescription.FilterProperties[0].ExternalName);
            Assert.Equal("ExternalName", modelDescription.FilterProperties[1].ExternalName);
        }

        public class KeyAttributeEntity {

            [Key]
            public int PrimaryKey { get; set; }

            public string Payload { get; set; } = string.Empty;

        }

        public class IdConventionEntity {
            public int Id { get; set; }

            public string Payload { get; set; } = string.Empty;
        }

        public class ClassNameConventionEntity {
            public int ClassNameConventionEntityId { get; set; }

            public string Payload { get; set; } = string.Empty;
        }

        public class NoImplicitStabilizer {
            public int PrimaryKey { get; set; }

            public string Payload { get; set; } = string.Empty;
        }

        public class CompositeKeyAttributeEntity {

            [Key]
            public int PrimaryKey { get; set; }

            [Key]
            public int SecondaryKey { get; set; }

            public string Payload { get; set; } = string.Empty;

        }

        public class SortPropertiesEntity {
            
            [Key]
            public int Key { get; set; }

            [Filter(FilterType.Contains)]
            public string Name { get; set; }

            [Filter(FilterType.Equals)]
            [JsonPropertyName("externalName")]
            public string InternalName { get; set; }

            public object Entity { get; set; }

            public ICollection<object> Collection { get; set; }

            [JsonIgnore]
            public string Ignored { get; set; }

            [NotMapped]
            public string NotMapped { get; set; }
        }
    }
}
