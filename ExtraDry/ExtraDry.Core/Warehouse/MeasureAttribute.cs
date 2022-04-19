﻿using System;

namespace ExtraDry.Core.Warehouse;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
public class MeasureAttribute : Attribute {

    public MeasureAttribute() { }

    public MeasureAttribute(string name)
    {
        Name = name;
    }

    public string? Name { get; set; }
}
