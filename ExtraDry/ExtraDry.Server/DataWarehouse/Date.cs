﻿namespace ExtraDry.Server.DataWarehouse;

[DimensionTable]
public class Date {

    public Date(int sequence, DayType dayType)
    {
        Sequence = sequence;
        DayType = dayType;
        Id = CreateId(sequence, dayType);
        Value = new DateOnly(1970, 1, 1).AddDays(sequence);
    }

    /// <summary>
    /// For a given date sequence and day type, creates a deterministic and monotonically increasing Id.
    /// </summary>
    public static int CreateId(int sequence, DayType dayType)
    {
        return 5 * sequence + (int)dayType;
    }

    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The Excel date format for the date, number of days since Jan 1 1970.
    /// </summary>
    public int Sequence { get; set; }

    [Attribute("Date")]
    public DateOnly Value { get; set; }

    /// <summary>
    /// The year.
    /// </summary>
    public int Year => Value.Year;

    /// <summary>
    /// The month of the year, 1 indexed from January
    /// </summary>
    public int Month => Value.Month;

    /// <summary>
    /// The day of the month, 1 indexed.
    /// </summary>
    public int Day => Value.Day;

    /// <summary>
    /// The number of the day of the week, 1 indexed using Monday as starting day of week.
    /// </summary>
    /// <remarks>Monday is start of week per ISO 8601.</remarks>
    [Attribute("Day of Week")]
    public int DayOfWeek => Value.DayOfWeek switch {
        System.DayOfWeek.Monday => 1,
        System.DayOfWeek.Tuesday => 2,
        System.DayOfWeek.Wednesday => 3,
        System.DayOfWeek.Thursday => 4,
        System.DayOfWeek.Friday => 5,
        System.DayOfWeek.Saturday => 6,
        System.DayOfWeek.Sunday => 7,
        _ => throw new NotImplementedException(),
    };

    /// <summary>
    /// The day of the year, 1 indexed from January 1st.
    /// </summary>
    [Attribute("Day of Year")]
    public int DayOfYear => Value.DayOfYear;

    [StringLength(9)]
    public string MonthName => Value.ToString("MMMM");

    [StringLength(3)]
    public string MonthShortName => Value.ToString("MMM");

    /// <summary>
    /// The name of the day of the week.
    /// </summary>
    [Attribute("Day of Week Name")]
    [StringLength(8)]
    public DayOfWeek DayOfWeekName => Value.DayOfWeek;

    [Attribute("Day of Week Short Name")]
    [StringLength(3)]
    public string DayOfWeekShortName => $"{DayOfWeekName}"[0..3];

    [AttributeIgnore]
    public int FiscalYearEndingMonth { get; set; } = 12;

    public int FiscalYear => Value.Month <= FiscalYearEndingMonth ? Value.Year : Value.Year + 1;

    public int FiscalQuarter => (Value.Month - FiscalYearEndingMonth + 11) % 12 / 3 + 1;

    [StringLength(7)]
    public string FiscalYearName => $"FY {FiscalYear}";

    public string FiscalQuarterName => $"{FiscalQuarterShortName} {FiscalYear}";

    public string FiscalQuarterShortName => $"Q{FiscalQuarter}";

    public DayType DayType { get; set; }

}
